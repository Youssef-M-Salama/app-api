using App.Core.Enums;

namespace App.Core.DTOs.Request
{
    public class UsersFilterRequestDTO
    {
        /// <summary>
        /// Optional filter by role.
        /// 0: Charity, 1: DonorOrganization, 2: Admin
        /// </summary>
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
