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
        public void Add(Node node) => _context.Nodes.Add(node);
        public void Delete(Node node) => _context.Nodes.Remove(node);
        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
