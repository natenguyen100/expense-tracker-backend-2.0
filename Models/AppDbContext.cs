using Microsoft.EntityFrameworkCore;
using JWTAuth.Models;

namespace ExpenseTrackerAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Budget> Budget { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<UserAccount> Users {get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>().ToTable("user_account");
            modelBuilder.Entity<Budget>().ToTable("budget");
            modelBuilder.Entity<Expense>().ToTable("expense");
            modelBuilder.Entity<Category>().ToTable("category");

            modelBuilder.Entity<Category>().HasData(
                new { id = Guid.Parse("11111111-1111-1111-1111-111111111111"), name = "Housing", created_at = DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime() },
                new { id = Guid.Parse("22222222-2222-2222-2222-222222222222"), name = "Rent", created_at = DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime() },
                new { id = Guid.Parse("33333333-3333-3333-3333-333333333333"), name = "Savings", created_at = DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime() },
                new { id = Guid.Parse("44444444-4444-4444-4444-444444444444"), name = "Transportation", created_at = DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime() },
                new { id = Guid.Parse("55555555-5555-5555-5555-555555555555"), name = "Shopping", created_at = DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime() },
                new { id = Guid.Parse("66666666-6666-6666-6666-666666666666"), name = "Utilities", created_at = DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime() }
            );
        }
    }
}