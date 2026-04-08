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
            return new NodewebFullResponseDTO
            {
                PublicId = web.PublicId.ToGuidFromMySqlBinary(),
                NodeWebName = web.NodeWebName,
                CreatedAt = web.CreatedAt,
                UpdatedAt = web.UpdatedAt,
                LastLayoutAt = web.LastLayoutAt,
                Nodes = web.Nodes.Select(n => n.ToResponseDTO()).ToList(),
                Edges = web.Edges.Select(e => e.ToResponseDTO()).ToList()
            };
        }
    }
}
