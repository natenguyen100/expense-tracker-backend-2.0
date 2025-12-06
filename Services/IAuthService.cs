using JWTAuth.Entities;
using JWTAuth.Models;
using ExpenseTrackerAPI.Models;

namespace JWTAuth.Services
{
    public interface IAuthService
    {
        Task<UserAccount?> SignUpAsync(UserDto request);
        Task<UserResponseDto?> SignInAsync(UserDto request, HttpResponse response);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<bool> SignOutAsync(SignOutRequestDto request);
    }
}