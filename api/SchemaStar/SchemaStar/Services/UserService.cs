using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchemaStar.DTOs;
using SchemaStar.DTOs.Authentication_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.JWT;
using SchemaStar.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchemaStar.Services
{
    public class UserService : IUserService
    {
        private readonly SchemastarContext _context;
        private readonly JWTOptions _jwt;
        public UserService(SchemastarContext context, IOptions<JWTOptions> jwt)
        {
            _context = context;
            _jwt = jwt.Value;
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

        public async Task<AuthenticationResponseDTO> GetTokenAsync(TokenRequestModel model)
        {

            var authenticationResponseDTO = new AuthenticationResponseDTO();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                throw new NotFoundException("Users");
            }

            //Use the BCrypt Verify method for password verification
            bool validPass = BCrypt.Net.BCrypt.Verify(model.Password, user.Pass);

            if (!validPass)
            {
                throw new UnauthorizedException("Users");
            }

            //Generate the token for the verified user
            var jwtToken = CreateJwtToken(user);

            //Return the response DTO
            return new AuthenticationResponseDTO
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Email = user.Email,
                UserName = user.Username
            };
        }

        private JwtSecurityToken CreateJwtToken(User user) 
        {
            //Convert the binary to string for user public id
            string publicIdString = new Guid(user.PublicId).ToString();

            //Create claims for the token information
            var claims = new List<Claim> 
            { 
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), //Subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Unique ID for the specific token
                new Claim(JwtRegisteredClaimNames.Email, user.Email), //Email claim
                new Claim("uid", publicIdString) //public id claim
            };

            //Create a Symmetric Key using the Secret string from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            //Define the algorithm HmacSha256 for signing the token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Create and return the token
            return new JwtSecurityToken
                (
                    issuer: _jwt.Issuer, //Issuer
                    audience: _jwt.Audience, //Audience
                    claims: claims, //Data Payload
                    expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes), //Expiration Time
                    signingCredentials: creds //The signature
                );
        }
    }
}
