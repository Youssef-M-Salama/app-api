using App.Core.Domain.IdentityEntities;
using App.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Core.Services
{
    /// <summary>
    /// JWT token generation service
    /// Follows Single Responsibility Principle - only handles JWT logic
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate JWT token for user with roles
        /// </summary>
        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var claims = CreateClaims(user, roles);
            var securityKey = GetSecurityKey();
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: GetExpirationTime(),
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Get token expiration time in minutes
        /// </summary>
        public int GetTokenExpirationMinutes()
        {
            return Convert.ToInt32(_configuration["Jwt:ExpirationMinutes"]);
        }

        #region Private Helper Methods

        private Claim[] CreateClaims(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims.ToArray();
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        private DateTime GetExpirationTime()
        {
            var minutes = GetTokenExpirationMinutes();
            return DateTime.UtcNow.AddMinutes(minutes);
        }

        #endregion
    }
}