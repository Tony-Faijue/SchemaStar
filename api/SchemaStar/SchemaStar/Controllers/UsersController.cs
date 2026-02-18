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

        public UsersController(UserManager<User> userManager, IUserService userService, SchemastarContext context)
        {
            _userManager = userManager;
            _userService = userService;
            _context = context;
        }

        // GET: api/Users
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

        // PUT: api/Users/{publicId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{publicId}")]
        public async Task<IActionResult> UpdateUser(Guid publicId, UpdateUserRequestDTO request)
        {
            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.PublicId == publicIdBytes);
            
            if (user == null)
            {
                return NotFound();
            }

            //Update fields
            user.UserName = request.Username;
            user.Email = request.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                //Create Custom Excetpion
                var errors = result.Errors.Select(e => e.Description);
                return Conflict(new{ errors});
            }

            return NoContent();
        }

        // POST: api/Users (Register Users)
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> PostUser(RegisterUserRequestDTO request)
        {
            //Call RegisterUserSync form UserService
            var response = await _userService.RegisterUserAsync(request);

            return CreatedAtAction(nameof(GetUser), new { publicId = response.PublicId }, response);
        }

        // DELETE: api/Users/{guid}
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteUser(Guid publicId)
        {
            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.PublicId == publicIdBytes);
            
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) 
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        // api/Users/token
        [HttpPost("token")]
        public async Task<IActionResult> GetActionAsync(TokenRequestModel model)
        {
            var result = await _userService.GetTokenAsync(model);
            return Ok(result);
        }
    }
}
