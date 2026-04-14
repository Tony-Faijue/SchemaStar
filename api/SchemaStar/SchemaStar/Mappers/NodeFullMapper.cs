using SchemaStar.DTOs.Node_DTOs;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Mappers
{
    public static class NodeFullMapper
    {
        public static NodeFullResponseDTO ToFullResponseDTO(this Node node)
        {
            var nodePublicId = node.PublicId.ToGuidFromMySqlBinary();

            return new NodeFullResponseDTO
            {
                PublicId = node.PublicId.ToGuidFromMySqlBinary(),
                NodeName = node.NodeName,
                NodeDescription = node.NodeDescription,
                PositionX = node.PositionX,
                PositionY = node.PositionY,
                Width = node.Width,
                Height = node.Height,
                State = node.State,
                CreatedAt = node.CreatedAt,
                UpdatedAt = node.UpdatedAt,
                NodeAssets = node.NodeAssets.Select(n => n.ToResponseDTO(nodePublicId)).ToList() //pass the nodePublicId to the nodeassets
            };
        }
    }
}
