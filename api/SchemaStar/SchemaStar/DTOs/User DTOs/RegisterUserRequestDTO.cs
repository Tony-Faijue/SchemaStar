using System.ComponentModel.DataAnnotations;

namespace SchemaStar.DTOs
{
    public class RegisterUserRequestDTO
    {
        //Send Request to Register User
        [Required, StringLength(255)]
        public string Username { get; set; } = null!;

        [Required, StringLength(255), EmailAddress]
        public string Email { get; set; } = null!;

        [Required, StringLength(255, MinimumLength = 8), RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
        // "Must contain one uppercase letter, one number, and one symbol."
        // Password will be hashed
        public string Password { get; set; } = null!;

    }
}
