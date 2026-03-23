using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class UpdateProfileRequestDTO
    {
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        public string? Whatsapp { get; set; }

        [StringLength(100, ErrorMessage = "City must not exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "Governorate must not exceed 100 characters")]
        public string? Governorate { get; set; }

        [StringLength(20, ErrorMessage = "Postal code must not exceed 20 characters")]
        public string? PostalCode { get; set; }
    }
}