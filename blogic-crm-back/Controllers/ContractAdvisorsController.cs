using blogic_crm_back.Data;
using blogic_crm_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blogic_crm_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractAdvisorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractAdvisorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ContractAdvisors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractAdvisor>>> GetContractAdvisors()
        {
            return await _context.ContractAdvisors.ToListAsync();
        }

        // GET: api/ContractAdvisors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractAdvisor>> GetContractAdvisor(int id)
        {
            var contractAdvisor = await _context.ContractAdvisors.FindAsync(id);

            if (contractAdvisor == null)
            {
                return NotFound();
            }

            return contractAdvisor;
        }

        // PUT: api/ContractAdvisors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContractAdvisor(int id, ContractAdvisor contractAdvisor)
        {
            if (id != contractAdvisor.ContractId)
            {
                return BadRequest();
            }

            _context.Entry(contractAdvisor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractAdvisorExists(id))
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

        // POST: api/ContractAdvisors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContractAdvisor>> PostContractAdvisor(ContractAdvisor contractAdvisor)
        {
            _context.ContractAdvisors.Add(contractAdvisor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ContractAdvisorExists(contractAdvisor.ContractId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetContractAdvisor", new { id = contractAdvisor.ContractId }, contractAdvisor);
        }

        // DELETE: api/ContractAdvisors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContractAdvisor(int id)
        {
            var contractAdvisor = await _context.ContractAdvisors.FindAsync(id);
            if (contractAdvisor == null)
            {
                return NotFound();
            }

            _context.ContractAdvisors.Remove(contractAdvisor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContractAdvisorExists(int id)
        {
            return _context.ContractAdvisors.Any(e => e.ContractId == id);
        }
    }
}
