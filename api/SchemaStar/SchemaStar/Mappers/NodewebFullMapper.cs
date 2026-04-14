using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Mappers
{
    /// <summary>
    /// Mapper for Nodeweb to convert Nodeweb to NodewebFullResponseDTO
    /// </summary>
    public static class NodewebFullMapper
    {
        public static NodewebFullResponseDTO ToFullResponseDTO(this Nodeweb web)
        {
            var nodewebPublicId = web.PublicId.ToGuidFromMySqlBinary();

            return new NodewebFullResponseDTO
            {
                PublicId = nodewebPublicId,
                NodeWebName = web.NodeWebName,
                CreatedAt = web.CreatedAt,
                UpdatedAt = web.UpdatedAt,
                LastLayoutAt = web.LastLayoutAt,
                Nodes = web.Nodes.Select(n => n.ToResponseDTO(nodewebPublicId)).ToList(),
                Edges = web.Edges.Select(e => e.ToResponseDTO(nodewebPublicId)).ToList()
            };
        }
    }
}
