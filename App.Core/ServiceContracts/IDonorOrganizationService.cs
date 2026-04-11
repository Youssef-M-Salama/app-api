using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.ServiceContracts
{
    public interface IDonorOrganizationService
    {
        public Task<ServiceResult<DonorDashboardResponseDTO>> GetDashboardAsync(Guid userId);
        Task<ServiceResult<OfferDetailResponseDTO>> CreateOfferAsync(Guid userId, CreateOfferRequestDTO request);



    
        Task<ServiceResult<IEnumerable<OfferDetailResponseDTO>>> GetMyOffersAsync(Guid userId, MyOffersFilterDTO query);
        Task<ServiceResult<OfferDetailResponseDTO>> GetMyOfferByIdAsync(Guid userId, Guid offerId);
        Task<ServiceResult<object>> UpdateOfferAsync(Guid userId, Guid offerId, UpdateOfferRequestDTO request);
        Task<ServiceResult<object>> DeleteOfferAsync(Guid userId, Guid offerId);
        Task<ServiceResult<object>> FulfillOfferAsync(Guid userId, Guid offerId);
        
        Task<ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>> GetReceivedApplicationsAsync(Guid userId, PaginationFilterDTO query);
        Task<ServiceResult<object>> AcceptOfferApplicationAsync(Guid userId, Guid offerApplicationId);
        Task<ServiceResult<object>> RejectOfferApplicationAsync(Guid userId, Guid offerApplicationId);
}
}
