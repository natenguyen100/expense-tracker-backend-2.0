using ExpenseTrackerAPI.Models;
using JWTAuth.Entities;
using JWTAuth.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.Services
{
    public class AuthServices(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<UserAccount?> RegisterAsync(UserDto request)
        {

            if (await context.Users.AnyAsync(user => user.email == request.email))
            {
                return null;
            }

            var user = new UserAccount()
            {
                id = Guid.NewGuid(),
                email = request.email,
                first_name = "",
                last_name = "",
                password_hashed = "",
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            var hashedPassword = new PasswordHasher<UserAccount>()
                .HashPassword(user, request.password);

            user.password_hashed = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<String?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.email == request.email);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<UserAccount>().VerifyHashedPassword(user, user.password_hashed, request.password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateToken(user);
        }

        private string CreateToken(UserAccount user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.email),
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
                };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}