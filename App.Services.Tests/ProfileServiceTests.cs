using App.Core.Domain.Entities;
using App.Core.Domain.IdentityEntities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using App.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace App.Services.Tests
{
    public class ProfileServiceTests
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

        private static Mock<IProfileRepository> CreateMockProfileRepository()
            => new Mock<IProfileRepository>();

        private static Mock<IFileService> CreateMockFileService()
        {
            var mock = new Mock<IFileService>();

            mock.Setup(f => f.BuildFullUrl(It.IsAny<string?>()))
                .Returns<string?>(path => path == null ? null : $"https://localhost:7007{path}");

            mock.Setup(f => f.SaveImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFolder>()))
                .ReturnsAsync(ServiceResult<string>.Success("Image uploaded successfully", "/images/users/test.jpg"));

            mock.Setup(f => f.DeleteImageAsync(It.IsAny<string?>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        private static ProfileService CreateService(
            Mock<UserManager<ApplicationUser>> userManager,
            Mock<IProfileRepository>? profileRepo = null,
            Mock<IFileService>? fileService = null)
            => new ProfileService(
                userManager.Object,
                (profileRepo ?? CreateMockProfileRepository()).Object,
                (fileService ?? CreateMockFileService()).Object);

        private static ApplicationUser CreateFakeUser(Guid? id = null) => new ApplicationUser
        {
            Id = id ?? Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            PhoneNumber = "01000000000",
            Whatsapp = "01000000000",
            City = "Cairo",
            Governorate = "Cairo",
            PostalCode = "11511",
            ImageUrl = "/images/users/old.jpg",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private static Charity CreateFakeCharity(Guid userId) => new Charity
        {
            CharityId = Guid.NewGuid(),
            UserId = userId,
            CharityName = "Hope Foundation",
            CharityDescription = "Helping those in need",
            IsVerified = true,
            IsActive = true
        };

        private static DonorOrganization CreateFakeDonor(Guid userId) => new DonorOrganization
        {
            DonorOrganizationId = Guid.NewGuid(),
            UserId = userId,
            DonorOrganizationName = "EgyFood Corp",
            DonorOrganizationDescription = "Food distribution organization",
            IsVerified = true,
            IsActive = true
        };

        private static Mock<IFormFile> CreateMockFormFile(string fileName = "test.jpg", long size = 1024 * 1024)
        {
            var mock = new Mock<IFormFile>();
            mock.Setup(f => f.FileName).Returns(fileName);
            mock.Setup(f => f.Length).Returns(size);
            return mock;
        }

        // =========================================================
        // GetProfileAsync
        // =========================================================

        [Fact]
        public async Task GetProfileAsync_ReturnsSuccess_ForCharityUser()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockProfileRepo = CreateMockProfileRepository();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Charity" });

            var charity = CreateFakeCharity(user.Id);
            mockProfileRepo.Setup(r => r.GetCharityByUserIdAsync(user.Id))
                .ReturnsAsync(charity);

            var service = CreateService(mockUserManager, mockProfileRepo);

            // Act
            var result = await service.GetProfileAsync(user.Id);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal("Charity", result.Response.Data!.Role);
            Assert.NotNull(result.Response.Data.CharityDetails);
            Assert.Null(result.Response.Data.DonorDetails);
            Assert.Equal(charity.CharityName, result.Response.Data.CharityDetails.CharityName);
            Assert.True(result.Response.Data.IsVerified);
        }

        [Fact]
        public async Task GetProfileAsync_ReturnsSuccess_ForDonorOrganizationUser()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockProfileRepo = CreateMockProfileRepository();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "DonorOrganization" });

            var donor = CreateFakeDonor(user.Id);
            mockProfileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(user.Id))
                .ReturnsAsync(donor);

            var service = CreateService(mockUserManager, mockProfileRepo);

            // Act
            var result = await service.GetProfileAsync(user.Id);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal("DonorOrganization", result.Response.Data!.Role);
            Assert.NotNull(result.Response.Data.DonorDetails);
            Assert.Null(result.Response.Data.CharityDetails);
            Assert.Equal(donor.DonorOrganizationName, result.Response.Data.DonorDetails.DonorOrganizationName);
            Assert.True(result.Response.Data.IsVerified);
        }

        [Fact]
        public async Task GetProfileAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.GetProfileAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task GetProfileAsync_ReturnsInternal_WhenExceptionThrown()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB error"));

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.GetProfileAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // =========================================================
        // UpdateProfileAsync
        // =========================================================

        [Fact]
        public async Task UpdateProfileAsync_ReturnsSuccess_WhenValidRequest()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.UpdateProfileAsync(user.Id, new UpdateProfileRequestDTO
            {
                Phone = "01111111111",
                City = "Alexandria",
                Governorate = "Alexandria"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockUserManager.Verify(m => m.UpdateAsync(
                It.Is<ApplicationUser>(u =>
                    u.PhoneNumber == "01111111111" &&
                    u.City == "Alexandria")), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.UpdateProfileAsync(Guid.NewGuid(), new UpdateProfileRequestDTO());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsValidationError_WhenUpdateFails()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Code = "UpdateFailed", Description = "Failed to update user" }));

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.UpdateProfileAsync(user.Id, new UpdateProfileRequestDTO
            {
                Phone = "01111111111"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
            Assert.NotNull(result.Response.Error?.Details);
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsInternal_WhenExceptionThrown()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB error"));

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.UpdateProfileAsync(Guid.NewGuid(), new UpdateProfileRequestDTO());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // =========================================================
        // ChangePasswordAsync
        // =========================================================

        [Fact]
        public async Task ChangePasswordAsync_ReturnsSuccess_WhenValidCredentials()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.ChangePasswordAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.ChangePasswordAsync(user.Id, new ChangePasswordRequestDTO
            {
                CurrentPassword = "OldPass@1",
                NewPassword = "NewPass@1",
                ConfirmPassword = "NewPass@1"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
        }

        [Fact]
        public async Task ChangePasswordAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.ChangePasswordAsync(Guid.NewGuid(), new ChangePasswordRequestDTO
            {
                CurrentPassword = "OldPass@1",
                NewPassword = "NewPass@1",
                ConfirmPassword = "NewPass@1"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task ChangePasswordAsync_ReturnsValidationError_WhenCurrentPasswordWrong()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.ChangePasswordAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Code = "PasswordMismatch", Description = "Incorrect password" }));

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.ChangePasswordAsync(user.Id, new ChangePasswordRequestDTO
            {
                CurrentPassword = "WrongPass@1",
                NewPassword = "NewPass@1",
                ConfirmPassword = "NewPass@1"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
            Assert.NotNull(result.Response.Error?.Details);
        }

        [Fact]
        public async Task ChangePasswordAsync_ReturnsInternal_WhenExceptionThrown()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB error"));

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.ChangePasswordAsync(Guid.NewGuid(), new ChangePasswordRequestDTO
            {
                CurrentPassword = "OldPass@1",
                NewPassword = "NewPass@1",
                ConfirmPassword = "NewPass@1"
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // =========================================================
        // UpdateProfileImageAsync
        // =========================================================

        [Fact]
        public async Task UpdateProfileImageAsync_ReturnsSuccess_WhenValidImage()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockFileService = CreateMockFileService();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(mockUserManager, fileService: mockFileService);

            // Act
            var result = await service.UpdateProfileImageAsync(user.Id, new UpdateProfileImageRequestDTO
            {
                Image = CreateMockFormFile().Object
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockFileService.Verify(f => f.DeleteImageAsync("/images/users/old.jpg"), Times.Once);
            mockFileService.Verify(f => f.SaveImageAsync(
                It.IsAny<IFormFile>(), ImageFolder.Users), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileImageAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.UpdateProfileImageAsync(Guid.NewGuid(), new UpdateProfileImageRequestDTO
            {
                Image = CreateMockFormFile().Object
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task UpdateProfileImageAsync_ReturnsBadRequest_WhenImageValidationFails()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockFileService = CreateMockFileService();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockFileService.Setup(f => f.SaveImageAsync(
                    It.IsAny<IFormFile>(), It.IsAny<ImageFolder>()))
                .ReturnsAsync(ServiceResult<string>.BadRequest("Invalid image format. Allowed: .jpg, .jpeg, .png, .webp"));

            var service = CreateService(mockUserManager, fileService: mockFileService);

            // Act
            var result = await service.UpdateProfileImageAsync(user.Id, new UpdateProfileImageRequestDTO
            {
                Image = CreateMockFormFile("test.pdf").Object
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task UpdateProfileImageAsync_RollbacksNewImage_WhenUpdateFails()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            var mockFileService = CreateMockFileService();
            var user = CreateFakeUser();

            mockUserManager.Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Code = "UpdateFailed", Description = "Failed to update user" }));

            var service = CreateService(mockUserManager, fileService: mockFileService);

            // Act
            var result = await service.UpdateProfileImageAsync(user.Id, new UpdateProfileImageRequestDTO
            {
                Image = CreateMockFormFile().Object
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Response.Success);

            // Verify rollback — new image deleted
            mockFileService.Verify(f => f.DeleteImageAsync("/images/users/test.jpg"), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileImageAsync_ReturnsInternal_WhenExceptionThrown()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB error"));

            var service = CreateService(mockUserManager);

            // Act
            var result = await service.UpdateProfileImageAsync(Guid.NewGuid(), new UpdateProfileImageRequestDTO
            {
                Image = CreateMockFormFile().Object
            });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Response.Success);
        }
    }
}