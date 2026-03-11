namespace SchemaStar.DTOs.Authentication_DTOs
{
    /// <summary>
    /// Response for cookie token authentication (web)
    /// </summary>
    public class CookieAuthResponseDTO
    {
        public Guid PublicId { get; set; }
        public bool IsAuthenticated { get; set; } = false;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
