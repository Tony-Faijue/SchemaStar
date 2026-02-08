using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaStar.Models;
using SchemaStar.DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Services;

namespace SchemaStar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SchemastarContext _context;
        //Custom User Service
        private readonly IUserService _userService;

        public UsersController(SchemastarContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            return await _context.Users.Select(u => new UserResponseDTO
            {
                PublicId = new Guid(u.PublicId),
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToListAsync();

        }

        // GET: api/Users/{guid}
        [HttpGet("{publicId}")]
        public async Task<ActionResult<UserResponseDTO>> GetUser(Guid publicId)
        {
            byte[] publicIdBytes = publicId.ToByteArray();

            var user = await _context.Users
                .Where(u => u.PublicId == publicIdBytes)
                .Select(u => new UserResponseDTO
                {
                    PublicId = new Guid(u.PublicId),
                    Username = u.Username,
                    Email = u.Email,
                    CreatedAt=u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).FirstOrDefaultAsync();

            if (user == null)
            {
               // return NotFound();
               //Use Custom NotFoundException
                throw new NotFoundException("Users", publicId);
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{publicId}")]
        public async Task<IActionResult> UpdateUser(Guid publicId, UpdateUserRequestDTO request)
        {
            byte[] publicIdBytes = publicId.ToByteArray();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicId == publicIdBytes);
            
            if (user == null)
            {
                return NotFound();
            }

            //Update fields
            //Make sure to update field if it is provided
            user.Username = request.Username;
            user.Email = request.Email;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) 
            {
                return Conflict("Username or Email already exists.");
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
            byte[] publicIdBytes = publicId.ToByteArray();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicId == publicIdBytes);
            
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
