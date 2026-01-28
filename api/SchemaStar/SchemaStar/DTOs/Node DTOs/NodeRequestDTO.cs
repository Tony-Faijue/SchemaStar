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

        [StringLength(512)]
        public string? NodeImageUrl { get; set;}

        [StringLength(512)]
        public string? NodeAudioUrl { get; set;}

        [StringLength(64)]
        [RegularExpression(@"^image/(jpeg|png|gif|bmp|webp)$", ErrorMessage = "Invalid Image MIME type")]
        public string? NodeImageMime { get; set;}
        
        [StringLength(64)]
        [RegularExpression(@"^audio/(mpeg|wav|ogg|mp4|x-m4a)$", ErrorMessage ="Invalid Audio MIME type")]
        public string? NodeAudioMime { get; set; }

        [Range(1, 20971520, ErrorMessage ="Image size cannot be greater than 20MB and must contain at least 1 byte")]
        public int? NodeImageSize { get; set;}

        [Range(1, 52428800, ErrorMessage = "Audio size cannot be greater than 50MB and must contain at least 1 byte")]
        public int? NodeAudioSize { get; set;}

        [Range(-50000, 50000, ErrorMessage = "Node is too far from center")]
        public double? PositionX { get; set;}

        [Range(-50000, 50000, ErrorMessage = "Node is too far from center")]
        public double? PositionY { get; set;}

        [Required]
        [Range(120, 800, ErrorMessage = "Width size should be between 120 - 800")]
        public int Width { get; set; } = 200;

        [Required]
        [Range(80, 1200, ErrorMessage = "Height size should be between 80 - 1200")]
        public int Height { get; set; } = 152;

        public NodeState? State { get; set;}
    }
}
