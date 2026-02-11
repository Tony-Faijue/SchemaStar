namespace SchemaStar.DTOs.Authentication_DTOs
{
    public class AuthenticationResponseDTO
    {
        public string Message { get; set; } = null!;
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;

    }
}
