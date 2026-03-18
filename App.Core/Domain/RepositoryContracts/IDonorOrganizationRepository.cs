using App.Core.Domain.Entities;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for <see cref="DonorOrganization"/> data access.
    /// </summary>
    public interface IDonorOrganizationRepository
    {
        /// <summary>
        /// Returns the count of verified and active donor organizations on the platform.
        /// Used to compute <c>TotalDonors</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountTotalDonorsAsync();
    }
}