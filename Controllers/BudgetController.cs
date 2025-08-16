using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly ExpenseTrackerDbContext _context;

        public BudgetController(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetBudget")]
        public async Task<ActionResult<IEnumerable<Budget>>> GetBudget()
        {
            return await _context.Budget.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(Guid id)
        {
            var budget = await _context.Budget.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            return budget;
        }

        [HttpPost]
        public async Task<ActionResult<Budget>> CreateBudget(Budget budget)
        {
            budget.id = Guid.NewGuid();
            budget.created_at = DateTime.UtcNow;
            budget.updated_at = DateTime.UtcNow;

            _context.Budget.Add(budget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBudget), new { id = budget.id }, budget);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(Guid id, Budget budget)
        {
            if (id != budget.id)
            {
                return BadRequest();
            }

            var existingBudget = await _context.Budget.FindAsync(id);
            if (existingBudget == null)
            {
                return NotFound();
            }

            existingBudget.name = budget.name;
            existingBudget.amount = budget.amount;
            existingBudget.updated_at = DateTime.UtcNow;

            _context.Entry(existingBudget).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            var budget = await _context.Budget.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            _context.Budget.Remove(budget);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}