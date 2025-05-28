using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blogic_crm_back.Models;

namespace blogic_crm_back.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstitutionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstitutionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Institutions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Institution>>> GetInstitutions()
        {
            return await _context.Institutions
                .Include(i => i.Contracts)
                    .ThenInclude(c => c.Users)
                        .ThenInclude(cu => cu.User)
                            .ThenInclude(u => u.Role)
                .ToListAsync();
        }

        // GET: api/Institutions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Institution>> GetInstitution(int id)
        {
            var institution = await _context.Institutions
                .Include(i => i.Contracts)
                    .ThenInclude(c => c.Users)
                        .ThenInclude(cu => cu.User)
                            .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (institution == null)
            {
                return NotFound();
            }

            return institution;
        }

        // PUT: api/Institutions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstitution(int id, Institution institution)
        {
            if (id != institution.Id)
            {
                return BadRequest();
            }

            _context.Entry(institution).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstitutionExists(id))
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

        // POST: api/Institutions
        [HttpPost]
        public async Task<ActionResult<Institution>> PostInstitution(Institution institution)
        {
            _context.Institutions.Add(institution);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstitution), new { id = institution.Id }, institution);
        }

        // DELETE: api/Institutions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstitution(int id)
        {
            var institution = await _context.Institutions.FindAsync(id);
            if (institution == null)
            {
                return NotFound();
            }

            _context.Institutions.Remove(institution);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InstitutionExists(int id)
        {
            return _context.Institutions.Any(e => e.Id == id);
        }
    }
}
