using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Category.ToListAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(Guid id)
        {
            try
            {
                var category = await _context.Category.FindAsync(id);

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the category", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            try
            {
                category.id = Guid.NewGuid();
                category.created_at = DateTime.UtcNow;
                _context.Category.Add(category);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCategory), new { id = category.id }, category);
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = $"Error creating expense: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }
                return BadRequest(errorMessage);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, Category category)
        {
            try
            {
                var existingCategory = await _context.Category.FindAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new { message = "Category not found" });
                }
                existingCategory.name = category.name;
                existingCategory.updated_at = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = $"Error updating category: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }
                return BadRequest(errorMessage);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var category = await _context.Category.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                _context.Category.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = $"Error deleting category: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }
                return BadRequest(errorMessage);
            }
        }

    }
}