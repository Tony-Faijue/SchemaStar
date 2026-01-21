using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs
{
    public class UpdateUserRequestDTO
    {
        [Required, StringLength(255)]
        public string Username { get; set; } = null!;

        [Required, StringLength(255), EmailAddress]
        public string Email { get; set; } = null!;
    }
}
