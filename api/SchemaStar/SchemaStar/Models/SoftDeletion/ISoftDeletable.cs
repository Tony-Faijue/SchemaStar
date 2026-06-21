namespace SchemaStar.Models.SoftDeletion
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
