namespace App.Core.DTOs.Request
{
    public class PendingRequestsFilterDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
