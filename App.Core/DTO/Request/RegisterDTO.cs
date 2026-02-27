using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTO.Request
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Account type is required")]
        [RegularExpression("^(charity|donor_organization)$", ErrorMessage = "Account type must be 'charity' or 'donor_organization'")]
        public string AccountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name can't be blank")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username can't be blank")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [Remote(action: "IsUsernameAlreadyRegistered", controller: "Account", ErrorMessage = "Username is already in use")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in proper format")]
        [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already registered")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number can't be blank")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can't be blank")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password can't be blank")]
        [Compare(nameof(Password), ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        
    }
}