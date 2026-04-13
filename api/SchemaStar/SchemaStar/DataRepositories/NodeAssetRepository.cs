using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;

namespace SchemaStar.DataRepositories
{
    public class NodeAssetRepository : INodeAssetRepository
    {
        private readonly SchemastarContext _context;

        public NodeAssetRepository(SchemastarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a list of node assets
        /// </summary>
        /// <param name="nodePublicId"></param>
        /// <param name="userId"></param>
        /// <returns> a list of NodeAssets that belong the node and use</returns>
        public async Task<IEnumerable<NodeAsset>> GetNodeAssetsByNodeIdAsync(byte[] nodePublicId, ulong userId)
        {
            return await _context.NodeAssets
                .Include(n => n.Node) //Inlcude parent Node
                .Where(n => n.Node.PublicId == nodePublicId && n.Node.NodeWeb.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the specific node asset
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="userId"></param>
        /// <returns>a node asset with the given public id</returns>
        public async Task<NodeAsset?> GetNodeAssetByPublicIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.NodeAssets
                .Include(n => n.Node) //Inlcude parent Node
                .FirstOrDefaultAsync(n => n.PublicId == publicId && n.Node.NodeWeb.UserId == userId);
        }

        public void Add(NodeAsset nodeAsset) => _context.NodeAssets.Add(nodeAsset);

        public void Delete(NodeAsset nodeAsset) => _context.NodeAssets.Remove(nodeAsset);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();

    }
}
