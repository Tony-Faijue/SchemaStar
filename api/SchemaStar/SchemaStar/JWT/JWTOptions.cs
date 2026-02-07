using System.ComponentModel.DataAnnotations;

namespace SchemaStar.JWT
{
    public class JWTOptions
    {
        public const string SectionName = "JWT";

        [Required, MinLength(32)]
        public string Key { get; set; } = null!;
        [Required]
       
        public string Issuer { get; set; } = null!;
        [Required]
      
        public string Audience { get; set; } = null!;
        [Required, Range(1, 120)]
        
        public double DurationInMinutes { get; set; } = 5.0;

    }
}
