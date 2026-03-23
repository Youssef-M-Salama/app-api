using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service contract for profile management.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Returns the full profile of the authenticated user.
        /// Includes charity or donor organization details based on role.
        /// </summary>
        Task<ServiceResult<ProfileResponseDTO>> GetProfileAsync(Guid userId);

        /// <summary>
        /// Updates the authenticated user's contact and location fields.
        /// </summary>
        Task<ServiceResult<object>> UpdateProfileAsync(Guid userId, UpdateProfileRequestDTO request);

        /// <summary>
        /// Changes the authenticated user's password.
        /// Validates current password before applying the change.
        /// </summary>
        Task<ServiceResult<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDTO request);

        /// <summary>
        /// Updates the authenticated user's profile image.
        /// Deletes the old image and saves the new one.
        /// </summary>
        Task<ServiceResult<object>> UpdateProfileImageAsync(Guid userId, UpdateProfileImageRequestDTO request);
    }
}