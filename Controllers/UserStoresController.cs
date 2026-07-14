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
    public class UserStoresController : ControllerBase
    {
        private readonly SariKartContext _context;

        public UserStoresController(SariKartContext context)
        {
            _context = context;
        }

        // GET: api/UserStores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserStore>>> GetUserStores()
        {
            return await _context.UserStores.ToListAsync();
        }

        // GET: api/UserStores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserStore>> GetUserStore(int id)
        {
            var userStore = await _context.UserStores.FindAsync(id);

            if (userStore == null)
            {
                return NotFound();
            }

            return userStore;
        }

        // PUT: api/UserStores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserStore(int id, UserStore userStore)
        {
            if (id != userStore.Id)
            {
                return BadRequest();
            }

            _context.Entry(userStore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserStoreExists(id))
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

        // POST: api/UserStores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserStore>> PostUserStore(UserStore userStore)
        {
            _context.UserStores.Add(userStore);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserStore", new { id = userStore.Id }, userStore);
        }

        // DELETE: api/UserStores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserStore(int id)
        {
            var userStore = await _context.UserStores.FindAsync(id);
            if (userStore == null)
            {
                return NotFound();
            }

            _context.UserStores.Remove(userStore);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserStoreExists(int id)
        {
            return _context.UserStores.Any(e => e.Id == id);
        }
    }
}
