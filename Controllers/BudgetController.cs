using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BudgetController(AppDbContext context)
        {
            _context = context;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim);
        }

        [HttpGet("GetBudget")]
        public async Task<ActionResult<IEnumerable<object>>> GetBudget()
        {
            try
            {
                var userId = GetCurrentUserId();

                var budgets = await _context.Budget
                    .Where(budget => budget.user_id == userId)
                    .OrderBy(budget => budget.created_at)
                    .Select(budget => new
                    {
                        id = budget.id,
                        user_id = budget.user_id,
                        category_id = budget.category_id,
                        name = budget.name,
                        amount = budget.amount,
                        created_at = budget.created_at,
                        updated_at = budget.updated_at,
                        category_name = _context.Category
                            .Where(c => c.id == budget.category_id)
                            .Select(c => c.name)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(budgets);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var budget = await _context.Budget
                    .FirstOrDefaultAsync(b => b.id == id && b.user_id == userId);

                if (budget == null)
                    return NotFound();

                return budget;
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Budget>> CreateBudget(Budget budget)
        {
            try
            {
                budget.user_id = GetCurrentUserId();
                budget.id = Guid.NewGuid();
                budget.created_at = DateTime.UtcNow;
                budget.updated_at = DateTime.UtcNow;

                _context.Budget.Add(budget);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBudget), new { id = budget.id }, budget);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = $"Error creating budget: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }
                return BadRequest(errorMessage);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(Guid id, Budget budget)
        {
            if (id != budget.id)
                return BadRequest("ID mismatch");

            try
            {
                var userId = GetCurrentUserId();

                var existingBudget = await _context.Budget
                    .FirstOrDefaultAsync(b => b.id == id && b.user_id == userId);

                if (existingBudget == null)
                    return NotFound();

                existingBudget.name = budget.name;
                existingBudget.amount = budget.amount;
                existingBudget.category_id = budget.category_id;
                existingBudget.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Budget.Any(b => b.id == id))
                    return NotFound();
                else
                    throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var budget = await _context.Budget
                    .FirstOrDefaultAsync(b => b.id == id && b.user_id == userId);

                if (budget == null)
                    return NotFound();

                _context.Budget.Remove(budget);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = $"Error deleting budget: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }
                return BadRequest(errorMessage);
            }
        }
    }
}