using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ExpenseTrackerAPI.Extensions;


namespace ExpenseTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpenseController(AppDbContext context)
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

        [HttpGet("GetExpenses")]
        public async Task<ActionResult<IEnumerable<object>>> GetExpense()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var expenses = await _context.Expense
                    .Include(e => e.Category)
                    .Where(expense => expense.user_id == userId)
                    .OrderByDescending(expense => expense.expense_date)
                    .Select(expense => new
                    {
                        expense.id,
                        expense.user_id,
                        expense.category_id,
                        category_name = expense.category_id != null ? expense.Category.name : null,
                        expense.name,
                        expense.amount,
                        expense.currency,
                        expense.description,
                        expense.expense_date,
                        expense.payment_method,
                        expense.receipt_url,
                        expense.is_recurring,
                        expense.recurring_frequency,
                        expense.recurring_end_date,
                        expense.created_at,
                        expense.updated_at
                    })
                    .ToListAsync();

                return Ok(expenses);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var expense = await _context.Expense
                    .FirstOrDefaultAsync(e => e.id == id && e.user_id == userId);
                    
                if (expense == null)
                    return NotFound();

                return expense;
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense(Expense expense)
        {
            try
            {
                expense.user_id = GetCurrentUserId();
                expense.id = Guid.NewGuid();
                expense.created_at = DateTime.UtcNow;
                expense.updated_at = DateTime.UtcNow;
                
                expense.expense_date = expense.expense_date.AsUtc();
                expense.recurring_end_date = expense.recurring_end_date.AsUtc();

                _context.Expense.Add(expense);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetExpense), new { id = expense.id }, expense);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
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
        public async Task<IActionResult> UpdateExpense(Guid id, Expense expense)
        {
            if (id != expense.id)
                return BadRequest("ID mismatch");

            try
            {
                var userId = GetCurrentUserId();
                
                var existingExpense = await _context.Expense
                    .FirstOrDefaultAsync(e => e.id == id && e.user_id == userId);
                    
                if (existingExpense == null)
                    return NotFound();

                existingExpense.name = expense.name;
                existingExpense.amount = expense.amount;
                existingExpense.currency = expense.currency;
                existingExpense.description = expense.description;
                existingExpense.expense_date = expense.expense_date.AsUtc();
                existingExpense.payment_method = expense.payment_method;
                existingExpense.receipt_url = expense.receipt_url;
                existingExpense.is_recurring = expense.is_recurring;
                existingExpense.recurring_frequency = expense.recurring_frequency;
                existingExpense.recurring_end_date = expense.recurring_end_date.AsUtc();
                existingExpense.category_id = expense.category_id;
                existingExpense.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Expense.Any(e => e.id == id))
                    return NotFound();
                else
                    throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var expense = await _context.Expense
                    .FirstOrDefaultAsync(e => e.id == id && e.user_id == userId);
                    
                if (expense == null)
                    return NotFound();

                _context.Expense.Remove(expense);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user authentication");
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = $"Error deleting expense: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }
                return BadRequest(errorMessage);
            }
        }
    }
}