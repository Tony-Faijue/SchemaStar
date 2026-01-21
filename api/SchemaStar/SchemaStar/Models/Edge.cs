using System;
using System.Collections.Generic;

namespace SchemaStar.Models;

public partial class Edge
{
    public ulong Id { get; set; }

    public byte[] PublicId { get; set; } = null!;

    public string? EdgeType { get; set; }

    public ulong FromNodeId { get; set; }

    public ulong ToNodeId { get; set; }

    public virtual Node FromNode { get; set; } = null!;

    public virtual Node ToNode { get; set; } = null!;
}
