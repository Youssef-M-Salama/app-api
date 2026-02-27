using App.Core.Domain.IdentityEntities;
using System.Security.Claims;

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
        /// Get token expiration time in minutes
        /// </summary>
        int GetTokenExpirationMinutes();
    }
}