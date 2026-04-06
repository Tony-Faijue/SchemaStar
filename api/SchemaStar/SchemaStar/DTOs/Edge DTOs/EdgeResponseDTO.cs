using SchemaStar.Models.Enums;

namespace SchemaStar.DTOs.Edge_DTOs
{
    public class EdgeResponseDTO
    {
        public Guid PublicId { get; set; }
        public EdgeType? EdgeType { get; set; }
        public string? UiMetadata { get; set; }
        public Guid FromNodeId { get; set; }
        public Guid ToNodeId { get; set; }
        public Guid NodeWebId { get; set; }
    }
}
