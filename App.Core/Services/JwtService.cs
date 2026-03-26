using App.Core.Domain.IdentityEntities;
using App.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace App.Core.Services
{
    /// <summary>
    /// JWT token generation service.
    /// Follows Single Responsibility Principle - only handles JWT logic.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =========================================================
        // PUBLIC METHODS
        // =========================================================

        /// <summary>
        /// Generate a signed JWT access token for the given user and roles.
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
                expires: GetAccessTokenExpiration(),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate a cryptographically secure random refresh token.
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Get access token expiration time in minutes from configuration.
        /// </summary>
        public int GetTokenExpirationMinutes()
        {
            return Convert.ToInt32(_configuration["Jwt:ExpirationMinutes"]);
        }

        /// <summary>
        /// Get refresh token expiration time in days from configuration.
        /// </summary>
        public int GetRefreshTokenExpirationDays()
        {
            return Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"]);
        }

        // =========================================================
        // PRIVATE HELPERS
        // =========================================================

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

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return claims.ToArray();
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        private DateTime GetAccessTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes());
        }
    }
}