using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs.Authentication_DTOs
{
    public class TokenRequestModel
    {
        [Required]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Required, StringLength(255, MinimumLength = 8), RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
        public string Password { get; set; } = null!;
    }
}
