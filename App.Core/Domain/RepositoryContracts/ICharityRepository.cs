using App.Core.Domain.Entities;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for <see cref="Charity"/> data access.
    /// </summary>
    public interface ICharityRepository
    {
        /// <summary>
        /// Returns the count of verified and active charities on the platform.
        /// Used to compute <c>TotalCharities</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountTotalCharitiesAsync();

        /// <summary>
        /// Creates a new Charity profile linked to a user.
        /// </summary>
        Task AddAsync(Charity charity);
        
        Task <bool> IsVerifiedByUserId(Guid userId);
    }
}