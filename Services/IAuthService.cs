using JWTAuth.Entities;
using JWTAuth.Models;
using ExpenseTrackerAPI.Models;

namespace JWTAuth.Services
{
    public interface IAuthService
    {
        Task<UserAccount?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<bool> LogoutAsync(LogoutRequestDto request);
    }
}