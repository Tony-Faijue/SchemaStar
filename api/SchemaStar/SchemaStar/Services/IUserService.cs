using SchemaStar.DTOs;
using SchemaStar.DTOs.Authentication_DTOs;

namespace SchemaStar.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request);
        Task<AuthenticationResponseDTO> GetTokenAsync(TokenRequestModel model);
    }
}
