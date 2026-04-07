using SchemaStar.Models;

namespace SchemaStar.DataRepositories
{
    public interface INodeAssetRepository
    {

        Task<IEnumerable<NodeAsset>> GetNodeAssetsByNodeIdAsync(byte[] NodePublicId, ulong userId);
        Task<NodeAsset?> GetNodeAssetByPublicIdAsync(byte[] publicId, ulong userId);
        void Add(NodeAsset nodeAsset);
        void Delete(NodeAsset nodeAsset);
        Task SaveChangesAsync();
    }
}
