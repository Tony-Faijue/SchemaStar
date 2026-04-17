using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.DataRepositories
{
    public class NodeRepository : INodeRepository
    {
        private readonly SchemastarContext _context;
        public NodeRepository(SchemastarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicId"></param>
        /// <returns>returns the internal id given the publicId for the node</returns>
        public async Task<ulong?> GetInternalIdByPublicIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.Nodes
                .Where(n => n.PublicId == publicId && n.NodeWeb.UserId == userId)
                .Select(n => (ulong?)n.Id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a list of nodes that belong the user and node web
        /// </summary>
        /// <param name="nodeWebPublicId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Node>> GetNodesByNodeWebIdAsync(byte[] nodeWebPublicId, ulong userId)
        {
            return await _context.Nodes
                .Include(n => n.NodeWeb) //Include parent Nodeweb
                .Where(n => n.NodeWeb.PublicId == nodeWebPublicId && n.NodeWeb.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the specific node with the publicId
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Node?> GetNodeByPublicIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.Nodes
                .Include (n => n.NodeWeb) //Include parent Nodeweb
                .FirstOrDefaultAsync(n => n.PublicId == publicId && n.NodeWeb.UserId == userId);
        }

        /// <summary>
        /// Eager loads the node assets for the node
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Node?> GetFullNodeByPublicIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.Nodes
                .Include(n => n.NodeWeb) //Include parent Nodeweb
                .Include(n => n.NodeAssets) //Eager Loading
                .FirstOrDefaultAsync(n => n.PublicId == publicId && n.NodeWeb.UserId == userId);
        }
        /// <summary>
        /// Updates in bulk multiple nodes
        /// </summary>
        /// <param name="updatedNodes"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task UpdateNodesBulkAsync(IEnumerable<Node> updatedNodes, byte[] nodewebPublicId, ulong userId)
        {
            //Use of a dict/map for O(1) search
            var updatesMap = new Dictionary<string, Node>();
            foreach (var node in updatedNodes)
            {
                string key = Convert.ToBase64String(node.PublicId); //converts byte array to string for key values
                updatesMap.Add(key, node); //add the node object as the value
            }

            var publicIdsToUpdate = updatedNodes.Select(n => n.PublicId).ToList();
            
            var existingNodes = await _context.Nodes
                .Where(n => publicIdsToUpdate.Contains(n.PublicId) 
                    && n.NodeWeb.PublicId == nodewebPublicId
                    && n.NodeWeb.UserId == userId)
                .ToListAsync();

            foreach (var existing in existingNodes) 
            {
                var key = Convert.ToBase64String(existing.PublicId);
                if (updatesMap.TryGetValue(key, out var update))
                {
                    if (update.NodeName != null) existing.NodeName = update.NodeName;
                    if (update.NodeDescription != null) existing.NodeDescription = update.NodeDescription;

                    existing.PositionX = update.PositionX;
                    existing.PositionY = update.PositionY;

                    if (update.Width > 0) existing.Width = update.Width;
                    if (update.Height > 0) existing.Height = update.Height;

                    existing.State = update.State;
                }
            }
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Deletes Nodes in bulk for the given user
        /// </summary>
        /// <param name="publicIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> DeleteNodesBulkAsync(IEnumerable<byte[]> publicIds, byte[] nodeWebPublicId, ulong userId) 
        {
            return await _context.Nodes
                .Where(n => publicIds.Contains(n.PublicId)
                        && n.NodeWeb.PublicId == nodeWebPublicId
                        && n.NodeWeb.UserId == userId)
                .ExecuteDeleteAsync(); //Bulk SQL deletion
        }
        public void Add(Node node) => _context.Nodes.Add(node);
        public void Delete(Node node) => _context.Nodes.Remove(node);
        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
