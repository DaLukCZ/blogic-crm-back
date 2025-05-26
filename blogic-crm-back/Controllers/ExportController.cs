using blogic_crm_back.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

namespace blogic_crm_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public ExportController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: /api/export/contracts
        [HttpGet("contracts")]
        public async Task<IActionResult> ExportContracts()
        {
            var contracts = await db.Contracts
                .Include(c => c.Client)
                .Include(c => c.Manager)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("ID,Reference Number,Institution,Client,Manager");

            foreach (var contract in contracts)
            {
                csv.AppendLine($"{contract.Id},{contract.ReferenceNumber},{contract.Institution},{contract.Client?.FirstName} {contract.Client?.LastName},{contract.Manager?.FirstName} {contract.Manager?.LastName}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "contracts.csv");
        }

        // GET: /api/export/users?role=client
        [HttpGet("users")]
        public async Task<IActionResult> ExportUsers([FromQuery] string role)
        {
            var users = await db.Users
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.Name == role)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("ID,Username,FirstName,LastName,Email,Role");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.Id},{user.Username},{user.FirstName},{user.LastName},{user.Email},{user.Role?.Name}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"users-{role}.csv");
        }
    }
}
