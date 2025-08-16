namespace ExpenseTrackerAPI.Models
{
    public class Category
    {
        public Guid id { get; set; }
        public required string name { get; set; }
        public DateTime created_at { get; set; }
    }
}