using App.Core.Domain.IdentityEntities;
using App.Core.DTO.Request;
using App.Core.ServiceContracts;
using App.Core.Services;
using App.Services.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace App.Services.Tests
{
    public class AccountServiceTests
    {
        private static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, // IOptions<IdentityOptions>
                null, // IPasswordHasher<TUser>
                null, // IEnumerable<IUserValidator<TUser>>
                null, // IEnumerable<IPasswordValidator<TUser>>
                null, // ILookupNormalizer
                null, // IdentityErrorDescriber
                null, // IServiceProvider
                null  // ILogger<UserManager<TUser>>
            );
        }

        private static Mock<RoleManager<ApplicationRole>> CreateMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<ApplicationRole>>();
            var mock = new Mock<RoleManager<ApplicationRole>>(
                roleStore.Object,
                null, // IEnumerable<IRoleValidator<T>>
                null, // ILookupNormalizer
                null, // IdentityErrorDescriber
                null  // ILogger<RoleManager<T>>
            );

            // Default: role exists. Tests can override when needed.
            mock.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            return mock;
        }

        private static Mock<SignInManager<ApplicationUser>> CreateMockSignInManager(UserManager<ApplicationUser> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var principalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            return new Mock<SignInManager<ApplicationUser>>(
                userManager,
                contextAccessor.Object,
                principalFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object);
        }

        private static AccountService CreateService(
            Mock<UserManager<ApplicationUser>> userManager,
            Mock<RoleManager<ApplicationRole>> roleManager,
            Mock<SignInManager<ApplicationUser>> signInManager,
            Mock<IJwtService> jwt)
        {
            return new AccountService(
                userManager.Object,
                roleManager.Object,
                signInManager.Object,
                jwt.Object);
        }

        private static RegisterDTO CreateValidRegisterDto()
            => new()
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Phone = "0123456789",
                Password = "P@ssword1",
                ConfirmPassword = "P@ssword1",
                AccountType = "charity",
                Name = "Test"
            };

        [Fact]
        public async Task RegisterAsync_ReturnsCreated_OnSuccess()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager
                .Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager
                .Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Charity" });

            mockJwt.Setup(j => j.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                   .Returns("test-token");

            mockJwt.Setup(j => j.GetTokenExpirationMinutes()).Returns(60);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            var dto = CreateValidRegisterDto();

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(result.Response);
            Assert.True(result.Response.Success);
            Assert.NotNull(result.Response.Data);
            Assert.Equal("test-token", result.Response.Data.Token);
            Assert.Equal("Charity", result.Response.Data.Role);

            mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Charity"), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsValidationError_OnIdentityCreateFailure()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager(); // default RoleExistsAsync -> true
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            var identityError = new IdentityError { Code = "DuplicateEmail", Description = "Email already exists" };
            mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            var dto = new RegisterDTO
            {
                Username = "charity3",
                Email = "charity3@example.com",
                Phone = "0123456789",
                Password = "P@ssword1",
                ConfirmPassword = "P@ssword1",
                AccountType = "charity",
                Name = "Test"
            };

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
            Assert.NotNull(result.Response.Error);
            Assert.NotNull(result.Response.Error.Details);
        }

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
                IsActive = true
            };

            mockUserManager
                .Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            mockSignInManager
                .Setup(s => s.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);

            mockUserManager
                .Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "DonorOrganization" });

            mockJwt.Setup(j => j.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                   .Returns("login-token");
            mockJwt.Setup(j => j.GetTokenExpirationMinutes()).Returns(30);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            var dto = new LoginDTO
            {
                UsernameOrEmail = "testuser",
                Password = "P@ssword1",
                RememberMe = false
            };

            // Act
            var result = await service.LoginAsync(dto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal("login-token", result.Response.Data.Token);
            Assert.Equal("DonorOrganization", result.Response.Data.Role);
        }

        [Fact]
        public async Task LoginAsync_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockRoleManager = CreateMockRoleManager();
            var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);
            var mockJwt = new Mock<IJwtService>();

            mockUserManager
                .Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            mockUserManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            var dto = new LoginDTO
            {
                UsernameOrEmail = "notfound",
                Password = "whatever"
            };

            // Act
            var result = await service.LoginAsync(dto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
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
                IsActive = false // inactive user
            };

            mockUserManager
                .Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            mockSignInManager
                .Setup(s => s.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);

            var service = CreateService(mockUserManager, mockRoleManager, mockSignInManager, mockJwt);

            var dto = new LoginDTO
            {
                UsernameOrEmail = "testuser",
                Password = "P@ssword1"
            };

            // Act
            var result = await service.LoginAsync(dto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, result.StatusCode);
            Assert.False(result.Response.Success);
        }
    }
}