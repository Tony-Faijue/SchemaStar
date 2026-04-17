using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs.Node_DTOs
{
    public class NodeBulkUpdateRequestDTO : NodeUpdateRequestDTO
    {
        [Required]
        public Guid PublicId { get; set; } //public id is contained in the body instead of the header usually used for single updates
    }
}
