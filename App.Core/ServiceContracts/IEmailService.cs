using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service contract for sending emails.
    /// </summary>
    public interface IEmailService
    {
        // =========================================================
        // AUTH
        // =========================================================

        Task<ServiceResult<object>> SendVerificationEmailAsync(
            string to, string username, string verificationLink);

        Task<ServiceResult<object>> SendEmailVerifiedAsync(
            string to, string username);

        // =========================================================
        // ADMIN — account verification
        // =========================================================

        Task<ServiceResult<object>> SendAccountVerifiedAsync(
            string to, string username);

        Task<ServiceResult<object>> SendAccountRejectedAsync(
            string to, string username);

        // =========================================================
        // ADMIN — charity need approval
        // =========================================================

        Task<ServiceResult<object>> SendCharityNeedApprovedAsync(
            string to, string username, string productName);

        Task<ServiceResult<object>> SendCharityNeedRejectedAsync(
            string to, string username, string productName);

        // =========================================================
        // ADMIN — offer approval
        // =========================================================

        Task<ServiceResult<object>> SendOfferApprovedAsync(
            string to, string username, string productName);

        Task<ServiceResult<object>> SendOfferRejectedAsync(
            string to, string username, string productName);

        // =========================================================
        // APPLICATIONS — notify on new application
        // =========================================================

        /// <summary>
        /// Notifies a charity that a donor has applied to their charity need.
        /// Triggered when donor calls POST /donor/charityNeeds/{id}/apply.
        /// </summary>
        Task<ServiceResult<object>> SendNeedApplicationReceivedAsync(
            string to, string charityUsername, string donorName, string productName);

        /// <summary>
        /// Notifies a donor that a charity has applied to their offer.
        /// Triggered when charity calls POST /charity/offers/{id}/apply.
        /// </summary>
        Task<ServiceResult<object>> SendOfferApplicationReceivedAsync(
            string to, string donorUsername, string charityName, string productName);

        // =========================================================
        // APPLICATIONS — notify on accept/reject
        // =========================================================

        /// <summary>
        /// Notifies a donor that the charity accepted their need application.
        /// </summary>
        Task<ServiceResult<object>> SendNeedApplicationAcceptedAsync(
            string to, string donorUsername, string productName);

        /// <summary>
        /// Notifies a donor that the charity rejected their need application.
        /// </summary>
        Task<ServiceResult<object>> SendNeedApplicationRejectedAsync(
            string to, string donorUsername, string productName);

        /// <summary>
        /// Notifies a charity that the donor accepted their offer application.
        /// </summary>
        Task<ServiceResult<object>> SendOfferApplicationAcceptedAsync(
            string to, string charityUsername, string productName);

        /// <summary>
        /// Notifies a charity that the donor rejected their offer application.
        /// </summary>
        Task<ServiceResult<object>> SendOfferApplicationRejectedAsync(
            string to, string charityUsername, string productName);
    }
}