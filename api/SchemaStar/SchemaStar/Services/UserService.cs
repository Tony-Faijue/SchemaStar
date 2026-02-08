using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchemaStar.DTOs;
using SchemaStar.Exceptions;
using SchemaStar.JWT;
using SchemaStar.Models;

namespace SchemaStar.Services
{
    public class UserService : IUserService
    {
        private readonly SchemastarContext _context;
        private readonly JWTOptions? _jwt;
        public UserService(SchemastarContext context) 
        {
            _context = context;
            //param:IOptions<JWTOptions> jwt
            //_jwt = jwt.Value;
        }

        /// <summary>
        /// Registers new Users
        /// </summary>
        /// <param name="request"></param>
        /// <returns> UserResponseDTO</returns>
        public async Task<UserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request) 
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
            {
                //Custom 409 Conflict Exception
                throw new ConflictException("Users");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                //Use BCrypt for hashing
                Pass = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //Respsone DTO
            var response = new UserResponseDTO
            {
                PublicId = new Guid(user.PublicId),
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };

            return response;
        }
    }
}
