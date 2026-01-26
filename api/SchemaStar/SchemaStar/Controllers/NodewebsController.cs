using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Models;

namespace SchemaStar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodewebsController : ControllerBase
    {
        private readonly SchemastarContext _context;

        public NodewebsController(SchemastarContext context)
        {
            _context = context;
        }

        // GET: api/Nodewebs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NodewebResponseDTO>>> GetNodewebs()
        {
            return await _context.Nodewebs.Select(n => new NodewebResponseDTO
            { 
                PublicId = new Guid (n.PublicId),
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
            byte[] publicIdBytes = publicId.ToByteArray();

            var nodeweb = await _context.Nodewebs
                .Where(n => n.PublicId == publicIdBytes)
                .Select(n => new NodewebResponseDTO
                {
                    PublicId = new Guid(n.PublicId),
                    NodeWebName = n.NodeWebName,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    LastLayoutAt = n.LastLayoutAt,
                }).FirstOrDefaultAsync();

            if (nodeweb == null)
            {
                return NotFound();
            }

            return nodeweb;
        }

        // PUT: api/Nodewebs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{publicId}")]
        public async Task<IActionResult> UpdateNodeweb(Guid publicId, NodewebRequestDTO request)
        {
            byte[] publicIdBytes = publicId.ToByteArray();
            var nodeweb = await _context.Nodewebs.FirstOrDefaultAsync(n => n.PublicId == publicIdBytes);
           
            if (nodeweb == null)
            {
                return NotFound();
            }

            //Check for duplicate nodeweb names per the user
            bool isDuplicate = await _context.Nodewebs.AnyAsync(n =>
             n.UserId == nodeweb.UserId &&
             n.NodeWebName == request.NodeWebName && //check if request name matches existing one
             n.PublicId != publicIdBytes
            );

            if (isDuplicate) { return BadRequest("Nodeweb name already exist, duplicate found."); }

            //Update the name
            nodeweb.NodeWebName = request.NodeWebName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Record was updated elsewhere [database]");
            }
            catch (DbUpdateException) 
            {
                return BadRequest("Unable to save, make sure node web name is unqiue.");
            }

            return NoContent();
        }

        // POST: api/Nodewebs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NodewebResponseDTO>> PostNodeweb(NodewebRequestDTO request)
        {
            ulong currentUserId = 0; //Implement logic for getting authenticated user

            //Check if nodeweb with the request name already created for the user
            bool isDuplicate = await _context.Nodewebs.
                AnyAsync(n => n.UserId == currentUserId && n.NodeWebName == request.NodeWebName);

            if (isDuplicate) { return Conflict(new { message = $"A NodeWeb name '{request.NodeWebName} already exists for this user.'" }); }

            var nodeweb = new Nodeweb
            {
                UserId = currentUserId,
                NodeWebName = request.NodeWebName,
            };

            _context.Nodewebs.Add(nodeweb);

            return CreatedAtAction("GetNodeweb", new { id = nodeweb.Id }, nodeweb);
        }

        // DELETE: api/Nodewebs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNodeweb(ulong id)
        {
            var nodeweb = await _context.Nodewebs.FindAsync(id);
            if (nodeweb == null)
            {
                return NotFound();
            }

            _context.Nodewebs.Remove(nodeweb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NodewebExists(ulong id)
        {
            return _context.Nodewebs.Any(e => e.Id == id);
        }
    }
}
