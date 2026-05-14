using App.Core.Domain.IdentityEntities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using App.Core.Helpers;
using App.Core.Domain.Enums;

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
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            IProfileRepository profileRepository,
            IFileService fileService,
            ILogger<ProfileService> logger)
        {
            _userManager = userManager;
            _profileRepository = profileRepository;
            _fileService = fileService;
            _logger = logger;
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
                    VerificationState = VerificationState.Verified,
                    VerifyMyAccount = user.VerifyMyAccount,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                if (role == "Charity")
                {
                    var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                    if (charity != null)
                    {
                        dto.VerificationState = charity.VerificationState;
                        dto.CharityDetails = new CharityDetailsDTO
                        {
                            CharityName = charity.CharityName,
                            CharityDescription = charity.CharityDescription,
                            RegistrationNumber = charity.RegistrationNumber,
                            RegistrationDate = charity.RegistrationDate,
                            HeadquartersAddress = charity.HeadquartersAddress,
                            AuthorizedPersonName = charity.AuthorizedPersonName,
                            AuthorizedPersonPosition = charity.AuthorizedPersonPosition,
                            RegistrationCertificateUrl = _fileService.GetPdfUrl(charity.RegistrationCertificateUrl),
                            BylawsUrl = _fileService.GetPdfUrl(charity.BylawsUrl),
                            FoundersListUrl = _fileService.GetPdfUrl(charity.FoundersListUrl),
                            BoardMembersListUrl = _fileService.GetPdfUrl(charity.BoardMembersListUrl),
                            HeadquartersProofUrl = _fileService.GetPdfUrl(charity.HeadquartersProofUrl),
                            DelegationDocumentUrl = _fileService.GetPdfUrl(charity.DelegationDocumentUrl)
                        };
                    }
                }
                else if (role == "DonorOrganization")
                {
                    var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                    if (donor != null)
                    {
                        dto.VerificationState = donor.VerificationState;
                        dto.DonorDetails = new DonorDetailsDTO
                        {
                            DonorOrganizationName = donor.DonorOrganizationName,
                            DonorOrganizationDescription = donor.DonorOrganizationDescription,
                            CommercialRegistrationNumber = donor.CommercialRegistrationNumber,
                            CommercialRegistrationDate = donor.CommercialRegistrationDate,
                            TaxNumber = donor.TaxNumber,
                            BusinessLicenseNumber = donor.BusinessLicenseNumber,
                            HeadquartersAddress = donor.HeadquartersAddress,
                            CommercialRegisterUrl = _fileService.GetPdfUrl(donor.CommercialRegisterUrl),
                            TaxCardUrl = _fileService.GetPdfUrl(donor.TaxCardUrl),
                            BusinessLicenseUrl = _fileService.GetPdfUrl(donor.BusinessLicenseUrl),
                            CivilProtectionApprovalUrl = _fileService.GetPdfUrl(donor.CivilProtectionApprovalUrl),
                            EnvironmentalApprovalUrl = _fileService.GetPdfUrl(donor.EnvironmentalApprovalUrl),
                            OwnershipContractUrl = _fileService.GetPdfUrl(donor.OwnershipContractUrl)
                        };
                    }
                }

                return ServiceResult<ProfileResponseDTO>.Success("تم استرجاع الملف الشخصي بنجاح", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProfileService");
                return ServiceResult<ProfileResponseDTO>.Internal(
                    "حدث خطأ غير متوقع",
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

                user.PhoneNumber = PhoneNumberHelper.NormalizeEgyptianPhoneNumber(request.Phone) ?? user.PhoneNumber;
                user.Whatsapp = PhoneNumberHelper.NormalizeEgyptianPhoneNumber(request.Whatsapp) ?? user.Whatsapp;
                user.City = request.City ?? user.City;
                user.Governorate = request.Governorate ?? user.Governorate;
                user.PostalCode = request.PostalCode ?? user.PostalCode;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return ServiceResult<object>.ValidationError(
                        "فشل تحديث الملف الشخصي",
                        errors => errors.AddRange(result.Errors.Select(e => new FieldError
                        {
                            Field = e.Code,
                            Message = e.Description
                        })));

                return ServiceResult<object>.Success("تم تحديث الملف الشخصي بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProfileService");
                return ServiceResult<object>.Internal(
                    "حدث خطأ غير متوقع",
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
                        "فشل تغيير كلمة المرور",
                        errors => errors.AddRange(result.Errors.Select(e => new FieldError
                        {
                            Field = e.Code,
                            Message = e.Description
                        })));

                return ServiceResult<object>.Success("تم تغيير كلمة المرور بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProfileService");
                return ServiceResult<object>.Internal(
                    "حدث خطأ غير متوقع",
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
                        "فشل تحديث صورة الملف الشخصي",
                        errors => errors.AddRange(result.Errors.Select(e => new FieldError
                        {
                            Field = e.Code,
                            Message = e.Description
                        })));
                }

                return ServiceResult<object>.Success("تم تحديث صورة الملف الشخصي بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProfileService");
                return ServiceResult<object>.Internal(
                    "حدث خطأ غير متوقع",
                    new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SubmitForVerificationAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return ServiceResult<object>.NotFound("User not found");
                
                if (user.VerifyMyAccount)
                    return ServiceResult<object>.BadRequest("حسابك قيد المراجعة بالفعل أو تم طلب التحقق مسبقاً.");

                VerificationState currentState = VerificationState.Verified;

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                if (role == "Charity")
                {
                    var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                    if (charity != null) currentState = charity.VerificationState;
                }
                else if (role == "DonorOrganization")
                {
                    var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                    if (donor != null) currentState = donor.VerificationState;
                }
                else
                {
                    return ServiceResult<object>.BadRequest("هذا الإجراء متاح فقط للجمعيات والجهات المانحة.");
                }

                if (currentState != VerificationState.Pending)
                    return ServiceResult<object>.BadRequest("لا يمكنك تقديم طلب تحقق لأن حسابك قيد المراجعة أو تم البت فيه بالفعل.");

                user.VerifyMyAccount = true;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return ServiceResult<object>.Internal("فشل إرسال طلب التحقق");

                return ServiceResult<object>.Success("تم إرسال طلب التحقق بنجاح، يرجى انتظار مراجعة المسؤول.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in SubmitForVerificationAsync");
                return ServiceResult<object>.Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> CancelVerificationRequestAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return ServiceResult<object>.NotFound("User not found");

                if (!user.VerifyMyAccount)
                    return ServiceResult<object>.BadRequest("لم يتم إرسال طلب تحقق لإلغائه.");

                // Check VerificationState from Charity or Donor
                VerificationState currentState = VerificationState.Verified; // default fail-safe

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                if (role == "Charity")
                {
                    var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                    if (charity != null) currentState = charity.VerificationState;
                }
                else if (role == "DonorOrganization")
                {
                    var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                    if (donor != null) currentState = donor.VerificationState;
                }
                else
                {
                    return ServiceResult<object>.BadRequest("هذا الإجراء متاح فقط للجمعيات والجهات المانحة.");
                }

                if (currentState != VerificationState.Pending)
                    return ServiceResult<object>.BadRequest("لا يمكنك إلغاء الطلب لأن حسابك قيد المراجعة أو تم البت فيه بالفعل.");

                user.VerifyMyAccount = false;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return ServiceResult<object>.Internal("فشل إلغاء طلب التحقق");

                return ServiceResult<object>.Success("تم إلغاء طلب التحقق بنجاح.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CancelVerificationRequestAsync");
                return ServiceResult<object>.Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }
    }
}