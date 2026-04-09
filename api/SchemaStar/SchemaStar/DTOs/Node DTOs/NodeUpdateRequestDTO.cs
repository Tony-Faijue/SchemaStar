using SchemaStar.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs.Node_DTOs
{
    public class NodeUpdateRequestDTO
    {
        [StringLength(255, MinimumLength = 1)]
        public string? NodeName { get; set; }

        [StringLength(16383)]
        public string? NodeDescription { get; set; }

        public double? PositionX { get; set; }

        public double? PositionY { get; set; }

        [Range(120, 2000, ErrorMessage = "Width size should be between 120 - 2000")]
        public int? Width { get; set; }

        [Range(80, 5000, ErrorMessage = "Height size should be between 80 - 5000")]
        public int? Height { get; set; }

        public NodeState? State { get; set; }
    }
}
