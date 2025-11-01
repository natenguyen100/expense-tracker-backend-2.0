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

        [HttpGet("GetExpense")]
        public async Task<ActionResult<IEnumerable<object>>> GetExpense()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var expenses = await _context.Expense
                    .Where(e => e.user_id == userId)
                    .OrderByDescending(e => e.expense_date)
                    .Select(e => new
                    {
                        id = e.id,
                        userId = e.user_id,
                        categoryId = e.category_id,
                        category = e.category_id.HasValue 
                            ? _context.Category.Where(c => c.id == e.category_id.Value).Select(c => c.name).FirstOrDefault()
                            : null,
                        name = e.name,
                        amount = e.amount,
                        currency = e.currency,
                        description = e.description,
                        expenseDate = e.expense_date,
                        paymentMethod = e.payment_method,
                        receiptUrl = e.receipt_url,
                        isRecurring = e.is_recurring,
                        recurringFrequency = e.recurring_frequency,
                        recurringEndDate = e.recurring_end_date,
                        createdAt = e.created_at,
                        updatedAt = e.updated_at
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
        public async Task<ActionResult<object>> GetExpense(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var expense = await _context.Expense
                    .Where(e => e.id == id && e.user_id == userId)
                    .Select(e => new
                    {
                        id = e.id,
                        userId = e.user_id,
                        categoryId = e.category_id,
                        category = e.category_id.HasValue 
                            ? _context.Category.Where(c => c.id == e.category_id.Value).Select(c => c.name).FirstOrDefault()
                            : null,
                        name = e.name,
                        amount = e.amount,
                        currency = e.currency,
                        description = e.description,
                        expenseDate = e.expense_date,
                        paymentMethod = e.payment_method,
                        receiptUrl = e.receipt_url,
                        isRecurring = e.is_recurring,
                        recurringFrequency = e.recurring_frequency,
                        recurringEndDate = e.recurring_end_date,
                        createdAt = e.created_at,
                        updatedAt = e.updated_at
                    })
                    .FirstOrDefaultAsync();
                    
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
                
                if (expense.expense_date.Kind == DateTimeKind.Unspecified)
                {
                    expense.expense_date = DateTime.SpecifyKind(expense.expense_date, DateTimeKind.Utc);
                }
                
                if (expense.recurring_end_date.HasValue && expense.recurring_end_date.Value.Kind == DateTimeKind.Unspecified)
                {
                    expense.recurring_end_date = DateTime.SpecifyKind(expense.recurring_end_date.Value, DateTimeKind.Utc);
                }

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
                
                if (expense.expense_date.Kind == DateTimeKind.Unspecified)
                {
                    existingExpense.expense_date = DateTime.SpecifyKind(expense.expense_date, DateTimeKind.Utc);
                }
                else
                {
                    existingExpense.expense_date = expense.expense_date;
                }
                
                existingExpense.payment_method = expense.payment_method;
                existingExpense.receipt_url = expense.receipt_url;
                existingExpense.is_recurring = expense.is_recurring;
                existingExpense.recurring_frequency = expense.recurring_frequency;

                if (expense.recurring_end_date.HasValue && expense.recurring_end_date.Value.Kind == DateTimeKind.Unspecified)
                {
                    existingExpense.recurring_end_date = DateTime.SpecifyKind(expense.recurring_end_date.Value, DateTimeKind.Utc);
                }
                else
                {
                    existingExpense.recurring_end_date = expense.recurring_end_date;
                }
                
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