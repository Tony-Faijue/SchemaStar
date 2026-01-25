using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<Nodeweb>>> GetNodewebs()
        {
            return await _context.Nodewebs.ToListAsync();
        }

        // GET: api/Nodewebs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Nodeweb>> GetNodeweb(ulong id)
        {
            var nodeweb = await _context.Nodewebs.FindAsync(id);

            if (nodeweb == null)
            {
                return NotFound();
            }

            return nodeweb;
        }

        // PUT: api/Nodewebs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNodeweb(ulong id, Nodeweb nodeweb)
        {
            if (id != nodeweb.Id)
            {
                return BadRequest();
            }

            _context.Entry(nodeweb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NodewebExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Nodewebs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Nodeweb>> PostNodeweb(Nodeweb nodeweb)
        {
            _context.Nodewebs.Add(nodeweb);
            await _context.SaveChangesAsync();

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
