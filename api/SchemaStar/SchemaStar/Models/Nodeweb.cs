using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchemaStar.Models;

[Table("nodeweb")]
public partial class Nodeweb
{
    [Key]
    [Column("id")]
    public ulong Id { get; set; }

    [Required]
    [Column("public_id", TypeName = "binary(16)")] //Force EF Core to treat it as fixed-length binary instead of varbinary
    [MaxLength(16)]
    public byte[] PublicId { get; set; } = null!;

    [Required]
    [Column("node_web_name")]
    [StringLength(255)]
    public string NodeWebName { get; set; } = null!;

    [Column("last_layout_at")]
    public DateTime? LastLayoutAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Required]
    [Column("user_id")]
    public ulong UserId { get; set; }

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();

    public virtual User User { get; set; } = null!;
}
