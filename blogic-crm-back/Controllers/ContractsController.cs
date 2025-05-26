using blogic_crm_back.Data;
using blogic_crm_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace blogic_crm_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        private string GetCurrentUserRole() =>
            User.Claims.First(c => c.Type == ClaimTypes.Role).Value;

        // GET: api/Contracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
            [FromQuery] string? institution = null)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var query = _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Manager)
                .AsQueryable();

            if (role == "client")
            {
                query = query.Where(c => c.ClientId == userId);
            }

            if (!string.IsNullOrWhiteSpace(institution))
            {
                query = query.Where(c => c.Institution.Contains(institution));
            }

            return await query.ToListAsync();
        }

        // GET: api/Contracts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Manager)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null) return NotFound();

            var role = GetCurrentUserRole();
            var userId = GetCurrentUserId();

            if (role == "client" && contract.ClientId != userId)
                return Forbid();

            return contract;
        }

        // PUT: api/Contracts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, Contract contract)
        {
            if (id != contract.Id) return BadRequest();

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var existing = await _context.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null) return NotFound();

            if (role == "advisor" && existing.ManagerId != userId)
                return Forbid();

            if (role == "client")
                return Forbid();

            _context.Entry(contract).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Contracts
        [HttpPost]
        [Authorize(Roles = "advisor,admin")]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        // DELETE: api/Contracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            var role = GetCurrentUserRole();
            var userId = GetCurrentUserId();

            if (role == "advisor" && contract.ManagerId != userId)
                return Forbid();

            if (role == "client")
                return Forbid();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContractExists(int id) =>
            _context.Contracts.Any(e => e.Id == id);
    }
}
