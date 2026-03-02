namespace SchemaStar.DTOs.Authentication_DTOs
{
    /// <summary>
    /// Response for Bearer token authentication (mobile)
    /// </summary>
    public class BearerAuthResponseDTO
    {
        public string Message { get; set; } = null!;
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
