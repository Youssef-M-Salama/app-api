using App.Core.DTO.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service contract for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email verification link to the user after registration.
        /// </summary>
        Task<ServiceResult<object>> SendVerificationEmailAsync(
            string to, string username, string verificationLink);

        /// <summary>
        /// Sends a confirmation email after the user successfully verifies their email.
        /// </summary>
        Task<ServiceResult<object>> SendEmailVerifiedAsync(
            string to, string username);
    }
}