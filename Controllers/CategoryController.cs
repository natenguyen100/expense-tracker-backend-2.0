using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ExpenseTrackerDbContext _context;

        public CategoryController(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCategory")]
        public async Task<IActionResult> GetCategory()
        {
            var categories = await _context.Category.ToListAsync();
            return Ok(categories);
        }
    }
}