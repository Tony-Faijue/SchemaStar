using Microsoft.AspNetCore.Identity;
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
        //Need to use UserManager & SignInManager
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JWTOptions _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JWTOptions> jwt, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Registers new Users
        /// </summary>
        /// <param name="request"></param>
        /// <returns> UserResponseDTO</returns>
        public async Task<UserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request)
        {
            //Check if the user already exists
            //Check if the email exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                //Custom 409 Conflict Exception
                throw new ConflictException("Users");
            }
            //Check if the username exists
            existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null) 
            {
                throw new ConflictException("Users");
            }
            //Generate a new GUID and convert it to MySQL binary format
            var newGuid = Guid.NewGuid();

            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                PublicId = newGuid.ToMySqlBinary()
            };

            //UserManager handles password hashing automatically
            //Create the user
            var result = await _userManager.CreateAsync(user, request.Password);
            //Throw custom exception here if creation fails
            if (!result.Succeeded) 
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            //Respsone DTO
            var response = new UserResponseDTO
            {
                PublicId = newGuid,
                Username = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };

            return response;
        }

        /// <summary>
        /// Authenticate with HttpOnly cookie (web)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CookieAuthResponseDTO> GetTokenWithCookieAsync(TokenRequestModel model)
        {

            var user = await ValidateUseryAsync(model);

            //Generate the token for the verified user
            var jwtToken = CreateJwtToken(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            //Set JWT in HttpOnly cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, //the cookie is not accessible by client side-script
                Secure = true,  //sent through HTTPS
                SameSite = SameSiteMode.Strict, //CRSF protection
                Expires = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes) //Duration of the Cookie
            };

            //append the cookie to the http response
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                    "authToken", //Cookie name
                    tokenString, //JWT Token
                    cookieOptions
                );

            //Return the response DTO
            return new CookieAuthResponseDTO
            {
                IsAuthenticated = true,
                Email = user.Email!,
                UserName = user.UserName!
            };
        }

        /// <summary>
        /// Authenticate with Bearer token (mobile)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BearerAuthResponseDTO> GetTokenWithBearerAsync(TokenRequestModel model)
        {

            var user = await ValidateUseryAsync(model);

            //Generate the token for the verified user
            var jwtToken = CreateJwtToken(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            //Return the response DTO
            return new BearerAuthResponseDTO
            {
                IsAuthenticated = true,
                Email = user.Email!,
                UserName = user.UserName!,
                Token = tokenString!
            };
        }

        /// <summary>
        /// Generates the JWT Token based on the user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        private JwtSecurityToken CreateJwtToken(User user) 
        {
            //Convert the binary to string for user public id
            string publicIdString = user.PublicId.ToGuidFromMySqlBinary().ToString();

            //Create claims for the token information
            var claims = new List<Claim> 
            { 
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!), //Subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Unique ID for the specific token
                new Claim(JwtRegisteredClaimNames.Email, user.Email!), //Email claim
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

        /// <summary>
        /// Method to validate the user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>User object</returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        private async Task<User> ValidateUseryAsync(TokenRequestModel model) 
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new NotFoundException("Users");
            }

            //Use SignInManager for password check
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                lockoutOnFailure: false
                );

            if (!result.Succeeded)
            {
                throw new UnauthorizedException("Users");
            }
            return user;
        }
    }
}
