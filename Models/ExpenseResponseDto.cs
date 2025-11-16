namespace ExpenseTrackerAPI.Models;

public class ExpenseResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Description { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReceiptUrl { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurringFrequency { get; set; }
    public DateTime? RecurringEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
