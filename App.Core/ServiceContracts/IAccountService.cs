using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service contract for user account management (registration, login, logout, token refresh)
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Register a new user account (charity or donor organization)
        /// </summary>
        /// <param name="registerRequest">Registration data including account type, credentials, and contact information</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (201 Created): User created with JWT access token and refresh token
        /// - Validation Error (400): Invalid input data or duplicate email/username
        /// - Internal Error (500): Unexpected server error
        /// </returns>
        Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDTO registerRequest);

        /// <summary>
        /// Authenticate user and generate JWT access token and refresh token
        /// </summary>
        /// <param name="loginRequest">Login credentials (username or email + password)</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): Login successful with JWT access token and refresh token
        /// - Unauthorized (401): Invalid credentials
        /// - Forbidden (403): Account deactivated
        /// - Internal Error (500): Unexpected server error
        /// </returns>
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDTO loginRequest);

        /// <summary>
        /// Issue a new access token and rotate the refresh token
        /// </summary>
        /// <param name="refreshRequest">Request body containing the current refresh token</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): New access token and rotated refresh token
        /// - Unauthorized (401): Refresh token not found, expired, or already used
        /// - Forbidden (403): Account deactivated
        /// - Internal Error (500): Unexpected server error
        /// </returns>
        Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDTO refreshRequest);

        /// <summary>
        /// Logout user by revoking their refresh token
        /// </summary>
        /// <param name="userId">The ID of the authenticated user performing logout</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): Logout successful, refresh token cleared
        /// - Not Found (404): User not found
        /// - Internal Error (500): Unexpected server error
        /// </returns>
        Task<ServiceResult<object>> LogoutAsync(Guid userId);
    }
}