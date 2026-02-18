using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchemaStar.Models;

//Map this model User to users table in MySQL
[Table("users")]

public partial class User : IdentityUser<ulong>
{
       
    [Required]
    [Column("public_id", TypeName = "binary(16)")] //Force EF Core to treat it as fixed-length binary instead of varbinary
    [MaxLength(16)]
    public byte[] PublicId { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    //Relationship: user can have many nodewebs
    public ICollection<Nodeweb> Nodewebs { get; set; } = new List<Nodeweb>();

    public virtual ICollection<Node> NodeCreatedByNavigations { get; set; } = new List<Node>();

    public virtual ICollection<Node> NodeUpdatedByNavigations { get; set; } = new List<Node>();


}
