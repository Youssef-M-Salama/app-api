using App.Core.Domain.Entities;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for profile-related data access.
    /// Handles loading charity and donor organization details by user id.
    /// </summary>
    public interface IProfileRepository
    {
        /// <summary>
        /// Returns the charity linked to the given user id.
        /// Returns null if the user is not a charity.
        /// </summary>
        Task<Charity?> GetCharityByUserIdAsync(Guid userId);

        /// <summary>
        /// Returns the donor organization linked to the given user id.
        /// Returns null if the user is not a donor organization.
        /// </summary>
        Task<DonorOrganization?> GetDonorOrganizationByUserIdAsync(Guid userId);
    }
}