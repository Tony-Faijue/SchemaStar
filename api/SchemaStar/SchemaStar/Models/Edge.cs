using SchemaStar.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchemaStar.Models;

public partial class Edge
{
    [Key]
    [Column("id")]
    public ulong Id { get; set; }

    [Required]
    [Column("public_id", TypeName = "binary(16)")] //Force EF Core to treat it as fixed-length binary instead of varbinary
    [MaxLength(16)]
    public byte[] PublicId { get; set; } = null!;

    [Column("edge_type")]
    public EdgeType? EdgeType { get; set; }

    [Required]
    [Column("from_node_id")]
    public ulong FromNodeId { get; set; }

    [Required]
    [Column("to_node_id")]
    public ulong ToNodeId { get; set; }

    public virtual Node FromNode { get; set; } = null!;

    public virtual Node ToNode { get; set; } = null!;
}
