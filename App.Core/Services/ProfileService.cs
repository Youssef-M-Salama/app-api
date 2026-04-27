using App.Core.Domain.IdentityEntities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace App.Core.Services
{
    /// <summary>
    /// Handles profile retrieval, update, image, and password change.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileRepository _profileRepository;
        private readonly IFileService _fileService;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            IProfileRepository profileRepository,
            IFileService fileService)
        {
            _userManager = userManager;
            _profileRepository = profileRepository;
            _fileService = fileService;
        }

        // =========================================================
        // PUBLIC METHODS
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<ProfileResponseDTO>> GetProfileAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return ServiceResult<ProfileResponseDTO>.NotFound("User not found");

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? string.Empty;
                Enum.TryParse<UserRole>(role, out var roleEnum);

                var dto = new ProfileResponseDTO
                {
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Phone = user.PhoneNumber,
                    Whatsapp = user.Whatsapp,
                    City = user.City,
                    Governorate = user.Governorate,
                    PostalCode = user.PostalCode,
                    ImageUrl = _fileService.GetImageUrl(user.ImageUrl),
                    Role = roleEnum,
                    IsVerified = false,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                if (role == "Charity")
                {
                    var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                    if (charity != null)
                    {
                        dto.IsVerified = charity.IsVerified;
                        dto.CharityDetails = new CharityDetailsDTO
                        {
                            CharityName = charity.CharityName,
                            CharityDescription = charity.CharityDescription
                        };
                    }
                }
                else if (role == "DonorOrganization")
                {
                    var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                    if (donor != null)
                    {
                        dto.IsVerified = donor.IsVerified;
                        dto.DonorDetails = new DonorDetailsDTO
                        {
                            DonorOrganizationName = donor.DonorOrganizationName,
                            DonorOrganizationDescription = donor.DonorOrganizationDescription
                        };
                    }
                }

                return ServiceResult<ProfileResponseDTO>.Success("Profile retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileResponseDTO>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> UpdateProfileAsync(Guid userId, UpdateProfileRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return ServiceResult<object>.NotFound("User not found");

                user.PhoneNumber = request.Phone ?? user.PhoneNumber;
                user.Whatsapp = request.Whatsapp ?? user.Whatsapp;
                user.City = request.City ?? user.City;
                user.Governorate = request.Governorate ?? user.Governorate;
                user.PostalCode = request.PostalCode ?? user.PostalCode;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return ServiceResult<object>.ValidationError(
                        "Failed to update profile",
                        errors => errors.AddRange(result.Errors.Select(e => new FieldError
                        {
                            Field = e.Code,
                            Message = e.Description
                        })));

                return ServiceResult<object>.Success("Profile updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return ServiceResult<object>.NotFound("User not found");

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword);

                if (!result.Succeeded)
                    return ServiceResult<object>.ValidationError(
                        "Failed to change password",
                        errors => errors.AddRange(result.Errors.Select(e => new FieldError
                        {
                            Field = e.Code,
                            Message = e.Description
                        })));

                return ServiceResult<object>.Success("Password changed successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> UpdateProfileImageAsync(Guid userId, UpdateProfileImageRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return ServiceResult<object>.NotFound("User not found");

                var saveResult = await _fileService.SaveImageAsync(request.Image, ImageFolder.Users);
                if (!saveResult.Response.Success)
                    return ServiceResult<object>.BadRequest(
                        saveResult.Response.Message,
                        saveResult.Response.Error?.Details);

                // Delete old image if exists
                await _fileService.DeleteImageAsync(user.ImageUrl);

                user.ImageUrl = saveResult.Response.Data;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    // Rollback — delete the newly saved image
                    await _fileService.DeleteImageAsync(saveResult.Response.Data);

                    return ServiceResult<object>.ValidationError(
                        "Failed to update profile image",
                        errors => errors.AddRange(result.Errors.Select(e => new FieldError
                        {
                            Field = e.Code,
                            Message = e.Description
                        })));
                }

                return ServiceResult<object>.Success("Profile image updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }
    }
}