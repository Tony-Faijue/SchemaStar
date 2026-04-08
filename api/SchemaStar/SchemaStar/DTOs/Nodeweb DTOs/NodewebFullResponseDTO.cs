using SchemaStar.DTOs.Edge_DTOs;
using SchemaStar.DTOs.Node_DTOs;

namespace SchemaStar.DTOs.Nodeweb_DTOs
{
    /// <summary>
    /// Response for eagerloading Nodeweb
    /// </summary>
    public class NodewebFullResponseDTO
    {
        public Guid PublicId { get; set; } // Map to Guid from byte[]
        public string NodeWebName { get; set; } = null!;
        public DateTime? LastLayoutAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //Collection of Nodes and Edges for the Nodeweb
        public List<NodeResponseDTO> Nodes { get; set; } = new List<NodeResponseDTO>();
        public List<EdgeResponseDTO> Edges { get; set; } = new List<EdgeResponseDTO>();
    }
}
