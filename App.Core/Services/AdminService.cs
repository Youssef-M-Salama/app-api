using App.Core.Enums;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Domain.RepositoryContracts;
using App.Core.ServiceContracts;

namespace App.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailService _emailService;

        public AdminService(IAdminRepository adminRepository, IEmailService emailService)
        {
            _adminRepository = adminRepository;
            _emailService = emailService;
        }

        public async Task<ServiceResult<AdminDashboardResponseDTO>> GetDashboardStatisticsAsync()
        {
            try
            {
                var stats = await _adminRepository.GetDashboardStatisticsAsync();

                var dto = new AdminDashboardResponseDTO
                {
                    PendingVerifications = stats.PendingVerifications,
                    PendingCharityNeeds = stats.PendingCharityNeeds,
                    PendingOffers = stats.PendingOffers,
                    TotalUsers = stats.TotalUsers,
                    ActiveCharityNeeds = stats.ActiveCharityNeeds,
                    ActiveOffers = stats.ActiveOffers
                };

                return ServiceResult<AdminDashboardResponseDTO>
                    .Success("Dashboard statistics retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<AdminDashboardResponseDTO>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<PendingVerificationsResponseDTO>> GetPendingVerificationsAsync()
        {
            try
            {
                var (pendingCharities, pendingDonors) = await _adminRepository.GetPendingVerificationsAsync();

                var dto = new PendingVerificationsResponseDTO
                {
                    PendingCharities = pendingCharities.Select(c => new PendingCharityDTO
                    {
                        CharityId = c.CharityId,
                        UserId = c.UserId,
                        CharityName = c.CharityName,
                        Email = c.ApplicationUser?.Email,
                        City = c.ApplicationUser?.City,
                        Governorate = c.ApplicationUser?.Governorate,
                        CreatedAt = c.CreatedAt
                    }),
                    PendingDonors = pendingDonors.Select(d => new PendingDonorDTO
                    {
                        DonorOrganizationId = d.DonorOrganizationId,
                        UserId = d.UserId,
                        DonorOrganizationName = d.DonorOrganizationName,
                        Email = d.ApplicationUser?.Email,
                        City = d.ApplicationUser?.City,
                        Governorate = d.ApplicationUser?.Governorate,
                        CreatedAt = d.CreatedAt
                    })
                };

                return ServiceResult<PendingVerificationsResponseDTO>.Success("Pending verifications retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<PendingVerificationsResponseDTO>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> VerifyUserAsync(ActionUserRequestDTO request)
        {
            try
            {
                var (success, email, username) = await _adminRepository.VerifyUserAsync(request.UserId);
                if (!success) return ServiceResult<object>.NotFound("User not found.");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username))
                    await _emailService.SendAccountVerifiedAsync(email, username);

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> RejectUserAsync(ActionUserRequestDTO request)
        {
            try
            {
                var (success, email, username) = await _adminRepository.RejectUserAsync(request.UserId);
                if (!success) return ServiceResult<object>.NotFound("User not found.");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username))
                    await _emailService.SendAccountRejectedAsync(email, username);

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<IEnumerable<CharityNeedResponseDTO>>> GetPendingCharityNeedsAsync(PendingRequestsFilterDTO query)
        {
            try
            {
                if (query.Page <= 0) return ServiceResult<IEnumerable<CharityNeedResponseDTO>>.InvalidPage();
                if (query.PageSize <= 0) return ServiceResult<IEnumerable<CharityNeedResponseDTO>>.InvalidPageSize();
                if (query.PageSize > 50) return ServiceResult<IEnumerable<CharityNeedResponseDTO>>.PageSizeTooLarge();

                var items = await _adminRepository.GetPendingCharityNeedsAsync(query.Page, query.PageSize);
                var totalCount = await _adminRepository.CountPendingCharityNeedsAsync();

                var data = items.Select(cn => new CharityNeedResponseDTO
                {
                    CharityNeedId = cn.CharityNeedId,
                    CharityName = cn.Charity?.CharityName,
                    ProductName = cn.ProductName,
                    Category = cn.Category,
                    City = cn.Charity?.ApplicationUser?.City,
                    Governorate = cn.Charity?.ApplicationUser?.Governorate,
                    Quantity = cn.Quantity,
                    Priority = cn.Priority,
                    Status = cn.Status,
                    CreatedAt = cn.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<CharityNeedResponseDTO>>
                    .SuccessPaginated("CharityNeeds retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CharityNeedResponseDTO>>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> ApproveCharityNeedAsync(ActionCharityNeedRequestDTO request)
        {
            try
            {
                var (success, email, username, productName) = await _adminRepository.ApproveCharityNeedAsync(request.CharityNeedId);
                if (!success) return ServiceResult<object>.NotFound("Pending CharityNeed not found.");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(productName))
                    await _emailService.SendCharityNeedApprovedAsync(email, username, productName);

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> RejectCharityNeedAsync(ActionCharityNeedRequestDTO request)
        {
            try
            {
                var (success, email, username, productName) = await _adminRepository.RejectCharityNeedAsync(request.CharityNeedId);
                if (!success) return ServiceResult<object>.NotFound("Pending CharityNeed not found.");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(productName))
                    await _emailService.SendCharityNeedRejectedAsync(email, username, productName);

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<IEnumerable<OfferResponseDTO>>> GetPendingOffersAsync(PendingRequestsFilterDTO query)
        {
            try
            {
                if (query.Page <= 0) return ServiceResult<IEnumerable<OfferResponseDTO>>.InvalidPage();
                if (query.PageSize <= 0) return ServiceResult<IEnumerable<OfferResponseDTO>>.InvalidPageSize();
                if (query.PageSize > 50) return ServiceResult<IEnumerable<OfferResponseDTO>>.PageSizeTooLarge();

                var items = await _adminRepository.GetPendingOffersAsync(query.Page, query.PageSize);
                var totalCount = await _adminRepository.CountPendingOffersAsync();

                var data = items.Select(o => new OfferResponseDTO
                {
                    OfferId = o.OfferId,
                    DonorOrganizationName = o.DonorOrganization?.DonorOrganizationName ?? string.Empty,
                    ProductName = o.ProductName,
                    Category = o.Category,
                    City = o.DonorOrganization?.ApplicationUser?.City,
                    Governorate = o.DonorOrganization?.ApplicationUser?.Governorate,
                    Quantity = o.Quantity,
                    ProductImage = o.ProductImage,
                    ExpiryDate = o.ExpiryDate,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<OfferResponseDTO>>
                    .SuccessPaginated("Offers retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<OfferResponseDTO>>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> ApproveOfferAsync(ActionOfferRequestDTO request)
        {
            try
            {
                var (success, email, username, productName) = await _adminRepository.ApproveOfferAsync(request.OfferId);
                if (!success) return ServiceResult<object>.NotFound("Pending Offer not found.");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(productName))
                    await _emailService.SendOfferApprovedAsync(email, username, productName);

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> RejectOfferAsync(ActionOfferRequestDTO request)
        {
            try
            {
                var (success, email, username, productName) = await _adminRepository.RejectOfferAsync(request.OfferId);
                if (!success) return ServiceResult<object>.NotFound("Pending Offer not found.");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(productName))
                    await _emailService.SendOfferRejectedAsync(email, username, productName);

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<IEnumerable<UserResponseDTO>>> GetAllUsersAsync(UsersFilterRequestDTO query)
        {
            try
            {
                if (query.Page <= 0) return ServiceResult<IEnumerable<UserResponseDTO>>.InvalidPage();
                if (query.PageSize <= 0) return ServiceResult<IEnumerable<UserResponseDTO>>.InvalidPageSize();
                if (query.PageSize > 50) return ServiceResult<IEnumerable<UserResponseDTO>>.PageSizeTooLarge();

                var users = await _adminRepository.GetAllUsersAsync(query.Role, query.IsActive, query.Page, query.PageSize);
                var totalCount = await _adminRepository.CountAllUsersAsync(query.Role, query.IsActive);

                var data = users.Select(u => {
                    var roleStr = u.Charity != null ? "Charity" : (u.DonorOrganization != null ? "DonorOrganization" : "Admin");
                    Enum.TryParse<UserRole>(roleStr, out var roleEnum);
                    
                    return new UserResponseDTO
                    {
                        UserId = u.Id,
                        Email = u.Email,
                        IsActive = u.IsActive,
                        IsVerified = u.Charity?.IsVerified ?? u.DonorOrganization?.IsVerified ?? true,
                        Name = u.Charity?.CharityName ?? u.DonorOrganization?.DonorOrganizationName ?? "Admin",
                        Role = roleEnum,
                        CreatedAt = u.CreatedAt
                    };
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<UserResponseDTO>>
                    .SuccessPaginated("Users retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserResponseDTO>>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> DeactivateUserAsync(ActionUserRequestDTO request)
        {
            try
            {
                var success = await _adminRepository.DeactivateUserAsync(request.UserId);
                if (!success) return ServiceResult<object>.NotFound("User not found.");

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> ActivateUserAsync(ActionUserRequestDTO request)
        {
            try
            {
                var success = await _adminRepository.ActivateUserAsync(request.UserId);
                if (!success) return ServiceResult<object>.NotFound("User not found.");

                return ServiceResult<object>.Success("Operation completed successfully", null!);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }
    }
}
