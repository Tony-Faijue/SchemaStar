using SchemaStar.Models.Enums;

namespace SchemaStar.DTOs
{
    public class EdgeRequestDTO
    {
        public EdgeType? EdgeType { get; set; }
        public string? UiMetadata { get; set; }
        public Guid FromNodeId { get; set; }
        public Guid ToNodeId { get; set; }
        public Guid NodeWebId { get; set; }
    }
}
