using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Mappers
{
    /// <summary>
    /// Mapper for Nodeweb to convert Nodeweb to NodewebResponseDTO
    /// </summary>
    public static class NodewebMapper
    {
        public static NodewebResponseDTO ToResponseDTO(this Nodeweb web) 
        {
            return new NodewebResponseDTO
            {
                PublicId = web.PublicId.ToGuidFromMySqlBinary(),
                NodeWebName = web.NodeWebName,
                CreatedAt = web.CreatedAt,
                UpdatedAt = web.UpdatedAt,
                LastLayoutAt = web.LastLayoutAt
            };
        }
    }
}
