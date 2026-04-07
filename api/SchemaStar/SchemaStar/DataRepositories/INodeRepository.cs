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
        Task SaveChangesAsync();

    }
}
