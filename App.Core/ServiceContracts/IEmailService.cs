using App.Core.DTOs.ResultPattern;

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

        /// <summary>
        /// Sends an email notifying the user that their account has been verified by admin.
        /// </summary>
        Task<ServiceResult<object>> SendAccountVerifiedAsync(
            string to, string username);

        /// <summary>
        /// Sends an email notifying the user that their account has been rejected by admin.
        /// </summary>
        Task<ServiceResult<object>> SendAccountRejectedAsync(
            string to, string username);

        /// <summary>
        /// Sends an email notifying the charity that their need has been approved by admin.
        /// </summary>
        Task<ServiceResult<object>> SendCharityNeedApprovedAsync(
            string to, string username, string productName);

        /// <summary>
        /// Sends an email notifying the charity that their need has been rejected by admin.
        /// </summary>
        Task<ServiceResult<object>> SendCharityNeedRejectedAsync(
            string to, string username, string productName);

        /// <summary>
        /// Sends an email notifying the donor organization that their offer has been approved by admin.
        /// </summary>
        Task<ServiceResult<object>> SendOfferApprovedAsync(
            string to, string username, string productName);

        /// <summary>
        /// Sends an email notifying the donor organization that their offer has been rejected by admin.
        /// </summary>
        Task<ServiceResult<object>> SendOfferRejectedAsync(
            string to, string username, string productName);
    }
}