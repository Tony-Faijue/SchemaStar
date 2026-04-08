using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;

namespace SchemaStar.DataRepositories
{
    public class NodewebRepository: INodewebRepository
    {
        private readonly SchemastarContext _context;

        public NodewebRepository(SchemastarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the nodewebs that belong to the user with userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Nodeweb>> GetAllNodewebsByUserIdAsync(ulong userId)
        {
            return await _context.Nodewebs
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the nodeweb with the matching the publicId that belongs to the user
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Nodeweb?> GetNodewebByPublicIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.Nodewebs
                .FirstOrDefaultAsync(n => n.PublicId == publicId && n.UserId == userId);
        }

        /// <summary>
        /// Checks whether a nodeweb exists for the user with the given name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="excludePublicId"></param>
        /// <returns></returns>
        public async Task<bool> ExistsByNameAsync(ulong userId, string name, byte[]? excludePublicId = null)
        {
            var query = _context.Nodewebs
                .Where(n => n.UserId == userId && n.NodeWebName == name);

            //If updating a nodeweb, don't flag the node currently updating
            if (excludePublicId != null) 
            {
                query = query.Where(n => n.PublicId != excludePublicId);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Eager loads the nodes, node assets and edges for the nodeweb
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Nodeweb?> GetFullNodeWebByPublicIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.Nodewebs
                .Include(n => n.Nodes) //Eager Loading
                    .ThenInclude(n => n.NodeAssets)
                .Include(n => n.Edges)
                .FirstOrDefaultAsync(n => n.PublicId == publicId && n.UserId == userId);
        }
        /// <summary>
        /// Add nodeweb 
        /// </summary>
        /// <param name="nodeweb"></param>
        public void Add(Nodeweb nodeweb) => _context.Nodewebs.Add(nodeweb);

        /// <summary>
        /// Removes nodeweb
        /// </summary>
        /// <param name="nodeweb"></param>
        public void Delete(Nodeweb nodeweb) => _context.Nodewebs.Remove(nodeweb);

        /// <summary>
        /// Save changes async
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    }
}
