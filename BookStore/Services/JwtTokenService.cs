using BookStore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookStore.Services
{
    public class JwtTokenService(IOptions<JwtOptions> jwtOptions, UserManager<Users> userManager)
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var credentials = new SigningCredentials(_jwtOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: _jwtOptions.ExpiryDate,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateUserToken(Users user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                // Important: Use ClaimTypes.Role so that ASP.NET recognizes it for [Authorize(Roles = "User")]
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return GenerateToken(claims);
        }
    };



}
