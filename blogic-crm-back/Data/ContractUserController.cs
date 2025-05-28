using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blogic_crm_back.Models;

namespace blogic_crm_back.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ContractUser
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractUser>>> GetContractUsers()
        {
            return await _context.ContractUser
                .Include(cu => cu.User)
                .Include(cu => cu.Contract)
                .ToListAsync();
        }

        // GET: api/ContractUser/5/8
        [HttpGet("{contractId}/{userId}")]
        public async Task<ActionResult<ContractUser>> GetContractUser(int contractId, int userId)
        {
            var contractUser = await _context.ContractUser
                .Include(cu => cu.User)
                .Include(cu => cu.Contract)
                .FirstOrDefaultAsync(cu => cu.ContractId == contractId && cu.UserId == userId);

            if (contractUser == null)
            {
                return NotFound();
            }

            return contractUser;
        }

        // POST: api/ContractUser
        [HttpPost]
        public async Task<ActionResult<ContractUser>> PostContractUser(ContractUser contractUser)
        {
            _context.ContractUser.Add(contractUser);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ContractUserExists(contractUser.ContractId, contractUser.UserId))
                {
                    return Conflict("This advisor is already assigned to the contract.");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetContractUser), new { contractId = contractUser.ContractId, userId = contractUser.UserId }, contractUser);
        }

        // DELETE: api/ContractUser/5/8
        [HttpDelete("{contractId}/{userId}")]
        public async Task<IActionResult> DeleteContractUser(int contractId, int userId)
        {
            var contractUser = await _context.ContractUser
                .FirstOrDefaultAsync(cu => cu.ContractId == contractId && cu.UserId == userId);

            if (contractUser == null)
            {
                return NotFound();
            }

            _context.ContractUser.Remove(contractUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContractUserExists(int contractId, int userId)
        {
            return _context.ContractUser.Any(e => e.ContractId == contractId && e.UserId == userId);
        }
    }
}
