using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;

namespace SchemaStar.DataRepositories
{
    public class EdgeRepository : IEdgeRepository
    {
        private readonly SchemastarContext _context;
        public EdgeRepository(SchemastarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Eager loads;
        /// Gets the edges that belong the Nodeweb with the given publicId
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="userId"></param>
        /// <returns>a list of Edges that belong to the Nodeweb</returns>
        public async Task<IEnumerable<Edge>> GetEdgesByNodeWebIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.Edges
                .Include(e => e.FromNode)   //Eager load the FromNode and ToNode 
                .Include(e => e.ToNode)     //Mapper will get the PublicId for the From and To Nodes
                .Include(e => e.Nodeweb)    //Eager load Nodeweb to join for user id check
                .Where(e => e.Nodeweb.PublicId == publicId && e.Nodeweb.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// The edge with the given publicId and matching userId
        /// </summary>
        /// <param name="publicId"></param>
        /// <param name="userId"></param>
        /// <returns>the edge by the given publicId</returns>
        public async Task<Edge?> GetEdgeByPublicIdAsync(byte[] publicId, ulong userId)
        {
            return await _context.Edges
                .Include(e => e.FromNode) //Eager load the FromNode, ToNode, Nodeweb
                .Include(e => e.ToNode)
                .Include(e => e.Nodeweb)
                .FirstOrDefaultAsync(e => e.PublicId == publicId && e.Nodeweb.UserId == userId);
        }
        public async Task<int> DeleteEdgesBulkAsync(IEnumerable<byte[]> publicIds, byte[] nodewebPublicId, ulong userId)
        {
            //Get the internal ids for edges in the schema
            var edgesInSchema = await _context.Edges
                .Where(e => e.Nodeweb.PublicId == nodewebPublicId
                    && e.Nodeweb.UserId == userId)
                .Select(e => new { e.Id, e.PublicId }) //gets the internal and public ids
                .ToListAsync();

            //Identify which internal IDs match the PublicIds to delete
            var idsToRemove = edgesInSchema
                .Where(e => publicIds.Any(p => p.SequenceEqual(e.PublicId)))
                .Select(e => e.Id)
                .ToList();

            if (!idsToRemove.Any()) return 0;

            return await _context.Edges
                .Where(e => idsToRemove.Contains(e.Id)) //SQl can map the list of numbers
                .ExecuteDeleteAsync(); //Bulk SQL Delete
        }
        public void Add(Edge edge) => _context.Edges.Add(edge);
        public void Delete(Edge edge) => _context.Edges.Remove(edge);
        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
