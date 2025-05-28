using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blogic_crm_back.Models;
using blogic_crm_back.Dto;

namespace blogic_crm_back.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Contracts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContractDto>>> GetContracts()
    {
        var contracts = await _context.Contracts
            .Include(c => c.Institution)
            .Include(c => c.Users)
                .ThenInclude(cu => cu.User)
                    .ThenInclude(u => u.Role)
            .ToListAsync();

        var contractDtos = contracts.Select(c => new ContractDto
        {
            Id = c.Id,
            ReferenceNumber = c.ReferenceNumber,
            InstitutionName = c.Institution?.Name,
            DateSigned = c.DateSigned,
            DateValidFrom = c.DateValidFrom,
            DateValidTo = c.DateValidTo,
            Users = c.Users.Select(cu => new UserDto
            {
                Id = cu.User.Id,
                Username = cu.User.Username,
                FirstName = cu.User.FirstName,
                LastName = cu.User.LastName,
                RoleName = cu.User.Role?.Name
            }).ToList()
        }).ToList();

        return contractDtos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContractDto>> GetContract(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Institution)
            .Include(c => c.Users)
                .ThenInclude(cu => cu.User)
                    .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
            return NotFound();

        var contractDto = new ContractDto
        {
            Id = contract.Id,
            ReferenceNumber = contract.ReferenceNumber,
            InstitutionName = contract.Institution?.Name,
            DateSigned = contract.DateSigned,
            DateValidFrom = contract.DateValidFrom,
            DateValidTo = contract.DateValidTo,
            Users = contract.Users.Select(cu => new UserDto
            {
                Id = cu.User.Id,
                Username = cu.User.Username,
                FirstName = cu.User.FirstName,
                LastName = cu.User.LastName,
                RoleName = cu.User.Role?.Name
            }).ToList()
        };

        return contractDto;
    }



    // PUT: api/Contracts/5
    [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, Contract contract)
        {
            if (id != contract.Id)
            {
                return BadRequest();
            }

            _context.Entry(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
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

        // POST: api/Contracts
        [HttpPost]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {
            // Uložit samotnou smlouvu
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            // Přidat vazby na uživatele (klienti i poradci)
            foreach (var userId in contract.UserIds.Distinct())
            {
                _context.ContractUser.Add(new ContractUser
                {
                    ContractId = contract.Id,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        // DELETE: api/Contracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
