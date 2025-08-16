namespace ExpenseTrackerAPI.Models
{
    public class Expense
    {
        public Guid id { get; set; }
        public Guid user_id { get; set; }
        public Guid? category_id { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; } = "USD";
        public string? description { get; set; }
        public DateTime expense_date { get; set; }
        public string? payment_method { get; set; }
        public string? receipt_url { get; set; }
        public bool is_recurring { get; set; } = false;
        public string? recurring_frequency { get; set; }
        public DateTime? recurring_end_date { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}