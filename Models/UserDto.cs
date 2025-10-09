namespace JWTAuth.Models
{
    public class UserDto
    {
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
    }
}