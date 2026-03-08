using App.Core.Domain.IdentityEntities;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service for JWT token generation and validation
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generate JWT token for authenticated user
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roles">User roles</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(ApplicationUser user, IList<string> roles);

        /// <summary>
        /// Get access token expiration time in minutes
        /// </summary>
        int GetTokenExpirationMinutes();

        /// <summary>
        /// Get refresh token expiration time in days
        /// </summary>
        int GetRefreshTokenExpirationDays();

        /// <summary>
        /// Generate a secure random refresh token
        /// </summary>
        string GenerateRefreshToken();
    }
}