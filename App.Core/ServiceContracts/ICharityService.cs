using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service contract for charity-specific operations.
    /// All methods require the caller to be an authenticated charity user.
    /// </summary>
    public interface ICharityService
    {
        // =========================================================
        // DASHBOARD
        // =========================================================

        /// <summary>
        /// Returns aggregated statistics for the authenticated charity's dashboard.
        /// Includes charity need counts (by status), need applications received
        /// (by status), and offer applications sent (by status).
        /// </summary>
        Task<ServiceResult<CharityDashboardResponseDTO>> GetDashboardAsync(Guid userId);

        // =========================================================
        // CHARITY NEED — CRUD
        // =========================================================

        /// <summary>
        /// Creates a new charity need for the authenticated charity.
        /// The charity must be verified and active.
        /// The need is created with status <c>Pending</c>.
        /// Returns 201 Created with the saved need detail.
        /// </summary>
        Task<ServiceResult<CharityNeedDetailResponseDTO>> CreateCharityNeedAsync(
            Guid userId,
            CreateCharityNeedRequestDTO request);

        /// <summary>
        /// Returns a paginated list of the authenticated charity's own needs.
        /// Optionally filtered by status. All statuses are visible to the owner.
        /// </summary>
        Task<ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>> GetMyCharityNeedsAsync(
            Guid userId,
            MyCharityNeedsFilterDTO query);

        /// <summary>
        /// Returns full detail of a single charity need owned by the authenticated charity.
        /// All statuses are visible to the owner.
        /// Returns 404 if not found or not owned by the caller.
        /// </summary>
        Task<ServiceResult<CharityNeedDetailResponseDTO>> GetMyCharityNeedByIdAsync(
            Guid userId,
            Guid charityNeedId);

        /// <summary>
        /// Updates a pending charity need owned by the authenticated charity.
        /// Only fields provided (non-null) are applied.
        /// Returns 422 if the need is not in <c>Pending</c> status.
        /// </summary>
        Task<ServiceResult<object>> UpdateCharityNeedAsync(
            Guid userId,
            Guid charityNeedId,
            UpdateCharityNeedRequestDTO request);

        /// <summary>
        /// Deletes a pending charity need owned by the authenticated charity.
        /// Returns 422 if the need is not in <c>Pending</c> status.
        /// </summary>
        Task<ServiceResult<object>> DeleteCharityNeedAsync(
            Guid userId,
            Guid charityNeedId);

        /// <summary>
        /// Marks an approved charity need as fulfilled.
        /// Returns 422 if the need is not in <c>Approved</c> status.
        /// </summary>
        Task<ServiceResult<object>> FulfillCharityNeedAsync(
            Guid userId,
            Guid charityNeedId);

        // =========================================================
        // NEED APPLICATIONS — received (donors applied to my needs)
        // =========================================================

        /// <summary>
        /// Returns a paginated list of need applications received by the charity
        /// across all of its charity needs.
        /// </summary>
        Task<ServiceResult<IEnumerable<NeedApplicationResponseDTO>>> GetReceivedApplicationsAsync(
            Guid userId,
            PaginationFilterDTO query);

        /// <summary>
        /// Accepts a pending need application received by the charity.
        /// Returns 403 if the application does not belong to this charity's need.
        /// Returns 422 if the application is not in <c>Pending</c> status.
        /// </summary>
        Task<ServiceResult<object>> AcceptNeedApplicationAsync(
            Guid userId,
            Guid needApplicationId);

        /// <summary>
        /// Rejects a pending need application received by the charity.
        /// Returns 403 if the application does not belong to this charity's need.
        /// Returns 422 if the application is not in <c>Pending</c> status.
        /// </summary>
        Task<ServiceResult<object>> RejectNeedApplicationAsync(
            Guid userId,
            Guid needApplicationId);

        // =========================================================
        // OFFER APPLICATIONS — sent (I applied to donor offers)
        // =========================================================

        /// <summary>
        /// Applies the authenticated charity to an approved donor offer.
        /// Returns 409 if the charity has already applied to this offer.
        /// Returns 404 if the offer does not exist or is not approved.
        /// </summary>
        Task<ServiceResult<object>> ApplyToOfferAsync(
            Guid userId,
            Guid offerId);

        /// <summary>
        /// Returns a paginated list of offer applications sent by the charity.
        /// </summary>
        Task<ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>> GetSentApplicationsAsync(
            Guid userId,
            PaginationFilterDTO query);

        /// <summary>
        /// Cancels a pending offer application sent by the charity.
        /// Returns 403 if the application does not belong to the caller.
        /// Returns 422 if the application is not in <c>Pending</c> status.
        /// </summary>
        Task<ServiceResult<object>> CancelOfferApplicationAsync(
            Guid userId,
            Guid offerApplicationId);
    }
}