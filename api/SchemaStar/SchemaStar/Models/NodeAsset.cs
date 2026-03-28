using SchemaStar.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchemaStar.Models
{
    [Table("node_asset")]
    public partial class NodeAsset
    {
        [Key]
        [Column("id")]
        public ulong Id { get; set; }

        [Required]
        [Column("public_id", TypeName = "binary(16)")]
        [MaxLength(16)]
        public byte[] PublicId { get; set; } = null!;

        [Required]
        [Column("asset_type")]
        public NodeAssetEnums.NodeAssetType NodeAssetType { get; set; }

        [Required]
        [Column("asset_source")]
        public NodeAssetEnums.NodeAssetSource NodeAssetSource { get; set; }

        [Column("url")]
        [StringLength(2048)]
        public string? Url { get; set; }

        [Column("mime_type")]
        [StringLength(127)]
        public string MimeType { get; set; } = null!;

        [Column("file_size")]
        [Range(1, 104857600, ErrorMessage = "File size cannot be greater than 100MB and must contain at least 1 byte")]
        public int? FileSize { get; set; }

        [Required]
        [Column("node_id")]
        public ulong NodeId { get; set; }

        [ForeignKey("NodeId")]
        public virtual Node Node { get; set; } = null!;

    }
}
