using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service contract for user account management (registration, login)
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Register a new user account (charity or donor organization)
        /// </summary>
        /// <param name="registerRequest">Registration data including account type, credentials, and contact information</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (201 Created): User created with JWT token
        /// - Validation Error (400): Invalid input data or duplicate email/username
        /// - Internal Error (500): Unexpected server error
        /// </returns>
        /// <remarks>
        /// This endpoint creates a new user account and assigns appropriate role based on account type.
        /// User will not be verified until admin approval.
        /// All validation is performed via data annotations and remote validation before reaching this method.
        /// </remarks>
        Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDTO registerRequest);

        /// <summary>
        /// Authenticate user and generate JWT token
        /// </summary>
        /// <param name="loginRequest">Login credentials (username or email + password)</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): Login successful with JWT token
        /// - Unauthorized (401): Invalid credentials
        /// - Forbidden (403): Account deactivated
        /// - Internal Error (500): Unexpected server error
        /// </returns>
        /// <remarks>
        /// Accepts either username or email for login.
        /// Returns JWT token valid for duration specified in configuration.
        /// Token must be included in Authorization header for protected endpoints.
        /// </remarks>
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDTO loginRequest);
    }
}