using SchemaStar.Models.Enums;

namespace SchemaStar.DTOs.Node_DTOs
{
    public class NodeResponseDTO
    {
        public Guid PublicId { get; set; } //Map to Guid from byte []
        public string NodeName { get; set; } = null!;
        public string? NodeDescription { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public NodeState State { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid NodeWebId { get; set; }
    }
}
