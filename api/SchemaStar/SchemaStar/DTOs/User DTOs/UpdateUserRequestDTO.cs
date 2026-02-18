using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs
{
    public class UpdateUserRequestDTO
    {
        [StringLength(255)]
        public string? Username { get; set; }

        [StringLength(255), EmailAddress]
        public string? Email { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
