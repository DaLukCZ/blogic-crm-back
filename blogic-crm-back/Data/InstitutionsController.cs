using blogic_crm_back.Dto;
using blogic_crm_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<InstitutionDto>>> GetInstitutions()
        {
            var institutions = await _context.Institutions
                .Include(i => i.Contracts)
                .ThenInclude(c => c.Users)
                .ThenInclude(cu => cu.User)
                .ThenInclude(u => u.Role)
                .ToListAsync();

            var dtoList = institutions.Select(i => new InstitutionDto
            {
                Id = i.Id,
                Name = i.Name,
                Contracts = i.Contracts.Select(c => new ContractDto
                {
                    Id = c.Id,
                    ReferenceNumber = c.ReferenceNumber,
                    Users = c.Users.Select(cu => new UserDto
                    {
                        Id = cu.User.Id,
                        FirstName = cu.User.FirstName,
                        LastName = cu.User.LastName,
                        RoleName = cu.User.Role?.Name ?? string.Empty
                    }).ToList()
                }).ToList()
            }).ToList();

            return dtoList;
        }

        // GET: api/Institutions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InstitutionDto>> GetInstitution(int id)
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

            var dto = new InstitutionDto
            {
                Id = institution.Id,
                Name = institution.Name,
                Contracts = institution.Contracts.Select(c => new ContractDto
                {
                    Id = c.Id,
                    ReferenceNumber = c.ReferenceNumber,
                    Users = c.Users.Select(cu => new UserDto
                    {
                        Id = cu.User.Id,
                        FirstName = cu.User.FirstName,
                        LastName = cu.User.LastName,
                        RoleName = cu.User.Role?.Name ?? string.Empty
                    }).ToList()
                }).ToList()
            };

            return dto;
        }

        private bool InstitutionExists(int id)
        {
            return _context.Institutions.Any(e => e.Id == id);
        }
    }
}
