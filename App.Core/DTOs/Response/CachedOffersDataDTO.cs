using App.Core.DTOs.ResultPattern;
using System.Collections.Generic;

namespace App.Core.DTOs.Response
{
    public class CachedOffersDataDTO
    {
        public IEnumerable<OfferResponseDTO> Offers { get; set; } = new List<OfferResponseDTO>();
        public PaginationInfo Pagination { get; set; } = default!;
    }
}
