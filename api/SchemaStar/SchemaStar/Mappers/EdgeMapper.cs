using SchemaStar.DTOs.Edge_DTOs;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Mappers
{
    /// <summary>
    /// Mapper for Edge to convert Edge to EdgeResponseDTO
    /// </summary>
    public static class EdgeMapper
    {
        public static EdgeResponseDTO ToResponseDTO(this Edge edge, Guid? nodeWebPublicId = null)
        {
            return new EdgeResponseDTO
            {
                PublicId = edge.PublicId.ToGuidFromMySqlBinary(),
                EdgeType = edge.EdgeType,
                UiMetadata = edge.UiMetadata,
                FromNodeId = edge.FromNode?.PublicId.ToGuidFromMySqlBinary() ?? Guid.Empty,
                ToNodeId = edge.ToNode?.PublicId.ToGuidFromMySqlBinary() ?? Guid.Empty, //Edge repository eager loads, handles when nodes are null
                NodeWebId = nodeWebPublicId ?? edge.Nodeweb.PublicId.ToGuidFromMySqlBinary() ////Use the parameter otherwise use the loaded parent object
            };
        }
    }
}
