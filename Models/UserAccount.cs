namespace ExpenseTrackerAPI.Models
{
    public class UserAccount
    {
        public Guid id { get; set; }
        public required string first_name { get; set; }
        public required string last_name { get; set; }
        public required string email { get; set; }
        public required string password_hashed { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}