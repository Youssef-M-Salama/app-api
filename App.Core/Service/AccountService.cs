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

        public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDTO request)
        {
            try
            {
                // Run aggregated server-side validation (only cross-field & uniqueness checks).
                // Do NOT duplicate model-attribute validations already enforced by ModelState.
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

                // Create user entity
                var user = new ApplicationUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Persist user
                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    return ServiceResult<AuthResponseDto>.ValidationError(
                        "Registration failed",
                        errors =>
                        {
                            foreach (var error in createResult.Errors)
                            {
                                errors.Add(new FieldError
                                {
                                    Field = ConvertIdentityErrorCode(error.Code),
                                    Message = error.Description
                                });
                            }
                        });
                }

                // Map account type to role and assign
                var roleName = MapAccountTypeToRole(request.AccountType);
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }

                // Generate JWT token
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);
                var expiration = DateTime.UtcNow.AddMinutes(_jwtService.GetTokenExpirationMinutes());

                // Build response
                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    IsVerified = false,
                    Token = token,
                    TokenExpiration = expiration
                };

                return ServiceResult<AuthResponseDto>.Created("Registration successful", response);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponseDto>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message }
                );
            }
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDTO request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UsernameOrEmail)
                           ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);

                if (user == null)
                    return ServiceResult<AuthResponseDto>.Unauthorized("Invalid credentials");

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
                if (!result.Succeeded)
                    return ServiceResult<AuthResponseDto>.Unauthorized("Invalid credentials");

                if (!user.IsActive)
                    return ServiceResult<AuthResponseDto>.Forbidden("Account is deactivated");

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);
                var expiration = DateTime.UtcNow.AddMinutes(_jwtService.GetTokenExpirationMinutes());

                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    IsVerified = false,
                    Token = token,
                    TokenExpiration = expiration
                };

                return ServiceResult<AuthResponseDto>.Success("Login successful", response);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponseDto>.Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        // -----------------------
        // Validation helpers
        // -----------------------

        /// <summary>
        /// Aggregates cross-field and uniqueness validation checks only.
        /// Avoids duplicating DTO model-attribute validations that are handled by ModelState.
        /// </summary>
        private async Task<List<FieldError>> ValidateRegisterAsync(RegisterDTO request)
        {
            var errors = new List<FieldError>();

            // Account type allowed?
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
                // If we map to a role, ensure the role actually exists in the system.
                var roleName = MapAccountTypeToRole(request.AccountType);
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    var roleExists = await _roleManager.RoleExistsAsync(roleName);
                    if (!roleExists)
                    {
                        errors.Add(new FieldError
                        {
                            Field = "AccountType",
                            Message = $"Role '{roleName}' is not configured in the system."
                        });
                    }
                }
            }

            // Password confirmation (cross-field)
            if (request.Password != request.ConfirmPassword)
            {
                errors.Add(new FieldError
                {
                    Field = "ConfirmPassword",
                    Message = "Password and confirm password do not match"
                });
            }

            // Uniqueness checks (username/email). Performed only if values provided to avoid duplicating required checks.
            if (!string.IsNullOrWhiteSpace(request.Username) && !await IsUsernameUniqueAsync(request.Username))
            {
                errors.Add(new FieldError
                {
                    Field = "Username",
                    Message = "Username is already in use"
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && !await IsEmailUniqueAsync(request.Email))
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
        /// Returns true when accountType is allowed.
        /// Centralized for reuse.
        /// </summary>
        private static bool IsAllowedAccountType(string? accountType)
            => !string.IsNullOrWhiteSpace(accountType) && AllowedAccountTypes.Contains(accountType.Trim());

        /// <summary>
        /// Maps incoming account type to role name used by Identity.
        /// Centralized to avoid scattered string literals.
        /// Returns null/empty when mapping cannot be made.
        /// </summary>
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

        /// <summary>
        /// Username uniqueness check (wraps UserManager).
        /// Separated for unit testing and reuse.
        /// </summary>
        private async Task<bool> IsUsernameUniqueAsync(string username)
        {
            var existing = await _userManager.FindByNameAsync(username);
            return existing == null;
        }

        /// <summary>
        /// Email uniqueness check (wraps UserManager).
        /// Separated for unit testing and reuse.
        /// </summary>
        private async Task<bool> IsEmailUniqueAsync(string email)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            return existing == null;
        }

        private string ConvertIdentityErrorCode(string code)
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