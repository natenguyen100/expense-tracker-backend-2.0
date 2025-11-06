namespace JWTAuth.Models
{
    public class UserResponseDto
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public decimal? TotalIncome { get; set; }
    }
}