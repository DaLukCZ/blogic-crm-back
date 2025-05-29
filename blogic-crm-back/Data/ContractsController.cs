using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // GET: api/Contracts/5
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

        [HttpGet("byuser/{userId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsByUserId(int userId)
        {
            var contracts = await _context.Contracts
                .Include(c => c.Institution)
                .Include(c => c.Users)
                    .ThenInclude(cu => cu.User)
                        .ThenInclude(u => u.Role)
                .Where(c => c.Users.Any(cu => cu.UserId == userId))
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

        // PUT: api/Contracts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, Contract contract)
        {
            if (id != contract.Id)
                return BadRequest();

            var existingContract = await _context.Contracts
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingContract == null)
                return NotFound();

            existingContract.ReferenceNumber = contract.ReferenceNumber;
            existingContract.InstitutionId = contract.InstitutionId;
            existingContract.DateSigned = contract.DateSigned;
            existingContract.DateValidFrom = contract.DateValidFrom;
            existingContract.DateValidTo = contract.DateValidTo;

            var newUserIds = contract.UserIds.Distinct().ToList();

            var toRemove = existingContract.Users.Where(cu => !newUserIds.Contains(cu.UserId)).ToList();
            _context.ContractUser.RemoveRange(toRemove);

            var existingUserIds = existingContract.Users.Select(cu => cu.UserId).ToList();
            var toAdd = newUserIds.Where(idUser => !existingUserIds.Contains(idUser))
                                 .Select(idUser => new ContractUser { ContractId = id, UserId = idUser });
            await _context.ContractUser.AddRangeAsync(toAdd);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Contracts
        [HttpPost]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

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

        [HttpPost("AddContractWithInstitution")]
        public async Task<ActionResult<ContractDto>> AddContractWithInstitution(ContractWithInstitutionDto dto)
        {
            var institution = new Institution { Name = dto.InstitutionName };
            _context.Institutions.Add(institution);
            await _context.SaveChangesAsync();

            var contract = new Contract
            {
                ReferenceNumber = dto.ReferenceNumber,
                InstitutionId = institution.Id,
                DateSigned = dto.DateSigned,
                DateValidFrom = dto.DateValidFrom,
                DateValidTo = dto.DateValidTo,
                UserIds = dto.UserIds
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            foreach (var userId in dto.UserIds.Distinct())
            {
                _context.ContractUser.Add(new ContractUser
                {
                    ContractId = contract.Id,
                    UserId = userId
                });
            }
            await _context.SaveChangesAsync();

            var contractDto = new ContractDto
            {
                Id = contract.Id,
                ReferenceNumber = contract.ReferenceNumber,
                InstitutionName = institution.Name,
                DateSigned = contract.DateSigned,
                DateValidFrom = contract.DateValidFrom,
                DateValidTo = contract.DateValidTo,
                Users = (await _context.ContractUser
                    .Where(cu => cu.ContractId == contract.Id)
                    .Include(cu => cu.User)
                    .ThenInclude(u => u.Role)
                    .ToListAsync())
                    .Select(cu => new UserDto
                    {
                        Id = cu.User.Id,
                        Username = cu.User.Username,
                        FirstName = cu.User.FirstName,
                        LastName = cu.User.LastName,
                        RoleName = cu.User.Role?.Name
                    }).ToList()
            };

            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contractDto);
        }

        // DELETE: api/Contracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }


        [HttpGet("export")]
        public async Task<IActionResult> ExportContractsCsv()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Institution)
                .Include(c => c.Users)
                    .ThenInclude(cu => cu.User)
                        .ThenInclude(u => u.Role)
                .ToListAsync();

            var csv = new StringBuilder();

            // CSV Header
            csv.AppendLine("Id,ReferenceNumber,InstitutionName,DateSigned,DateValidFrom,DateValidTo,Users");

            foreach (var c in contracts)
            {
                var usersStr = string.Join("; ", c.Users.Select(cu => $"{EscapeCsv(cu.User.FirstName)} {EscapeCsv(cu.User.LastName)} ({EscapeCsv(cu.User.Role?.Name ?? "")})"));

                csv.AppendLine($"{c.Id}," +
                               $"{EscapeCsv(c.ReferenceNumber)}," +
                               $"{EscapeCsv(c.Institution?.Name ?? "")}," +
                               $"{c.DateSigned:yyyy-MM-dd}," +
                               $"{c.DateValidFrom:yyyy-MM-dd}," +
                               $"{c.DateValidTo:yyyy-MM-dd}," +
                               $"{EscapeCsv(usersStr)}");
            }

            var csvString = csv.ToString();
            var utf8Bom = Encoding.UTF8.GetPreamble();
            var csvBytes = Encoding.UTF8.GetBytes(csvString);

            var csvWithBomBytes = new byte[utf8Bom.Length + csvBytes.Length];
            Buffer.BlockCopy(utf8Bom, 0, csvWithBomBytes, 0, utf8Bom.Length);
            Buffer.BlockCopy(csvBytes, 0, csvWithBomBytes, utf8Bom.Length, csvBytes.Length);

            var base64Csv = Convert.ToBase64String(csvWithBomBytes);

            return Ok(new { base64Csv });
        }


        private string EscapeCsv(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                s = $"\"{s.Replace("\"", "\"\"")}\"";

            return s;
        }
    }
}
