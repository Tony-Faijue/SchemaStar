using SchemaStar.Models.Enums;

namespace SchemaStar.DTOs.NodeAsset_DTOs
{
    public class NodeAssetResponseDTO
    {
        public Guid PublicId { get; set; }
        public string? NodeAssetName { get; set; }
        public NodeAssetEnums.NodeAssetType NodeAssetType { get; set; }
        public NodeAssetEnums.NodeAssetSource NodeAssetSource { get; set; }
        public string? Url { get; set; }
        public string? MimeType { get; set; }
        public int? FileSize { get; set; }
        public string? BlobPath { get; set; }

       // public Guid NodeId { get; set; }
    }
}
