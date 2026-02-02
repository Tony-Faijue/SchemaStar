using SchemaStar.Models.Enums;

namespace SchemaStar.DTOs.Node_DTOs
{
    public class NodeResponseDTO
    {
        public Guid publicId { get; set; } //Map to Guid from byte []
        public string NodeName { get; set; } = null!;
        public string? NodeDescription { get; set; }
        public string? NodeImageUrl { get; set; }
        public string? NodeAudioUrl { get; set; }
        public string? NodeImageMime { get; set; }
        public string? NodeAudioMime { get; set; }
        public int? NodeImageSize { get; set; }
        public int? NodeAudioSize { get; set; }
        public double? PositionX { get; set; }
        public double? PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public NodeState? State { get; set; }
    }
}
