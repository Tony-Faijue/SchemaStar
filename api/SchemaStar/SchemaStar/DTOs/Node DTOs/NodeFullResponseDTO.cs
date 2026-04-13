using SchemaStar.DTOs.NodeAsset_DTOs;
using SchemaStar.Models.Enums;

namespace SchemaStar.DTOs.Node_DTOs
{
    public class NodeFullResponseDTO
    {
        public Guid PublicId { get; set; } //Map to Guid from byte []
        public string NodeName { get; set; } = null!;
        public string? NodeDescription { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public NodeState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid NodeWebId { get; set; }

        //Collection of NodeAssets for the Node
        public List<NodeAssetResponseDTO> NodeAssets { get; set; } = new List<NodeAssetResponseDTO>();
    }
}
