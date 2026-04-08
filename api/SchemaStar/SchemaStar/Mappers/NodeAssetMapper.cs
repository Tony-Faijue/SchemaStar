using SchemaStar.DTOs.NodeAsset_DTOs;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Mappers
{
    /// <summary>
    /// Mapper for NodeAsset to convert NodeAsset to NodeAssetResponseDTO
    /// </summary>
    public static class NodeAssetMapper
    {
        public static NodeAssetResponseDTO ToResponseDTO(this NodeAsset asset)
        {
            return new NodeAssetResponseDTO
            {
                PublicId = asset.PublicId.ToGuidFromMySqlBinary(),
                NodeAssetName = asset.NodeAssetName,
                NodeAssetType = asset.NodeAssetType,
                NodeAssetSource = asset.NodeAssetSource,
                Url = asset.Url,
                MimeType = asset.MimeType,
                FileSize = asset.FileSize,
                BlobPath = asset.BlobPath,
            };
        }
    }
}
