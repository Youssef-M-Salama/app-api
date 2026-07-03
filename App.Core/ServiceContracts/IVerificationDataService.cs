using App.Core.DTOs.Request;
using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    public interface IVerificationDataService
    {
        Task<ServiceResult<object>> UpdateCharityVerificationDataAsync(Guid userId, UpdateCharityVerificationDataRequestDTO request);
        Task<ServiceResult<object>> UpdateDonorVerificationDataAsync(Guid userId, UpdateDonorVerificationDataRequestDTO request);
    }
}
