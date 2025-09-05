using JWTAuth.Entities;
using JWTAuth.Models;
using ExpenseTrackerAPI.Models;

namespace JWTAuth.Services
{
    public interface IAuthService
    {
        Task<UserAccount?> RegisterAsync(UserDto request);
        Task<string?> LoginAsync(UserDto request);
    }
}