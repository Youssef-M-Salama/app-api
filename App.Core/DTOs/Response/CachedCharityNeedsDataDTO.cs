using App.Core.DTOs.ResultPattern;
using System.Collections.Generic;

namespace App.Core.DTOs.Response
{
    public class CachedCharityNeedsDataDTO
    {
        public IEnumerable<CharityNeedResponseDTO> CharityNeeds { get; set; } = new List<CharityNeedResponseDTO>();
        public PaginationInfo Pagination { get; set; } = default!;
    }
}
