using SchemaStar.Models;

namespace SchemaStar.DataRepositories
{
    public interface IEdgeRepository
    {
        Task<IEnumerable<Edge>> GetEdgesByNodeWebIdAsync(byte[] NodeWebPublicId, ulong userId);
        Task<Edge?> GetEdgeByPublicIdAsync(byte[] publicId, ulong userId);
        void Add(Edge edge);
        void Delete(Edge edge);
        Task SaveChangesAsync();
    }
}
