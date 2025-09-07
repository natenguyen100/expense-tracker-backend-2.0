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
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("Username already exists.");
    
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }

         [Authorize]
         [HttpPost("logout")]
         public async Task<IActionResult> Logout(LogoutRequestDto request)
         {
            var result = await authService.LogoutAsync(request);
            if (!result)
                return BadRequest("Logout failed or invalid refresh token.");
            return Ok(new { Message = "Successfully logged out." });
         }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");
            
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }
    }
}