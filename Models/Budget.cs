using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackerAPI.Models
{
    public class Budget
    {
        public Guid id { get; set; }
        public Guid user_id { get; set; }
        public Guid? category_id { get; set; }

        [ForeignKey("category_id")]
        public required string name { get; set; }
        public decimal amount { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}