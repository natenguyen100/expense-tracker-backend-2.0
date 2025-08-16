using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Models
{
    public class ExpenseTrackerDbContext : DbContext
    {
        public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options)
            : base(options) { }

        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>().ToTable("user_account");
            modelBuilder.Entity<Budget>().ToTable("budget");
            modelBuilder.Entity<Expense>().ToTable("expense");
            modelBuilder.Entity<Category>().ToTable("category");
        }
    }
}