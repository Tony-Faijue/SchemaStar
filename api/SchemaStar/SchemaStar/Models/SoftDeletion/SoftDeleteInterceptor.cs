using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SchemaStar.Models.SoftDeletion
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        /// <summary>
        /// Overrides the SavingChangesAsync() for entities that implement ISoftDeletable
        /// when .Remove() is called on the entity an UPDATE statement is used instead of DELETE statement
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null) return ValueTask.FromResult(result);

            //iterate over enitities that implemente ISoftDeletable
            foreach (var entry in eventData.Context.ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State != EntityState.Deleted) continue; //Skip entity unless marked as Delete

                entry.State = EntityState.Modified; //change the state from Deleted to Modified
                entry.Entity.IsDeleted = true; // set IsDeleted true
                entry.Entity.DeletedAt = DateTime.UtcNow;

                // Cascade soft delete to loaded child entities
                CascadeSoftDelete(eventData.Context, entry.Entity);
            }
            return ValueTask.FromResult(result);
        }

        /// <summary>
        /// Soft Deletion which updates/modify the entities instead of hard deletion
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parentEntity"></param>
        private static void CascadeSoftDelete(DbContext context, ISoftDeletable parentEntity) 
        {
            if (parentEntity is Node node) 
            {
                var timeStamp = DateTime.UtcNow;

                //Only query edges from the db that are not marked as isDeleted = true, where
                // this node is either the from node or to node
                var connectedEdges = context.Set<Edge>()
                    .Where(e => (e.FromNodeId == node.Id || e.ToNodeId == node.Id) && !e.IsDeleted)
                    .ToList();

                // Modify the edges by updates to soft deletion instead of hard deletions
                foreach (var edge in connectedEdges) 
                {
                    var edgeEntry = context.Entry(edge);

                    // Check if edge is already deleted avoid updating it
                    if (edge.IsDeleted) continue;

                    edgeEntry.State = EntityState.Modified;
                    edge.IsDeleted = true;
                    edge.DeletedAt = timeStamp;
                }

                //Query node assets that are not marked isDeleted = true 
                var connectedAssets = context.Set<NodeAsset>()
                    .Where(a => a.Node.Id == node.Id && !a.IsDeleted)
                    .ToList();

                //Modify the node assets by updates to soft deletion
                foreach (var asset in connectedAssets)
                {
                    var assetEntry = context.Entry(asset);

                    // Check if node asset is already deleted avoid updating it
                    if (asset.IsDeleted) continue;

                    assetEntry.State = EntityState.Modified;
                    asset.IsDeleted = true;
                    asset.DeletedAt = timeStamp;
                }
            }
        }
    }
}
