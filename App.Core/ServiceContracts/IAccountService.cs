using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    public interface IAccountService
    {
        /// <summary>
        /// Register a new user account (charity or donor organization).
        /// Returns 201 Created with a message to check email for verification.
        /// </summary>
        Task<ServiceResult<object>> RegisterAsync(RegisterDTO registerRequest);

        /// <summary>
        /// Authenticate user and generate JWT access token and refresh token.
        /// </summary>
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDTO loginRequest);

        /// <summary>
        /// Issue a new access token and rotate the refresh token.
        /// </summary>
        Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDTO refreshRequest);

        /// <summary>
        /// Logout user by revoking their refresh token.
        /// </summary>
        Task<ServiceResult<object>> LogoutAsync(Guid userId);

        /// <summary>
        /// Verify user email using the token sent to their email address.
        /// </summary>
        Task<ServiceResult<object>> VerifyEmailAsync(Guid userId, string token);

        /// <summary>
        /// Resend email verification link to the user.
        /// </summary>
        Task<ServiceResult<object>> ResendVerificationEmailAsync(string email);
    }
}