using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;
using SchemaStar.DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Services;
using SchemaStar.DTOs.Authentication_DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchemaStar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SchemastarContext _context;
        //Custom User Service
        private readonly IUserService _userService;
        //Using Serilog Logger
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<User> userManager, IUserService userService, SchemastarContext context, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _userService = userService;
            _context = context;
            _logger = logger;
        }

        // GET: api/Users
        //Only admin role should access all users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            return users.Select(u => new UserResponseDTO
            {
                PublicId = u.PublicId.ToGuidFromMySqlBinary(),
                Username = u.UserName!,
                Email = u.Email!,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList();
        }

        // GET: api/Users/{publicId}
        [Authorize]
        [HttpGet("{publicId}")]
        public async Task<ActionResult<UserResponseDTO>> GetUser(Guid publicId)
        {
            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var user = await _context.Users
                .Where(u => u.PublicId == publicIdBytes)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                //Use Custom NotFoundException
                throw new NotFoundException("Users", publicId);
            }

            return new UserResponseDTO
            {
                PublicId = user.PublicId.ToGuidFromMySqlBinary(),
                Username = user.UserName!,
                Email = user.Email!,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        // PATCH: api/Users/{publicId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPatch("{publicId}")]
        public async Task<ActionResult<UserResponseDTO>> UpdateUser(Guid publicId, UpdateUserRequestDTO request)
        {
            //Verify the same user is updating themself
            //Get the UID from the token claims
            var tokenUid = User.FindFirstValue("uid");

            //Compare uid token strings
            if (tokenUid == null || !string.Equals(tokenUid, publicId.ToString(), StringComparison.OrdinalIgnoreCase)) 
            {
                throw new ForbiddenException("Users");
            }

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var isChanged = request.Email != null || request.Username != null || request.PhoneNumber != null;

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.PublicId == publicIdBytes);

            if (user == null)
            {
                throw new NotFoundException("Users");
            }

            //Update fields
            if (request.Username != null)
            {
                user.UserName = request.Username;
            }
            if (request.Email != null)
            {
                user.Email = request.Email;
            }
            if (request.PhoneNumber != null) 
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                //Create Custom Excetpion
                var errors = result.Errors.Select(e => e.Description);
                return Conflict(new{ errors});
            }
            //Check if any value has changed and return a user response dto
            if (isChanged)
            {
                var response = new UserResponseDTO
                {
                    PublicId = user.PublicId.ToGuidFromMySqlBinary(),
                    Username = user.UserName!,
                    Email = user.Email!
                };
                return Ok(response);
            }

            return NoContent();
        }

        // POST: api/Users (Register Users)
        // Register user with Cookie Token
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> PostUser(RegisterUserRequestDTO request)
        {
            //Call RegisterUserSync form UserService
            var response = await _userService.RegisterUserAsync(request);

            //Email Verification Logic

            return CreatedAtAction(nameof(GetUser), new { publicId = response.PublicId }, response);
        }

        // DELETE: api/Users/{publicId}
        [Authorize]
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteUser(Guid publicId)
        {
            //Check to make sure the user is deleting themself
            //Get the UID from the token claims
            var tokenUid = User.FindFirstValue("uid");

            //Compare uid token strings
            if (tokenUid == null || !string.Equals(tokenUid, publicId.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Users");
            }

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.PublicId == publicIdBytes);
            
            if (user == null)
            {
                throw new NotFoundException("Users");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) 
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        /// <summary>
        /// Login with HttpOnly cookie (web)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> LoginWithCookie(TokenRequestModel model)
        {
            var result = await _userService.GetTokenWithCookieAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// Login with Bearer token (mobile and testing)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("token/bearer")]
        public async Task<IActionResult> LoginWithBearer(TokenRequestModel model) 
        {
            var result = await _userService.GetTokenWithBearerAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// Logout user by clearing authentication cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        public IActionResult LogOutWithCookie() 
        {
            Response.Cookies.Delete("authToken", new CookieOptions {
                Path = "/",
                HttpOnly = true
            });
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Gets the Current User Info only if the cookie is valid
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CookieAuthResponseDTO>> GetCurrentUserInfo() {

            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("Current User is invalid");

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) throw new NotFoundException("User does not exists");

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
    }
}
