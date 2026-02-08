using SchemaStar.DTOs;

namespace SchemaStar.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request);
    }
}
