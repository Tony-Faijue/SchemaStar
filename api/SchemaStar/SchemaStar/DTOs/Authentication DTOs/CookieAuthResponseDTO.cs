namespace SchemaStar.DTOs.Authentication_DTOs
{
    /// <summary>
    /// Response for cookie token authentication (web)
    /// </summary>
    public class CookieAuthResponseDTO
    {
        public string Message { get; set; } = null!;
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
