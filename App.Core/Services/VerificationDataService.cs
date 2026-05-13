using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.Domain.Enums;
using App.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace App.Core.Services
{
    public class VerificationDataService : IVerificationDataService
    {
        private readonly ICharityRepository _charityRepository;
        private readonly IDonorOrganizationRepository _donorOrganizationRepository;
        private readonly IFileService _fileService;
        private readonly ILogger<VerificationDataService> _logger;

        public VerificationDataService(
            ICharityRepository charityRepository,
            IDonorOrganizationRepository donorOrganizationRepository,
            IFileService fileService,
            ILogger<VerificationDataService> logger)
        {
            _charityRepository = charityRepository;
            _donorOrganizationRepository = donorOrganizationRepository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<ServiceResult<object>> UpdateCharityVerificationDataAsync(Guid userId, UpdateCharityVerificationDataRequestDTO request)
        {
            try
            {
                var charity = await _charityRepository.GetByUserIdAsync(userId);
                if (charity == null)
                    return ServiceResult<object>.NotFound("Charity not found.");

                if (charity.VerificationState != VerificationState.Pending)
                    return ServiceResult<object>.Forbidden("لا يمكنك تعديل البيانات إلا إذا كانت حالة الحساب 'قيد الانتظار'. الحسابات التي يتم مراجعتها أو توثيقها أو رفضها لا يمكن تعديل بياناتها.");

                // Update plain strings
                if (request.RegistrationNumber != null) charity.RegistrationNumber = request.RegistrationNumber;
                if (request.RegistrationDate.HasValue) charity.RegistrationDate = request.RegistrationDate.Value;
                if (request.HeadquartersAddress != null) charity.HeadquartersAddress = request.HeadquartersAddress;
                if (request.AuthorizedPersonName != null) charity.AuthorizedPersonName = request.AuthorizedPersonName;
                if (request.AuthorizedPersonPosition != null) charity.AuthorizedPersonPosition = request.AuthorizedPersonPosition;

                // Handle file uploads
                var uploadResult = await ProcessCharityFileUploadsAsync(charity, request);
                if (!uploadResult.Response.Success)
                    return uploadResult;

                await _charityRepository.UpdateAsync(charity);
                return ServiceResult<object>.Success("تم تحديث بيانات الجمعية بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateCharityVerificationDataAsync");
                return ServiceResult<object>.Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> UpdateDonorVerificationDataAsync(Guid userId, UpdateDonorVerificationDataRequestDTO request)
        {
            try
            {
                var donor = await _donorOrganizationRepository.GetByUserIdAsync(userId);
                if (donor == null)
                    return ServiceResult<object>.NotFound("Donor organization not found.");

                if (donor.VerificationState != VerificationState.Pending)
                    return ServiceResult<object>.Forbidden("لا يمكنك تعديل البيانات إلا إذا كانت حالة الحساب 'قيد الانتظار'. الحسابات التي يتم مراجعتها أو توثيقها أو رفضها لا يمكن تعديل بياناتها.");

                if (request.CommercialRegistrationNumber != null) donor.CommercialRegistrationNumber = request.CommercialRegistrationNumber;
                if (request.CommercialRegistrationDate.HasValue) donor.CommercialRegistrationDate = request.CommercialRegistrationDate.Value;
                if (request.TaxNumber != null) donor.TaxNumber = request.TaxNumber;
                if (request.BusinessLicenseNumber != null) donor.BusinessLicenseNumber = request.BusinessLicenseNumber;
                if (request.HeadquartersAddress != null) donor.HeadquartersAddress = request.HeadquartersAddress;

                var uploadResult = await ProcessDonorFileUploadsAsync(donor, request);
                if (!uploadResult.Response.Success)
                    return uploadResult;

                await _donorOrganizationRepository.UpdateAsync(donor);
                return ServiceResult<object>.Success("تم تحديث بيانات المؤسسة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateDonorVerificationDataAsync");
                return ServiceResult<object>.Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        private async Task<ServiceResult<object>> ProcessCharityFileUploadsAsync(App.Core.Domain.Entities.Charity charity, UpdateCharityVerificationDataRequestDTO request)
        {
            var r1 = await TryUploadPdf(request.RegistrationCertificate, PdfFolder.RegistrationCertificate, charity.RegistrationCertificateUrl);
            if (!r1.Response.Success) return ServiceResult<object>.BadRequest(r1.Response.Message); else charity.RegistrationCertificateUrl = r1.Response.Data ?? charity.RegistrationCertificateUrl;

            var r2 = await TryUploadPdf(request.Bylaws, PdfFolder.Bylaws, charity.BylawsUrl);
            if (!r2.Response.Success) return ServiceResult<object>.BadRequest(r2.Response.Message); else charity.BylawsUrl = r2.Response.Data ?? charity.BylawsUrl;

            var r3 = await TryUploadPdf(request.FoundersList, PdfFolder.FoundersList, charity.FoundersListUrl);
            if (!r3.Response.Success) return ServiceResult<object>.BadRequest(r3.Response.Message); else charity.FoundersListUrl = r3.Response.Data ?? charity.FoundersListUrl;

            var r4 = await TryUploadPdf(request.BoardMembersList, PdfFolder.BoardMembersList, charity.BoardMembersListUrl);
            if (!r4.Response.Success) return ServiceResult<object>.BadRequest(r4.Response.Message); else charity.BoardMembersListUrl = r4.Response.Data ?? charity.BoardMembersListUrl;

            var r5 = await TryUploadPdf(request.HeadquartersProof, PdfFolder.HeadquartersProof, charity.HeadquartersProofUrl);
            if (!r5.Response.Success) return ServiceResult<object>.BadRequest(r5.Response.Message); else charity.HeadquartersProofUrl = r5.Response.Data ?? charity.HeadquartersProofUrl;

            var r6 = await TryUploadPdf(request.DelegationDocument, PdfFolder.DelegationDocument, charity.DelegationDocumentUrl);
            if (!r6.Response.Success) return ServiceResult<object>.BadRequest(r6.Response.Message); else charity.DelegationDocumentUrl = r6.Response.Data ?? charity.DelegationDocumentUrl;

            return ServiceResult<object>.Success("");
        }

        private async Task<ServiceResult<object>> ProcessDonorFileUploadsAsync(App.Core.Domain.Entities.DonorOrganization donor, UpdateDonorVerificationDataRequestDTO request)
        {
            var r1 = await TryUploadPdf(request.CommercialRegister, PdfFolder.CommercialRegister, donor.CommercialRegisterUrl);
            if (!r1.Response.Success) return ServiceResult<object>.BadRequest(r1.Response.Message); else donor.CommercialRegisterUrl = r1.Response.Data ?? donor.CommercialRegisterUrl;

            var r2 = await TryUploadPdf(request.TaxCard, PdfFolder.TaxCard, donor.TaxCardUrl);
            if (!r2.Response.Success) return ServiceResult<object>.BadRequest(r2.Response.Message); else donor.TaxCardUrl = r2.Response.Data ?? donor.TaxCardUrl;

            var r3 = await TryUploadPdf(request.BusinessLicense, PdfFolder.BusinessLicense, donor.BusinessLicenseUrl);
            if (!r3.Response.Success) return ServiceResult<object>.BadRequest(r3.Response.Message); else donor.BusinessLicenseUrl = r3.Response.Data ?? donor.BusinessLicenseUrl;

            var r4 = await TryUploadPdf(request.CivilProtectionApproval, PdfFolder.CivilProtectionApproval, donor.CivilProtectionApprovalUrl);
            if (!r4.Response.Success) return ServiceResult<object>.BadRequest(r4.Response.Message); else donor.CivilProtectionApprovalUrl = r4.Response.Data ?? donor.CivilProtectionApprovalUrl;

            var r5 = await TryUploadPdf(request.EnvironmentalApproval, PdfFolder.EnvironmentalApproval, donor.EnvironmentalApprovalUrl);
            if (!r5.Response.Success) return ServiceResult<object>.BadRequest(r5.Response.Message); else donor.EnvironmentalApprovalUrl = r5.Response.Data ?? donor.EnvironmentalApprovalUrl;

            var r6 = await TryUploadPdf(request.OwnershipContract, PdfFolder.OwnershipContract, donor.OwnershipContractUrl);
            if (!r6.Response.Success) return ServiceResult<object>.BadRequest(r6.Response.Message); else donor.OwnershipContractUrl = r6.Response.Data ?? donor.OwnershipContractUrl;

            return ServiceResult<object>.Success("");
        }

        private async Task<ServiceResult<string>> TryUploadPdf(IFormFile? file, PdfFolder folder, string? existingPath)
        {
            if (file == null)
                return ServiceResult<string>.Success("", null!);

            var saveResult = await _fileService.SavePdfAsync(file, folder);
            if (!saveResult.Response.Success)
                return ServiceResult<string>.BadRequest(saveResult.Response.Message);

            if (!string.IsNullOrEmpty(existingPath))
                await _fileService.DeletePdfAsync(existingPath);

            return saveResult;
        }
    }
}
