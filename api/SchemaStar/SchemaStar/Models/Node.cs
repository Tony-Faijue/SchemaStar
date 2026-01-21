using System;
using System.Collections.Generic;

namespace SchemaStar.Models;

public partial class Node
{
    public ulong Id { get; set; }

    public byte[] PublicId { get; set; } = null!;

    public string NodeName { get; set; } = null!;

    public string? NodeDescription { get; set; }

    public string? NodeImageUrl { get; set; }

    public string? NodeAudioUrl { get; set; }

    public string? NodeImageMime { get; set; }

    public string? NodeAudioMime { get; set; }

    public int? NodeImageSize { get; set; }

    public int? NodeAudioSize { get; set; }

    public double? PositionX { get; set; }

    public double? PositionY { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? State { get; set; }

    public ulong CreatedBy { get; set; }

    public ulong? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ulong NodeWebId { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Edge> EdgeFromNodes { get; set; } = new List<Edge>();

    public virtual ICollection<Edge> EdgeToNodes { get; set; } = new List<Edge>();

    public virtual Nodeweb NodeWeb { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
