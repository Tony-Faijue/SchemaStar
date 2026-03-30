using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NodewebsController : ControllerBase
    {
        private readonly SchemastarContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<NodewebsController> _logger;

        public NodewebsController(SchemastarContext context, IUserService userService, ILogger<NodewebsController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        // GET: api/Nodewebs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NodewebResponseDTO>>> GetNodewebs()
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access these NodeWebs");
             
            return await _context.Nodewebs
                .Where(n => n.UserId == userId)
                .Select(n => new NodewebResponseDTO
                {
                    PublicId = n.PublicId.ToGuidFromMySqlBinary(),
                    NodeWebName = n.NodeWebName,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    LastLayoutAt = n.LastLayoutAt,
                }).ToListAsync();
        }

        // GET: api/Nodewebs/{guid}
        [HttpGet("{publicId}")]
        public async Task<ActionResult<NodewebResponseDTO>> GetNodeweb(Guid publicId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access this NodeWeb");

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var nodeweb = await _context.Nodewebs
                .Where(n => n.PublicId == publicIdBytes && n.UserId == userId)
                .Select(n => new NodewebResponseDTO
                {
                    PublicId = n.PublicId.ToGuidFromMySqlBinary(),
                    NodeWebName = n.NodeWebName,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    LastLayoutAt = n.LastLayoutAt,
                }).FirstOrDefaultAsync();

            if (nodeweb == null)
            {
                _logger.LogWarning("NodeWeb {PublicId} not found for User {UserId}", publicId, userId);
                throw new NotFoundException("NodeWeb does exists for the user");
            }
            
            return nodeweb;
        }

        // PATCH: api/Nodewebs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{publicId}")]
        public async Task<IActionResult> UpdateNodeweb(Guid publicId, NodewebRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to modify this NodeWeb"); 

            byte[] publicIdBytes = publicId.ToMySqlBinary();
            var nodeweb = await _context.Nodewebs
                .Where(n => n.UserId == userId)
                .FirstOrDefaultAsync(n => n.PublicId == publicIdBytes);

            if (nodeweb == null)
            {
                _logger.LogWarning("NodeWeb {PublicId} for User {UserId} not found", publicId, userId);
                throw new NotFoundException("NodeWeb does not exist for this user");
            }

            //Check for duplicate nodeweb names per the user
            bool isDuplicate = await _context.Nodewebs.AnyAsync(n =>
             n.UserId == nodeweb.UserId &&
             n.NodeWebName == request.NodeWebName && //check if request name matches existing one
             n.PublicId != publicIdBytes
            );

            if (isDuplicate)
            {
                _logger.LogWarning("Duplicate name for NodeWeb {PublicId} found for User {UserId}", nodeweb.PublicId, userId);
                throw new ConflictException("NodeWeb with this name already exists");
            }

            //Update the name
            nodeweb.NodeWebName = request.NodeWebName;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("NodeWeb: {PublicId} has been updated by {UserId}", nodeweb.PublicId, userId);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Concurrency Error: Database update for NodeWeb {PublicId} conflicting with an existing update", nodeweb.PublicId);
                throw new ConflictException("Record was updated elsewhere by another process");
            }
            catch (DbUpdateException) 
            {
                _logger.LogError("NodeWeb {PublicId} name is invalid make sure the name is unique", nodeweb.PublicId);
                throw new ArgumentException("Unable to save, make sure node web name is unqiue.");
            }

            return NoContent();
        }

        // POST: api/Nodewebs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NodewebResponseDTO>> PostNodeweb(NodewebRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to create NodeWeb");

            //Check if nodeweb with the request name already created for the user
            bool isDuplicate = await _context.Nodewebs.
                AnyAsync(n => n.UserId == userId && n.NodeWebName == request.NodeWebName);

            if (isDuplicate)
            {
                _logger.LogWarning("Duplicate NodeWeb name found could not create new NodeWeb with name {NodeWebName} for User {UserId}", request.NodeWebName, userId);
                throw new ConflictException("NodeWeb name already exists for this user");
            }

            var newGuid = Guid.NewGuid(); //Generate a new GUID and convert to MySQL binary format

            var nodeweb = new Nodeweb
            {
                PublicId = newGuid.ToMySqlBinary(),
                NodeWebName = request.NodeWebName,
                UserId = (ulong) userId
            };

            var response = new NodewebResponseDTO
            {
                PublicId = newGuid,
                NodeWebName = nodeweb.NodeWebName,
                CreatedAt = nodeweb.CreatedAt,
                UpdatedAt = nodeweb.UpdatedAt,
            };
            
            _context.Nodewebs.Add(nodeweb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("NodeWeb {PublicId} created for User {UserId}", response.PublicId, userId);

            return CreatedAtAction(nameof(GetNodeweb), new { publicId = response.PublicId }, response);
        }

        // DELETE: api/Nodewebs/{guid}
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteNodeweb(Guid publicId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to delete NodeWeb");

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var nodeweb = await _context.Nodewebs
                .Where(n => n.UserId == userId)
                .FirstOrDefaultAsync(n => n.PublicId == publicIdBytes);

            if (nodeweb == null)
            {
                _logger.LogWarning("NodeWeb {PublicId} not found for User {UserId}", publicId, userId);
                throw new NotFoundException("NodeWeb does not exists");
            }
            
            _context.Nodewebs.Remove(nodeweb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("NodeWeb {PublicId} has been deleted by User {UserId}", nodeweb.PublicId, userId);

            return NoContent();
        }

    }
}
