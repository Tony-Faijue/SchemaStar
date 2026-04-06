using SchemaStar.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs.Node_DTOs
{
    public class NodeRequestDTO
    {
        [Required, StringLength(255, MinimumLength = 1)]
        public string NodeName { get; set; } = null!;

        [StringLength(16383)]
        public string? NodeDescription {get; set;}
        
        public double PositionX { get; set;}

        public double PositionY { get; set;}

        [Required]
        [Range(120, 2000, ErrorMessage = "Width size should be between 120 - 2000")]
        public int Width { get; set; } = 200;

        [Required]
        [Range(80, 5000, ErrorMessage = "Height size should be between 80 - 5000")]
        public int Height { get; set; } = 152;

        public NodeState State { get; set; } = NodeState.Unlocked;

        [Required]
        public Guid NodeWebId { get; set; }
    }
}
