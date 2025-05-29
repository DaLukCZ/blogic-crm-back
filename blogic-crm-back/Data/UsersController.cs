using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blogic_crm_back.Models;
using blogic_crm_back.Dto;
using System.Security.Claims;

namespace blogic_crm_back.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserIdStr == null || !int.TryParse(currentUserIdStr, out var currentUserId))
            {
                return Unauthorized("Chyba autentizace. Token není platný.");
            }

            if (currentUserId != id)
            {
                return Forbid("Nemáte oprávnění měnit heslo tohoto uživatele.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Uživatel nebyl nalezen.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Uživatelské jméno už existuje.");

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("E-mail už existuje.");

            if (await _context.Users.AnyAsync(u => u.Number == user.Number && u.CountryCode == user.CountryCode))
                return BadRequest("Telefonní číslo už existuje.");

            if (await _context.Users.AnyAsync(u => u.SSN == user.SSN))
                return BadRequest("Rodné číslo už existuje.");

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // GET: api/Users/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportUsers()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();

            var csv = new StringBuilder();

            csv.AppendLine("Id,Username,FirstName,LastName,Email,Phone,SSN,Age,Role");

            foreach (var u in users)
            {
                string age = "-";
                if (!string.IsNullOrEmpty(u.SSN) && u.SSN.Length >= 6)
                {
                    age = CalculateAgeFromSSN(u.SSN).ToString();
                }
                var phone = $"{u.CountryCode} {u.Number}".Trim();

                csv.AppendLine($"{u.Id}," +
                               $"{EscapeCsv(u.Username)}," +
                               $"{EscapeCsv(u.FirstName)}," +
                               $"{EscapeCsv(u.LastName)}," +
                               $"{EscapeCsv(u.Email)}," +
                               $"{EscapeCsv(phone)}," +
                               $"{EscapeCsv(u.SSN)}," +
                               $"{EscapeCsv(age)}," +
                               $"{EscapeCsv(u.Role?.Name ?? "")}");
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

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }


        private int CalculateAgeFromSSN(string ssn)
        {
            try
            {
                var clean = ssn.Replace("/", "");
                int year = int.Parse(clean.Substring(0, 2));
                int month = int.Parse(clean.Substring(2, 2));
                int day = int.Parse(clean.Substring(4, 2));

                if (month > 50) month -= 50;

                int fullYear = year + (year < 50 ? 2000 : 1900);
                var birthDate = new DateTime(fullYear, month, day);
                var today = DateTime.Today;

                int age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age)) age--;

                return age;
            }
            catch
            {
                return -1;
            }
        }
    }
}
