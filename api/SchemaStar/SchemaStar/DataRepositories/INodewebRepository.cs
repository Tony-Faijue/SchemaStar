using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Models;

namespace SchemaStar.DataRepositories
{
    public interface INodewebRepository
    {
        Task<IEnumerable<Nodeweb>> GetAllNodewebsByUserIdAsync(ulong userId);
        Task<Nodeweb?> GetNodewebByPublicIdAsync(byte[] publicId, ulong userId);
        Task<bool> ExistsByNameAsync(ulong userId, string name, byte[]? excludePublicId = null);
        void Add(Nodeweb nodeweb);
        void Delete(Nodeweb nodeweb);
        Task SaveChangesAsync();

    }
}
