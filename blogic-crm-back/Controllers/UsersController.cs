using blogic_crm_back.Data;
using blogic_crm_back.Models;
using blogic_crm_back.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace blogic_crm_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/users?role=advisor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] string? role)
        {
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role != null && u.Role.Name == role);
            }

            return await query.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return user;
        }

        // POST: api/Users
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> RegisterUser(RegisterUserRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CountryCode = request.CountryCode,
                Number = request.PhoneNumber,
                SSN = request.SSN,
                RoleId = request.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                throw;
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id) => _context.Users.Any(e => e.Id == id);
    }
}
