using System;
using System.Collections.Generic;

namespace SchemaStar.Models;

public partial class Nodeweb
{
    public ulong Id { get; set; }

    public byte[] PublicId { get; set; } = null!;

    public string NodeWebName { get; set; } = null!;

    public DateTime? LastLayoutAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ulong UserId { get; set; }

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();

    public virtual User User { get; set; } = null!;
}
