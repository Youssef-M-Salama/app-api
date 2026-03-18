using App.Core.Domain.IdentityEntities;
using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace App.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        // =========================================================
        // REGISTER
        // =========================================================

        public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDTO request)
        {
            try
            {
                var validationErrors = await ValidateRegisterAsync(request);
                if (validationErrors.Any())
                {
                    return ServiceResult<AuthResponseDto>.ValidationError(
                        "Validation failed",
                        errors =>
                        {
                            foreach (var err in validationErrors)
                                errors.Add(err);
                        });
                }

                var user = new ApplicationUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.Phone,
                    Whatsapp = request.Whatsapp,
                    City = request.City,
                    Governorate = request.Governorate,
                    PostalCode = request.PostalCode,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    return ServiceResult<AuthResponseDto>.ValidationError(
                        "Registration failed",
                        errors =>
                        {
                            foreach (var error in createResult.Errors)
                                errors.Add(new FieldError
                                {
                                    Field = ConvertIdentityErrorCode(error.Code),
                                    Message = error.Description
                                });
                        });
                }

                // Assign role based on AccountType enum
                var roleName = MapAccountTypeToRole(request.AccountType);
                await _userManager.AddToRoleAsync(user, roleName);

                var roles = await _userManager.GetRolesAsync(user);
                var authResponse = await GenerateAndPersistTokensAsync(user, roles);

                return ServiceResult<AuthResponseDto>.Created("Registration successful", authResponse);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponseDto>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        // =========================================================
        // LOGIN
        // =========================================================

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDTO request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UsernameOrEmail)
                           ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);

                if (user == null)
                    return ServiceResult<AuthResponseDto>.Unauthorized("Invalid credentials");

                var signInResult = await _signInManager.CheckPasswordSignInAsync(
                    user, request.Password, lockoutOnFailure: false);

                if (!signInResult.Succeeded)
                    return ServiceResult<AuthResponseDto>.Unauthorized("Invalid credentials");

                if (!user.IsActive)
                    return ServiceResult<AuthResponseDto>.Forbidden("Account is deactivated");

                var roles = await _userManager.GetRolesAsync(user);
                var authResponse = await GenerateAndPersistTokensAsync(user, roles);

                return ServiceResult<AuthResponseDto>.Success("Login successful", authResponse);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponseDto>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        // =========================================================
        // REFRESH TOKEN
        // =========================================================

        public async Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDTO request)
        {
            try
            {
                var user = _userManager.Users
                    .SingleOrDefault(u => u.RefreshToken == request.RefreshToken);

                if (user == null)
                    return ServiceResult<AuthResponseDto>.Unauthorized("Invalid refresh token");

                if (user.RefreshTokenExpiration == null ||
                    user.RefreshTokenExpiration <= DateTime.UtcNow)
                {
                    await RevokeRefreshTokenAsync(user);
                    return ServiceResult<AuthResponseDto>.Unauthorized(
                        "Refresh token has expired. Please log in again");
                }

                if (!user.IsActive)
                    return ServiceResult<AuthResponseDto>.Forbidden("Account is deactivated");

                var roles = await _userManager.GetRolesAsync(user);
                var authResponse = await GenerateAndPersistTokensAsync(user, roles);

                return ServiceResult<AuthResponseDto>.Success("Token refreshed successfully", authResponse);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponseDto>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        // =========================================================
        // LOGOUT
        // =========================================================

        public async Task<ServiceResult<object>> LogoutAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return ServiceResult<object>.NotFound("User not found");

                await RevokeRefreshTokenAsync(user);

                return ServiceResult<object>.Success("Logged out successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        // =========================================================
        // PRIVATE — TOKEN HELPERS
        // =========================================================

        private async Task<AuthResponseDto> GenerateAndPersistTokensAsync(
            ApplicationUser user,
            IList<string> roles)
        {
            var token = _jwtService.GenerateToken(user, roles);
            var tokenExpiration = DateTime.UtcNow.AddMinutes(
                _jwtService.GetTokenExpirationMinutes());

            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(
                _jwtService.GetRefreshTokenExpirationDays());

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = refreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsVerified = false,
                Token = token,
                TokenExpiration = tokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }

        private async Task RevokeRefreshTokenAsync(ApplicationUser user)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiration = null;
            await _userManager.UpdateAsync(user);
        }

        // =========================================================
        // PRIVATE — REGISTER VALIDATION HELPERS
        // =========================================================

        private async Task<List<FieldError>> ValidateRegisterAsync(RegisterDTO request)
        {
            var errors = new List<FieldError>();

            // AccountType is now an enum — no string validation needed.
            // Just check the role exists in the system.
            var roleName = MapAccountTypeToRole(request.AccountType);
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                errors.Add(new FieldError
                {
                    Field = "AccountType",
                    Message = $"Role '{roleName}' is not configured in the system."
                });
            }

            if (request.Password != request.ConfirmPassword)
            {
                errors.Add(new FieldError
                {
                    Field = "ConfirmPassword",
                    Message = "Password and confirm password do not match"
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Username) &&
                !await IsUsernameUniqueAsync(request.Username))
            {
                errors.Add(new FieldError
                {
                    Field = "Username",
                    Message = "Username is already in use"
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                !await IsEmailUniqueAsync(request.Email))
            {
                errors.Add(new FieldError
                {
                    Field = "Email",
                    Message = "Email is already registered"
                });
            }

            return errors;
        }

        /// <summary>
        /// Maps <see cref="AccountType"/> enum to the corresponding ASP.NET Identity role name.
        /// </summary>
        private static string MapAccountTypeToRole(AccountType accountType)
        {
            return accountType switch
            {
                AccountType.Charity => "Charity",
                AccountType.DonorOrganization => "DonorOrganization",
                _ => string.Empty
            };
        }

        private async Task<bool> IsUsernameUniqueAsync(string username)
            => await _userManager.FindByNameAsync(username) == null;

        private async Task<bool> IsEmailUniqueAsync(string email)
            => await _userManager.FindByEmailAsync(email) == null;

        private static string ConvertIdentityErrorCode(string code)
        {
            return code switch
            {
                "PasswordTooShort" => "Password",
                "PasswordRequiresNonAlphanumeric" => "Password",
                "PasswordRequiresDigit" => "Password",
                "PasswordRequiresLower" => "Password",
                "PasswordRequiresUpper" => "Password",
                "DuplicateUserName" => "Username",
                "DuplicateEmail" => "Email",
                _ => code.ToLower()
            };
        }
    }
}