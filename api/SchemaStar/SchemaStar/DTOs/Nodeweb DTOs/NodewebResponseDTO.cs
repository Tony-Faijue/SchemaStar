using System.Security.Policy;

namespace SchemaStar.DTOs.Nodeweb_DTOs
{
    //Expose Fields for Get Requests
    public class NodewebResponseDTO
    {
        public Guid PublicId { get; set; } // Map to Guid from byte[]
        public string NodeWebName { get; set; } = null!;
        public DateTime? LastLayoutAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
