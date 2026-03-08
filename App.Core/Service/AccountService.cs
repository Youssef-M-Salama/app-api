using App.Core.Domain.IdentityEntities;
using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;
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

        private static readonly HashSet<string> AllowedAccountTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "charity",
            "donor_organization"
        };

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
                // Cross-field and uniqueness validation only.
                // Model-attribute validations are handled by ModelState upstream.
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

                // Build user entity
                var user = new ApplicationUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Persist user with hashed password
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

                // Assign role based on account type
                var roleName = MapAccountTypeToRole(request.AccountType);
                if (!string.IsNullOrWhiteSpace(roleName))
                    await _userManager.AddToRoleAsync(user, roleName);

                // Generate access token + refresh token, persist refresh token
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
                // Find user by username or email
                var user = await _userManager.FindByNameAsync(request.UsernameOrEmail)
                           ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);

                if (user == null)
                    return ServiceResult<AuthResponseDto>.Unauthorized("Invalid credentials");

                // Validate password
                var signInResult = await _signInManager.CheckPasswordSignInAsync(
                    user, request.Password, lockoutOnFailure: false);

                if (!signInResult.Succeeded)
                    return ServiceResult<AuthResponseDto>.Unauthorized("Invalid credentials");

                // Check account is active
                if (!user.IsActive)
                    return ServiceResult<AuthResponseDto>.Forbidden("Account is deactivated");

                // Generate access token + refresh token, persist refresh token
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
                // Find the user who owns this exact refresh token value
                var user = _userManager.Users
                    .SingleOrDefault(u => u.RefreshToken == request.RefreshToken);

                // Token not found — either invalid or already rotated (reuse attack)
                if (user == null)
                    return ServiceResult<AuthResponseDto>.Unauthorized(
                        "Invalid refresh token");

                // Token found but has expired
                if (user.RefreshTokenExpiration == null ||
                    user.RefreshTokenExpiration <= DateTime.UtcNow)
                {
                    // Clear stale token to keep the DB clean
                    await RevokeRefreshTokenAsync(user);
                    return ServiceResult<AuthResponseDto>.Unauthorized(
                        "Refresh token has expired. Please log in again");
                }

                // Check account is still active
                if (!user.IsActive)
                    return ServiceResult<AuthResponseDto>.Forbidden("Account is deactivated");

                // Issue new access token and rotate refresh token
                var roles = await _userManager.GetRolesAsync(user);
                var authResponse = await GenerateAndPersistTokensAsync(user, roles);

                return ServiceResult<AuthResponseDto>.Success(
                    "Token refreshed successfully", authResponse);
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

                // Revoke refresh token so it cannot be reused
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

        /// <summary>
        /// Generates a new access token and rotates the refresh token,
        /// persists the new refresh token to the user row, and returns
        /// a fully populated AuthResponseDto.
        /// Shared by Register, Login, and RefreshToken flows.
        /// </summary>
        private async Task<AuthResponseDto> GenerateAndPersistTokensAsync(
            ApplicationUser user,
            IList<string> roles)
        {
            // Access token
            var token = _jwtService.GenerateToken(user, roles);
            var tokenExpiration = DateTime.UtcNow.AddMinutes(
                _jwtService.GetTokenExpirationMinutes());

            // Refresh token — always a brand new random value (rotation)
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(
                _jwtService.GetRefreshTokenExpirationDays());

            // Persist refresh token to DB
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

        /// <summary>
        /// Nulls out the refresh token on the user row.
        /// Called on logout and when an expired token is detected.
        /// </summary>
        private async Task RevokeRefreshTokenAsync(ApplicationUser user)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiration = null;
            await _userManager.UpdateAsync(user);
        }

        // =========================================================
        // PRIVATE — REGISTER VALIDATION HELPERS
        // =========================================================

        /// <summary>
        /// Aggregates cross-field and uniqueness validation checks only.
        /// Avoids duplicating DTO model-attribute validations handled by ModelState.
        /// </summary>
        private async Task<List<FieldError>> ValidateRegisterAsync(RegisterDTO request)
        {
            var errors = new List<FieldError>();

            if (!IsAllowedAccountType(request.AccountType))
            {
                errors.Add(new FieldError
                {
                    Field = "AccountType",
                    Message = "Account type must be either 'charity' or 'donor_organization'."
                });
            }
            else
            {
                var roleName = MapAccountTypeToRole(request.AccountType);
                if (!string.IsNullOrWhiteSpace(roleName) &&
                    !await _roleManager.RoleExistsAsync(roleName))
                {
                    errors.Add(new FieldError
                    {
                        Field = "AccountType",
                        Message = $"Role '{roleName}' is not configured in the system."
                    });
                }
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

        private static bool IsAllowedAccountType(string? accountType)
            => !string.IsNullOrWhiteSpace(accountType) &&
               AllowedAccountTypes.Contains(accountType.Trim());

        private static string MapAccountTypeToRole(string? accountType)
        {
            if (string.IsNullOrWhiteSpace(accountType)) return string.Empty;

            return accountType.Trim().ToLowerInvariant() switch
            {
                "charity" => "Charity",
                "donor_organization" => "DonorOrganization",
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