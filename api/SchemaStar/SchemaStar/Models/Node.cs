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

    [Column("position_x")]
    public double PositionX { get; set; } = 0;

    [Column("position_y")]
    public double PositionY { get; set; } = 0;

    [Required]
    [Column("width")]
    [Range(120, 2000, ErrorMessage = "Width size should be between 120 - 2000")]
    public int Width { get; set; } = 200; //Default for text node

    [Required]
    [Column("height")]
    [Range(80, 5000, ErrorMessage = "Height size should be between 80 - 5000")]
    public int Height { get; set; } = 152; //Default for text node

    [Column("state")]
    public NodeState State { get; set; } = NodeState.Unlocked;  //Uses Enum for state management

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Required]
    [Column("node_web_id")]
    public ulong NodeWebId { get; set; }

    public virtual ICollection<Edge> EdgeFromNodes { get; set; } = new List<Edge>();

    public virtual ICollection<Edge> EdgeToNodes { get; set; } = new List<Edge>();

    public virtual ICollection<NodeAsset> NodeAssets { get; set; } = new List<NodeAsset>();

    public virtual Nodeweb NodeWeb { get; set; } = null!;

}
