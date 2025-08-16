using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseTrackerDbContext _context;

        public ExpenseController(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetExpense")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpense()
        {
            return await _context.Expense.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(Guid id)
        {
            var expense = await _context.Expense.FindAsync(id);
            if (expense == null)
                return NotFound();

            return expense;
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense(Expense expense)
        {
            expense.id = Guid.NewGuid();
            expense.created_at = DateTime.UtcNow;
            expense.updated_at = DateTime.UtcNow;

            _context.Expense.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpense), new { id = expense.id }, expense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(Guid id, Expense expense)
        {
            if (id != expense.id)
                return BadRequest();

            expense.updated_at = DateTime.UtcNow;
            _context.Entry(expense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Expense.Any(expense => expense.id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            var expense = await _context.Expense.FindAsync(id);
            if (expense == null)
                return NotFound();

            _context.Expense.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}