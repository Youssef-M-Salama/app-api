namespace App.Core.DTOs.Response
{
    public class PendingVerificationsResponseDTO
    {
        public IEnumerable<PendingCharityDTO> PendingCharities { get; set; } = new List<PendingCharityDTO>();
        public IEnumerable<PendingDonorDTO> PendingDonors { get; set; } = new List<PendingDonorDTO>();
    }

    public class PendingCharityDTO
    {
        public Guid CharityId { get; set; }
        public Guid UserId { get; set; }
        public string CharityName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PendingDonorDTO
    {
        public Guid DonorOrganizationId { get; set; }
        public Guid UserId { get; set; }
        public string DonorOrganizationName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
