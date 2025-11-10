using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _context.Users
            .Where(user => user.id == id)
            .Select(user => new
            {
                id = user.id,
                first_name = user.first_name,
                last_name = user.last_name,
                email = user.email,
                password_hashed = user.password_hashed,
                total_income = user.total_income
            })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _context.Users
                .Select(user => new
                {
                    id = user.id,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    email = user.email,
                    password_hashed = user.password_hashed,
                    total_income = user.total_income
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetUser/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            var user = await _context.Users
            .Where(user => user.email == email)
            .Select(user => new
            {
                id = user.id,
                first_name = user.first_name,
                last_name = user.last_name,
                email = user.email,
                password_hashed = user.password_hashed,
                total_income = user.total_income
            })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserAccount user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPatch("UpdateUserIncome/{email}")]
        public async Task<IActionResult> UpdateUserIncome(string email, [FromBody] decimal newIncome)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            if (newIncome < 0)
            {
                return BadRequest(new { message = "Income cannot be negative" });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email.ToLower() == email.ToLower());

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.total_income = newIncome;
            user.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = new
            {
                id = user.id,
                first_name = user.first_name,
                last_name = user.last_name,
                email = user.email,
                password_hashed = user.password_hashed,
                total_income = user.total_income
            };

            return Ok(result);
        }
                
    }
}
