using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;

namespace SchemaStar.DataRepositories
{
    public interface INodeRepository
    {
        Task<ulong?> GetInternalIdByPublicIdAsync(byte[] publicId, ulong userId);
        Task<IEnumerable<Node>> GetNodesByNodeWebIdAsync(byte[] nodeWebPublicId, ulong userId);
        Task<Node?> GetNodeByPublicIdAsync(byte[] publicId, ulong userId);
        Task<Node?> GetFullNodeByPublicIdAsync(byte[] publicId, ulong userId);
        void Add(Node node);
        void Delete(Node node);
        Task UpdateNodesBulkAsync(IEnumerable<Node> updatedNodes, byte[] nodewebPublicId, ulong userId);
        Task<int> DeleteNodesBulkAsync(IEnumerable<byte[]> publicIds, byte[] nodewebPublicId, ulong userId);
        Task SaveChangesAsync();

    }
}
