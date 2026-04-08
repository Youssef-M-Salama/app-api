using App.Core.Domain.Entities;
using App.Core.Domain.IdentityEntities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using App.Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text;

namespace App.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;
        private readonly ICharityRepository _charityRepository;
        private readonly IDonorOrganizationRepository _donorOrganizationRepository;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IEmailService emailService,
            IOptions<AppSettings> appSettings,
            ICharityRepository charityRepository,
            IDonorOrganizationRepository donorOrganizationRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _appSettings = appSettings.Value;
            _charityRepository = charityRepository;
            _donorOrganizationRepository = donorOrganizationRepository;
        }

        // =========================================================
        // REGISTER
        // =========================================================

        public async Task<ServiceResult<object>> RegisterAsync(RegisterDTO request)
        {
            try
            {
                // STEP 1 — Validate inputs
                var validationErrors = await ValidateRegisterAsync(request);

                if (validationErrors.Any())
                {
                    return ServiceResult<object>.ValidationError(
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

                // STEP 3 — Create ApplicationUser
                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    return ServiceResult<object>.ValidationError(
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

                // STEP 3 — Create organization row (IsVerified = false -> admin must approve)
                try
                {
                    if (request.AccountType == AccountType.Charity)
                    {
                        await _charityRepository.AddAsync(new Charity
                        {
                            CharityId          = Guid.NewGuid(),
                            CharityName        = request.Name,
                            CharityDescription = request.Description,
                            IsVerified         = false,
                            IsActive           = true,
                            CreatedAt          = DateTime.UtcNow,
                            UpdatedAt          = DateTime.UtcNow,
                            UserId             = user.Id
                        });
                    }
                    else if (request.AccountType == AccountType.DonorOrganization)
                    {
                        await _donorOrganizationRepository.AddAsync(new DonorOrganization
                        {
                            DonorOrganizationId          = Guid.NewGuid(),
                            DonorOrganizationName        = request.Name,
                            DonorOrganizationDescription = request.Description,
                            IsVerified = false,
                            IsActive   = true,
                            CreatedAt  = DateTime.UtcNow,
                            UpdatedAt  = DateTime.UtcNow,
                            UserId     = user.Id
                        });
                    }
                }
                catch
                {
                    // Rollback: delete the user
                    await _userManager.DeleteAsync(user);
                    throw; // let the outer catch return Internal error
                }

                // STEP 6 — Assign role, send email
                var roleName = MapAccountTypeToRole(request.AccountType);
                await _userManager.AddToRoleAsync(user, roleName);

                // Generate and encode email verification token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                var verificationLink = $"{_appSettings.BaseUrl}/api/v1/auth/verify-email" +
                                       $"?userId={user.Id}&token={encodedToken}";

                // Send verification email
                var emailResult = await _emailService.SendVerificationEmailAsync(
                    user.Email!, user.UserName!, verificationLink);

                if (!emailResult.Response.Success)
                    return ServiceResult<object>.Created(
                        "Registration successful but verification email could not be sent. " +
                        "Please use resend verification.");

                return ServiceResult<object>.Created(
                    "Registration successful. Please check your email to verify your account.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal(
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

                if (signInResult == SignInResult.NotAllowed)
                    return ServiceResult<AuthResponseDto>.Forbidden(
                        "Please verify your email before logging in");

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
        // VERIFY EMAIL
        // =========================================================

        public async Task<ServiceResult<object>> VerifyEmailAsync(Guid userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return ServiceResult<object>.NotFound("User not found");

                if (user.EmailConfirmed)
                    return ServiceResult<object>.Success("Email is already verified");

                // Decode Base64 token back to raw Identity token
                var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (!result.Succeeded)
                    return ServiceResult<object>.BadRequest(
                        "Invalid or expired verification token");

                // Send email verified confirmation
                await _emailService.SendEmailVerifiedAsync(user.Email!, user.UserName!);

                return ServiceResult<object>.Success(
                    "Email verified successfully. Your account is pending admin approval.");
            }
            catch (FormatException)
            {
                // Token was not valid Base64
                return ServiceResult<object>.BadRequest(
                    "Invalid verification token format");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal(
                    "An unexpected error occurred",
                    new { message = ex.Message });
            }
        }

        // =========================================================
        // RESEND VERIFICATION EMAIL
        // =========================================================

        public async Task<ServiceResult<object>> ResendVerificationEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                // Always return success even if user not found
                // to prevent email enumeration attacks
                if (user == null)
                    return ServiceResult<object>.Success(
                        "If this email is registered, a verification link has been sent.");

                if (user.EmailConfirmed)
                    return ServiceResult<object>.Success("Email is already verified");

                if (!user.IsActive)
                    return ServiceResult<object>.Forbidden("Account is deactivated");

                // Generate and encode new token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                var verificationLink = $"{_appSettings.BaseUrl}/api/v1/auth/verify-email" +
                                       $"?userId={user.Id}&token={encodedToken}";

                var emailResult = await _emailService.SendVerificationEmailAsync(
                    user.Email!, user.UserName!, verificationLink);

                if (!emailResult.Response.Success)
                    return ServiceResult<object>.Internal(
                        "Failed to send verification email. Please try again later.");

                return ServiceResult<object>.Success(
                    "Verification email sent. Please check your inbox.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal(
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
