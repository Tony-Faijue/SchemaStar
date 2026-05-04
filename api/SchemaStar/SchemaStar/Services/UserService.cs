using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IOptions<JWTOptions> _jwt; //wrap in IOptions
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment; //For development & production context
        private readonly ILogger<UserService> _logger;
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JWTOptions> jwt, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// Registers new Users
        /// </summary>
        /// <param name="request"></param>
        /// <returns> UserResponseDTO</returns>
        public async Task<UserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request)
        {
            //Check if the user already exists
            //Check if the email/username exists
            var emailExists = await _userManager.FindByEmailAsync(request.Email) != null;
            var userNameExists = await _userManager.FindByNameAsync(request.Username) != null;

            if (emailExists || userNameExists)
            {
                _logger.LogWarning("Registration Failed: credentials conflict");
                //Custom 409 Conflict Exception
                throw new ConflictException("Registration Failed: email or username already in use");
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
                //sanitize specific identity errors for conflict exceptions
                var sensitiveErrorCodes = new HashSet<string> { "DuplicateEmail", "DuplicateUserName" };
                var hasDuplicateConflict = result.Errors.Any(e => sensitiveErrorCodes.Contains(e.Code));

                if (hasDuplicateConflict) 
                {
                    _logger.LogWarning("Registration failed: duplicate credential conflict");
                    throw new ConflictException("Registration failed: email or username is already taken");
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Identity Store Error: with following {Errors}", errors);
                throw new ValidationException($"Registration Failed: {errors}");
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

            _logger.LogInformation("New User successfully created");

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
            _logger.LogInformation("JSON Web Token Generated");

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            //Environment Context Development or Production
            bool isDev = _webHostEnvironment.IsDevelopment();
            bool isTesting = _webHostEnvironment.IsEnvironment("Testing");

            //Set JWT in HttpOnly cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, //the cookie is not accessible by client side-script
                Secure = !isDev && !isTesting,  //true - sent through HTTPS for production, false for development/testing
                SameSite = (isDev || isTesting) ? SameSiteMode.Lax : SameSiteMode.Strict, //Strict - CRSF protection, Lax for development/testing
                Expires = DateTime.UtcNow.AddMinutes(_jwt.Value.DurationInMinutes), //Duration of the Cookie
                Path = "/" //ensures the cookie is available for all routes
            };

            //append the cookie to the http response
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                    "authToken", //Cookie name
                    tokenString, //JWT Token
                    cookieOptions
                );

            _logger.LogInformation("Authentication successful: Cookie Issued");

            //Return the response DTO
            return new CookieAuthResponseDTO
            {
                PublicId = user.PublicId.ToGuidFromMySqlBinary(),
                IsAuthenticated = true,
                Email = user.Email!,
                Username = user.UserName!,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
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
            _logger.LogInformation("JSON Web Token Generated");

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            _logger.LogInformation("Authentication successful: Token Issued");

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
        /// Gets the Current UserId with the current claims prinicipal
        /// </summary>
        /// <returns> ulong public id</returns>
        public ulong? GetCurrentUserId() 
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return ulong.TryParse(userIdClaim, out var id) ? id : null;
        }

        /// <summary>
        /// Generates the JWT Token based on the user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        private JwtSecurityToken CreateJwtToken(User user) 
        {   //Check authentication configuration failures
            if (string.IsNullOrEmpty(_jwt.Value.Key)) 
            {
                _logger.LogCritical("JWT Key value is invalid or not configured! Authentication will fail!");
                throw new InvalidOperationException("Internal authentication configuration error");
            }
            if (string.IsNullOrEmpty(_jwt.Value.Issuer))
            {
                _logger.LogCritical("JWT Issuer is invalid or not configured! Authentication will fail!");
                throw new InvalidOperationException("Internal authentication configuration error");
            }
            if (string.IsNullOrEmpty(_jwt.Value.Audience))
            {
                _logger.LogCritical("JWT Audience is invalid or not configured! Authentication will fail!");
                throw new InvalidOperationException("Internal authentication configuration error");
            }
            if (_jwt.Value.DurationInMinutes <= 0) 
            {
                _logger.LogCritical("JWT DurationInMinutes is misconfigured: {Duration}", _jwt.Value.DurationInMinutes);
                throw new InvalidOperationException("Internal authentication configuration error");
            }

            //Convert the binary to string for user public id
            string publicIdString = user.PublicId.ToGuidFromMySqlBinary().ToString();

            //Create claims for the token information
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), //Add ulong datbase ID to the token
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!), //Subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Unique ID for the specific token
                new Claim(JwtRegisteredClaimNames.Email, user.Email!), //Email claim
                new Claim("uid", publicIdString) //public id claim
            };

            //Create a Symmetric Key using the Secret string from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Key));

            //Define the algorithm HmacSha256 for signing the token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Create and return the token
            return new JwtSecurityToken
                (
                    issuer: _jwt.Value.Issuer, //Issuer
                    audience: _jwt.Value.Audience, //Audience
                    claims: claims, //Data Payload
                    expires: DateTime.UtcNow.AddMinutes(_jwt.Value.DurationInMinutes), //Expiration Time
                    signingCredentials: creds //The signature
                );
        }

        /// <summary>
        /// Method to validate the user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>User object</returns>
        /// <exception cref="UnauthorizedException"></exception>
        private async Task<User> ValidateUseryAsync(TokenRequestModel model) 
        {
            //prevent user enumeration with same message
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _logger.LogWarning("Validation failed: invalid credentials");
                throw new UnauthorizedException("Invalid email or password");
            }

            //Use SignInManager for password check
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                lockoutOnFailure: false //set to true in production
                );

            if (!result.Succeeded)
            {
                _logger.LogWarning("Validation failed: invalid credentials");
                throw new UnauthorizedException("Invalid email or password");
            }
            return user;
        }
    }
}
