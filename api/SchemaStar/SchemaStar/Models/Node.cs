using SchemaStar.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchemaStar.Models;

[Table("node")]
public partial class Node
{
    [Key]
    [Column("id")]
    public ulong Id { get; set; }

    [Required]
    [Column("public_id", TypeName ="binary(16)")] //Force EF Core to treat it as fixed-length binary instead of varbinary
    [MaxLength(16)]
    public byte[] PublicId { get; set; } = null!;

    [Required]
    [Column("node_name")]
    [StringLength(255)]
    public string NodeName { get; set; } = null!;

    [Column("node_description")]
    [StringLength(16383)]   //Maxium Bytes to match MySQl's limit
    public string? NodeDescription { get; set; }

    [Column("node_image_url")]
    [StringLength(512)]
    public string? NodeImageUrl { get; set; }

    [Column("node_audio_url")]
    [StringLength(512)]
    public string? NodeAudioUrl { get; set; }

    [Column("node_image_mime")]
    [StringLength(64)]
    public string? NodeImageMime { get; set; }

    [Column("node_audio_mime")]
    [StringLength(64)]
    public string? NodeAudioMime { get; set; }

    [Column("node_image_size")]
    [Range(0, 20971520, ErrorMessage="Image size cannot be greater than 20MB")]
    public int? NodeImageSize { get; set; }

    [Column("node_audio_size")]
    [Range(0, 52428800, ErrorMessage = "Audio size cannot be greater than 50MB")]
    public int? NodeAudioSize { get; set; }

    [Column("position_x")]
    [Range(-50000, 50000, ErrorMessage = "Node is too far from center")]
    public double? PositionX { get; set; }

    [Column("position_y")]
    [Range(-50000, 50000, ErrorMessage = "Node is too far from center")]
    public double? PositionY { get; set; }

    [Required]
    [Column("width")]
    [Range(120, 800, ErrorMessage = "Width size should be between 120 - 800")]
    public int Width { get; set; } = 200; //Default for text node

    [Required]
    [Column("height")]
    [Range(80, 1200, ErrorMessage = "Height size should be between 80 - 1200")]
    public int Height { get; set; } = 152; //Default for text node

    [Column("state")]
    public NodeState State { get; set; } = NodeState.Unlocked; //Uses Enum for state management

    [Required]
    [Column("created_by")]
    public ulong CreatedBy { get; set; }

    [Column("updated_by")]
    public ulong? UpdatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Required]
    [Column("node_web_id")]
    public ulong NodeWebId { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Edge> EdgeFromNodes { get; set; } = new List<Edge>();

    public virtual ICollection<Edge> EdgeToNodes { get; set; } = new List<Edge>();

    public virtual Nodeweb NodeWeb { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
