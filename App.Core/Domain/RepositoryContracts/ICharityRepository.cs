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
    }
}