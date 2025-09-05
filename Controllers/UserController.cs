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
                    password_hashed = user.password_hashed
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
                password_hashed = user.password_hashed
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
    }
}
