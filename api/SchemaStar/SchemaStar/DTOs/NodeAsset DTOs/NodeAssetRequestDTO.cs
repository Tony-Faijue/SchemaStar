using SchemaStar.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs.NodeAsset_DTOs
{
    public class NodeAssetRequestDTO
    {
        public string? NodeAssetName { get; set; }
        public NodeAssetEnums.NodeAssetType NodeAssetType { get; set; }
        public NodeAssetEnums.NodeAssetSource NodeAssetSource { get; set; }
        public string? Url { get; set; }
        public string? MimeType { get; set; }
        public int? FileSize { get; set; }
        public string? BlobPath { get; set; }

        [Required]
        public Guid NodeId { get; set; }
    }
}
