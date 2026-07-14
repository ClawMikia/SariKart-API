using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SariKartAPIV2.Entities;

namespace SariKartAPIV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreBranchesController : ControllerBase
    {
        private readonly SariKartContext _context;

        public StoreBranchesController(SariKartContext context)
        {
            _context = context;
        }

        // GET: api/StoreBranches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreBranch>>> GetStoreBranches()
        {
            return await _context.StoreBranches.ToListAsync();
        }

        // GET: api/StoreBranches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoreBranch>> GetStoreBranch(int id)
        {
            var storeBranch = await _context.StoreBranches.FindAsync(id);

            if (storeBranch == null)
            {
                return NotFound();
            }

            return storeBranch;
        }

        // PUT: api/StoreBranches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStoreBranch(int id, StoreBranch storeBranch)
        {
            if (id != storeBranch.Id)
            {
                return BadRequest();
            }

            _context.Entry(storeBranch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreBranchExists(id))
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

        // POST: api/StoreBranches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StoreBranch>> PostStoreBranch(StoreBranch storeBranch)
        {
            _context.StoreBranches.Add(storeBranch);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStoreBranch", new { id = storeBranch.Id }, storeBranch);
        }

        // DELETE: api/StoreBranches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreBranch(int id)
        {
            var storeBranch = await _context.StoreBranches.FindAsync(id);
            if (storeBranch == null)
            {
                return NotFound();
            }

            _context.StoreBranches.Remove(storeBranch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoreBranchExists(int id)
        {
            return _context.StoreBranches.Any(e => e.Id == id);
        }
    }
}
