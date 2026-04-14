using SchemaStar.DTOs.Node_DTOs;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Mappers
{
    /// <summary>
    /// Mapper for Node to convert Node to NodeResponseDTO
    /// </summary>
    public static class NodeMapper
    {
        public static NodeResponseDTO ToResponseDTO(this Node node, Guid? nodeWebPublicId = null)
        {
            return new NodeResponseDTO
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
                NodeWebId = nodeWebPublicId ?? node.NodeWeb.PublicId.ToGuidFromMySqlBinary() //Use the parameter otherwise use the loaded parent object
            };
        }
    }
}
