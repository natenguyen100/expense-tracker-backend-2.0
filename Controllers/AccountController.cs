using JWTAuth.Entities;
using Microsoft.AspNetCore.Mvc;
using JWTAuth.Services;
using Microsoft.AspNetCore.Authorization;

namespace JWTAuth.Models.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAuthService authService) : ControllerBase
    {
        [HttpPost("signup")]
        public async Task<ActionResult<User>> SignUp(UserDto request)
        {
            var user = await authService.SignUpAsync(request);
            if (user is null)
                return BadRequest("Username already exists.");
    
            return Ok(user);
        }

        [HttpPost("signin")]
        public async Task<ActionResult<UserResponseDto>> SignIn(UserDto request)
        {
            var result = await authService.SignInAsync(request, Response);
            if (result is null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }

        [HttpPost("signout")]
         public async new Task<IActionResult> SignOut()
         {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("No refresh token found.");

            var result = await authService.SignOutAsync(new SignOutRequestDto
            {
                RefreshToken = refreshToken
            });

            if (!result)
                return BadRequest("Logout failed or invalid refresh token.");

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new { Message = "Successfully logged out." });
         }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token found.");
            
            var result = await authService.RefreshTokensAsync(new RefreshTokenRequestDto
            {
              RefreshToken = refreshToken,
            });

            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");
            
            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
            {
              HttpOnly = true,
              Secure = false, // Set to true in production with HTTPS
              SameSite = SameSiteMode.Lax, // Changed from Strict to Lax for cross-origin requests
              Expires = DateTimeOffset.UtcNow.AddMinutes(24)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
              HttpOnly = true,
              Secure = false, // Set to true in production with HTTPS
              SameSite = SameSiteMode.Lax, // Changed from Strict to Lax for cross-origin requests
              Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(result);
        }
    }
}