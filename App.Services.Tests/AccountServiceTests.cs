using App.Core.Domain.IdentityEntities;
using App.Core.DTOs.Request;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using App.Core.Services;
using App.Core.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;

namespace App.Services.Tests
{
    public class AccountServiceTests
    {
        // =========================================================
        // HELPERS
        // =========================================================

        private static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private static Mock<RoleManager<ApplicationRole>> CreateMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<ApplicationRole>>();
            var mock = new Mock<RoleManager<ApplicationRole>>(
                roleStore.Object, null, null, null, null);

            mock.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            return mock;
        }

        private static Mock<SignInManager<ApplicationUser>> CreateMockSignInManager(
            UserManager<ApplicationUser> userManager)
        {
            return new Mock<SignInManager<ApplicationUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object);
        }

        private static Mock<IEmailService> CreateMockEmailService()
        {
            var mock = new Mock<IEmailService>();

            // Default — email sends successfully
            mock.Setup(e => e.SendVerificationEmailAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceResult<object>.Success("Email sent successfully"));

            mock.Setup(e => e.SendEmailVerifiedAsync(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceResult<object>.Success("Email sent successfully"));

            return mock;
        }

        private static IOptions<AppSettings> CreateAppSettings()
            => Options.Create(new AppSettings { BaseUrl = "https://localhost:7007" });

        private static AccountService CreateService(
            Mock<UserManager<ApplicationUser>> userManager,
            Mock<RoleManager<ApplicationRole>> roleManager,
            Mock<SignInManager<ApplicationUser>> signInManager,
            Mock<IJwtService> jwt,
            Mock<IEmailService>? emailService = null)
            => new AccountService(
                userManager.Object,
                roleManager.Object,
                signInManager.Object,
                jwt.Object,
                (emailService ?? CreateMockEmailService()).Object,
                CreateAppSettings());

        private static RegisterDTO CreateValidRegisterDto()
            => new()
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Phone = "0123456789",
                Password = "P@ssword1",
                ConfirmPassword = "P@ssword1",
                AccountType = AccountType.Charity,
                Name = "Test"
            };

        private static void SetupJwt(Mock<IJwtService> mockJwt,
            string token = "test-token", string refreshToken = "rt-1")
        {
            mockJwt.Setup(j => j.GenerateToken(
                    It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                .Returns(token);
            mockJwt.Setup(j => j.GetTokenExpirationMinutes()).Returns(60);
            mockJwt.Setup(j => j.GenerateRefreshToken()).Returns(refreshToken);
            mockJwt.Setup(j => j.GetRefreshTokenExpirationDays()).Returns(7);
        }

        // =========================================================
        // RegisterAsync
        // =========================================================

        [Fact]
        public async Task RegisterAsync_ReturnsCreated_OnSuccess()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByNameAsync("testuser"))
                .ReturnsAsync((ApplicationUser?)null);
            mockUserManager.Setup(m => m.FindByEmailAsync("testuser@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            mockUserManager.Setup(m => m.CreateAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(
                    It.IsAny<ApplicationUser>()))
                .ReturnsAsync("email-confirmation-token");

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.RegisterAsync(CreateValidRegisterDto());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Contains("verify your account", result.Response.Message);

            mockUserManager.Verify(m => m.AddToRoleAsync(
                It.IsAny<ApplicationUser>(), "Charity"), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsCreated_WithWarning_WhenEmailFails()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();
            var mockEmail = new Mock<IEmailService>();

            mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            mockUserManager.Setup(m => m.CreateAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(
                    It.IsAny<ApplicationUser>()))
                .ReturnsAsync("email-confirmation-token");

            // Email sending fails
            mockEmail.Setup(e => e.SendVerificationEmailAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceResult<object>.Internal("SMTP error"));

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt, mockEmail);

            // Act
            var result = await service.RegisterAsync(CreateValidRegisterDto());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Contains("resend verification", result.Response.Message);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsValidationError_OnIdentityCreateFailure()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            mockUserManager.Setup(m => m.CreateAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Code = "DuplicateEmail", Description = "Email already exists" }));

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.RegisterAsync(new RegisterDTO
            {
                Username = "charity3",
                Email = "charity3@example.com",
                Phone = "0123456789",
                Password = "P@ssword1",
                ConfirmPassword = "P@ssword1",
                AccountType = AccountType.Charity,
                Name = "Test"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
            Assert.NotNull(result.Response.Error);
            Assert.NotNull(result.Response.Error.Details);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsValidationError_WhenUsernameAlreadyTaken()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByNameAsync("testuser"))
                .ReturnsAsync(new ApplicationUser { UserName = "testuser" });
            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.RegisterAsync(CreateValidRegisterDto());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsValidationError_WhenEmailAlreadyTaken()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByNameAsync("testuser"))
                .ReturnsAsync((ApplicationUser?)null);
            mockUserManager.Setup(m => m.FindByEmailAsync("testuser@example.com"))
                .ReturnsAsync(new ApplicationUser { Email = "testuser@example.com" });

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.RegisterAsync(CreateValidRegisterDto());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // =========================================================
        // LoginAsync
        // =========================================================

        [Fact]
        public async Task LoginAsync_ReturnsSuccess_OnValidCredentials()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                IsActive = true,
                EmailConfirmed = true
            };

            mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            mockSignInManager.Setup(s => s.CheckPasswordSignInAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);

            mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "DonorOrganization" });

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            SetupJwt(mockJwt, token: "login-token", refreshToken: "rt-login");

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.LoginAsync(new LoginDTO
            {
                UsernameOrEmail = "testuser",
                Password = "P@ssword1"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal("login-token", result.Response.Data.Token);
            Assert.Equal("DonorOrganization", result.Response.Data.Role);
            Assert.Equal("rt-login", result.Response.Data.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.LoginAsync(new LoginDTO
            {
                UsernameOrEmail = "notfound",
                Password = "whatever"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task LoginAsync_ReturnsForbidden_WhenEmailNotConfirmed()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                IsActive = true,
                EmailConfirmed = false
            };

            mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            mockSignInManager.Setup(s => s.CheckPasswordSignInAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.NotAllowed);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.LoginAsync(new LoginDTO
            {
                UsernameOrEmail = "testuser",
                Password = "P@ssword1"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task LoginAsync_ReturnsForbidden_WhenUserIsInactive()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                IsActive = false,
                EmailConfirmed = true
            };

            mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            mockSignInManager.Setup(s => s.CheckPasswordSignInAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.LoginAsync(new LoginDTO
            {
                UsernameOrEmail = "testuser",
                Password = "P@ssword1"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // =========================================================
        // VerifyEmailAsync
        // =========================================================

        [Fact]
        public async Task VerifyEmailAsync_ReturnsSuccess_WhenValidToken()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                EmailConfirmed = false
            };

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.ConfirmEmailAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Build valid Base64 token
            var rawToken = "valid-token";
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawToken));

            // Act
            var result = await service.VerifyEmailAsync(user.Id, encodedToken);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Contains("verified successfully", result.Response.Message);
        }

        [Fact]
        public async Task VerifyEmailAsync_ReturnsBadRequest_WhenTokenInvalid()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                EmailConfirmed = false
            };

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.ConfirmEmailAsync(
                    It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Code = "InvalidToken", Description = "Invalid token" }));

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalid-token"));

            // Act
            var result = await service.VerifyEmailAsync(user.Id, encodedToken);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task VerifyEmailAsync_ReturnsBadRequest_WhenTokenNotBase64()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                EmailConfirmed = false
            };

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act — pass a non-Base64 string
            var result = await service.VerifyEmailAsync(user.Id, "not-valid-base64!!!");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task VerifyEmailAsync_ReturnsSuccess_WhenEmailAlreadyVerified()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                EmailConfirmed = true // already verified
            };

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.VerifyEmailAsync(user.Id,
                Convert.ToBase64String(Encoding.UTF8.GetBytes("any-token")));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Contains("already verified", result.Response.Message);
        }

        [Fact]
        public async Task VerifyEmailAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.VerifyEmailAsync(Guid.NewGuid(),
                Convert.ToBase64String(Encoding.UTF8.GetBytes("any-token")));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // =========================================================
        // ResendVerificationEmailAsync
        // =========================================================

        [Fact]
        public async Task ResendVerificationEmailAsync_ReturnsSuccess_WhenEmailSent()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                IsActive = true,
                EmailConfirmed = false
            };

            mockUserManager.Setup(m => m.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(
                    It.IsAny<ApplicationUser>()))
                .ReturnsAsync("new-token");

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.ResendVerificationEmailAsync("test@example.com");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Contains("check your inbox", result.Response.Message);
        }

        [Fact]
        public async Task ResendVerificationEmailAsync_ReturnsSuccess_WhenUserNotFound()
        {
            // Arrange — prevent email enumeration
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.ResendVerificationEmailAsync("unknown@example.com");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
        }

        [Fact]
        public async Task ResendVerificationEmailAsync_ReturnsSuccess_WhenAlreadyVerified()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                IsActive = true,
                EmailConfirmed = true
            };

            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.ResendVerificationEmailAsync("test@example.com");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Contains("already verified", result.Response.Message);
        }

        // =========================================================
        // RefreshTokenAsync
        // =========================================================

        [Fact]
        public async Task RefreshTokenAsync_ReturnsSuccess_WhenValidRefreshToken()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "rt-user",
                Email = "rt@example.com",
                IsActive = true,
                RefreshToken = "valid-refresh",
                RefreshTokenExpiration = DateTime.UtcNow.AddHours(1)
            };

            mockUserManager.Setup(m => m.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable());

            mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Charity" });

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            SetupJwt(mockJwt, token: "new-access-token", refreshToken: "rotated-refresh");

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.RefreshTokenAsync(
                new RefreshTokenDTO { RefreshToken = "valid-refresh" });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal("new-access-token", result.Response.Data.Token);
            Assert.Equal("rotated-refresh", result.Response.Data.RefreshToken);
            mockUserManager.Verify(m => m.UpdateAsync(
                It.IsAny<ApplicationUser>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsUnauthorized_WhenTokenNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.Users)
                .Returns(new List<ApplicationUser>().AsQueryable());

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.RefreshTokenAsync(
                new RefreshTokenDTO { RefreshToken = "missing" });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsUnauthorized_WhenTokenExpired()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "rt-user",
                Email = "rt@example.com",
                IsActive = true,
                RefreshToken = "expired-refresh",
                RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(-5)
            };

            mockUserManager.Setup(m => m.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable());

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.RefreshTokenAsync(
                new RefreshTokenDTO { RefreshToken = "expired-refresh" });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.False(result.Response.Success);
            mockUserManager.Verify(m => m.UpdateAsync(
                It.Is<ApplicationUser>(u => u.RefreshToken == null)), Times.Once);
        }

        // =========================================================
        // LogoutAsync
        // =========================================================

        [Fact]
        public async Task LogoutAsync_ReturnsSuccess_OnValidUser()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "logout-user",
                Email = "lo@example.com",
                RefreshToken = "some-rt",
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(1)
            };

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.LogoutAsync(user.Id);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockUserManager.Verify(m => m.UpdateAsync(
                It.Is<ApplicationUser>(u =>
                    u.RefreshToken == null &&
                    u.RefreshTokenExpiration == null)), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsNotFound_WhenUserMissing()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            // Act
            var result = await service.LogoutAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }
    }
}