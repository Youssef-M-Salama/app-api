using App.Core.Domain.RepositoryContracts;
using App.Core.ServiceContracts;
using App.Core.Services;
using Microsoft.Extensions.Logging;
using App.Core.Enums;
using Moq;

namespace App.Services.Tests
{
    public class AdminServiceTests
    {
        private static Mock<IAdminRepository> CreateMockAdminRepo()
            => new Mock<IAdminRepository>();

        private static Mock<IEmailService> CreateMockEmailService()
            => new Mock<IEmailService>();

        private static AdminService CreateService(
            Mock<IAdminRepository>? adminRepo = null,
            Mock<IEmailService>? emailService = null)
            => new AdminService(
                (adminRepo ?? CreateMockAdminRepo()).Object,
                (emailService ?? CreateMockEmailService()).Object,
                new Mock<ILogger<AdminService>>().Object);

        [Fact]
        public async Task GetDashboardStatisticsAsync_ReturnsSuccess_WithCorrectValues()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.GetDashboardStatisticsAsync())
                .ReturnsAsync((PendingVerifications: 5, PendingCharityNeeds: 10, PendingOffers: 3, TotalUsers: 50, ActiveCharityNeeds: 20, ActiveOffers: 30));

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetDashboardStatisticsAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal("Dashboard statistics retrieved successfully", result.Response.Message);
            Assert.NotNull(result.Response.Data);
            Assert.Equal(5, result.Response.Data!.PendingVerifications);
            Assert.Equal(10, result.Response.Data.PendingCharityNeeds);
            Assert.Equal(50, result.Response.Data.TotalUsers);
            Assert.Equal(20, result.Response.Data.ActiveCharityNeeds);
            Assert.Equal(30, result.Response.Data.ActiveOffers);
        }

        [Fact]
        public async Task GetDashboardStatisticsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.GetDashboardStatisticsAsync())
                .ThrowsAsync(new Exception("DB error"));

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetDashboardStatisticsAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Response.Success);
            Assert.Equal("An unexpected error occurred", result.Response.Message);
        }

        [Fact]
        public async Task GetPendingVerificationsAsync_ReturnsSuccess_WithBothLists()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var fakeCharities = new List<App.Core.Domain.Entities.Charity>
            {
                new App.Core.Domain.Entities.Charity { CharityId = Guid.NewGuid(), CharityName = "Charity A", CreatedAt = DateTime.UtcNow }
            };
            var fakeDonors = new List<App.Core.Domain.Entities.DonorOrganization>
            {
                new App.Core.Domain.Entities.DonorOrganization { DonorOrganizationId = Guid.NewGuid(), DonorOrganizationName = "Donor B", CreatedAt = DateTime.UtcNow }
            };
            mockRepo.Setup(r => r.GetPendingVerificationsAsync())
                .ReturnsAsync((fakeCharities, fakeDonors));

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetPendingVerificationsAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.NotNull(result.Response.Data);
            Assert.Single(result.Response.Data!.PendingCharities);
            Assert.Single(result.Response.Data.PendingDonors);
            Assert.Equal("Charity A", result.Response.Data.PendingCharities.First().CharityName);
            Assert.Equal("Donor B", result.Response.Data.PendingDonors.First().DonorOrganizationName);
        }

        [Fact]
        public async Task GetPendingVerificationsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.GetPendingVerificationsAsync())
                .ThrowsAsync(new Exception("DB error"));

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetPendingVerificationsAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // ===== Verify User =====

        [Fact]
        public async Task VerifyUserAsync_ReturnsSuccess_AndSendsEmail()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var mockEmail = CreateMockEmailService();
            mockRepo.Setup(r => r.VerifyUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((true, "user@test.com", "TestUser"));

            var service = CreateService(mockRepo, mockEmail);

            // Act
            var result = await service.VerifyUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockEmail.Verify(e => e.SendAccountVerifiedAsync("user@test.com", "TestUser"), Times.Once);
        }

        [Fact]
        public async Task VerifyUserAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.VerifyUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((false, (string?)null, (string?)null));
            var service = CreateService(mockRepo);

            // Act
            var result = await service.VerifyUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // ===== Reject User =====

        [Fact]
        public async Task RejectUserAsync_ReturnsSuccess_AndSendsEmail()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var mockEmail = CreateMockEmailService();
            mockRepo.Setup(r => r.RejectUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((true, "user@test.com", "TestUser"));

            var service = CreateService(mockRepo, mockEmail);

            // Act
            var result = await service.RejectUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockEmail.Verify(e => e.SendAccountRejectedAsync("user@test.com", "TestUser"), Times.Once);
        }

        [Fact]
        public async Task RejectUserAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.RejectUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((false, (string?)null, (string?)null));
            var service = CreateService(mockRepo);

            // Act
            var result = await service.RejectUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // ===== Pending CharityNeeds =====

        [Fact]
        public async Task GetPendingCharityNeedsAsync_ReturnsSuccess_WithPagination()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var fakeNeeds = new List<App.Core.Domain.Entities.CharityNeed>
            {
                new App.Core.Domain.Entities.CharityNeed { CharityNeedId = Guid.NewGuid(), ProductName = "Bread", Category = ProductCategory.Food }
            };
            mockRepo.Setup(r => r.GetPendingCharityNeedsAsync(1, 10)).ReturnsAsync(fakeNeeds);
            mockRepo.Setup(r => r.CountPendingCharityNeedsAsync()).ReturnsAsync(1);

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetPendingCharityNeedsAsync(new App.Core.DTOs.Request.PendingRequestsFilterDTO { Page = 1, PageSize = 10 });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Single(result.Response.Data!);
            Assert.Equal(1, result.Response.Pagination!.TotalCount);
        }

        // ===== Approve CharityNeed =====

        [Fact]
        public async Task ApproveCharityNeedAsync_ReturnsSuccess_AndSendsEmail()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var mockEmail = CreateMockEmailService();
            mockRepo.Setup(r => r.ApproveCharityNeedAsync(It.IsAny<Guid>()))
                .ReturnsAsync((true, "charity@test.com", "CharityUser", "Rice"));

            var service = CreateService(mockRepo, mockEmail);

            // Act
            var result = await service.ApproveCharityNeedAsync(new App.Core.DTOs.Request.ActionCharityNeedRequestDTO { CharityNeedId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockEmail.Verify(e => e.SendCharityNeedApprovedAsync("charity@test.com", "CharityUser", "Rice"), Times.Once);
        }

        [Fact]
        public async Task ApproveCharityNeedAsync_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.ApproveCharityNeedAsync(It.IsAny<Guid>()))
                .ReturnsAsync((false, (string?)null, (string?)null, (string?)null));
            var service = CreateService(mockRepo);

            // Act
            var result = await service.ApproveCharityNeedAsync(new App.Core.DTOs.Request.ActionCharityNeedRequestDTO { CharityNeedId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // ===== Reject CharityNeed =====

        [Fact]
        public async Task RejectCharityNeedAsync_ReturnsSuccess_AndSendsEmail()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var mockEmail = CreateMockEmailService();
            mockRepo.Setup(r => r.RejectCharityNeedAsync(It.IsAny<Guid>()))
                .ReturnsAsync((true, "charity@test.com", "CharityUser", "Blankets"));

            var service = CreateService(mockRepo, mockEmail);

            // Act
            var result = await service.RejectCharityNeedAsync(new App.Core.DTOs.Request.ActionCharityNeedRequestDTO { CharityNeedId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockEmail.Verify(e => e.SendCharityNeedRejectedAsync("charity@test.com", "CharityUser", "Blankets"), Times.Once);
        }

        [Fact]
        public async Task RejectCharityNeedAsync_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.RejectCharityNeedAsync(It.IsAny<Guid>()))
                .ReturnsAsync((false, (string?)null, (string?)null, (string?)null));
            var service = CreateService(mockRepo);

            // Act
            var result = await service.RejectCharityNeedAsync(new App.Core.DTOs.Request.ActionCharityNeedRequestDTO { CharityNeedId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // ===== Users List =====

        [Fact]
        public async Task GetAllUsersAsync_ReturnsSuccess_WithFilters()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var fakeUsers = new List<App.Core.Domain.IdentityEntities.ApplicationUser>
            {
                new App.Core.Domain.IdentityEntities.ApplicationUser { Id = Guid.NewGuid(), Email = "admin@test.com", IsActive = true }
            };
            mockRepo.Setup(r => r.GetAllUsersAsync(null, null, 1, 10)).ReturnsAsync(fakeUsers);
            mockRepo.Setup(r => r.CountAllUsersAsync(null, null)).ReturnsAsync(1);

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetAllUsersAsync(new App.Core.DTOs.Request.UsersFilterRequestDTO { Page = 1, PageSize = 10 });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Single(result.Response.Data!);
            Assert.Equal("admin@test.com", result.Response.Data!.First().Email);
        }

        // ===== Deactivate / Activate User =====

        [Fact]
        public async Task DeactivateUserAsync_ReturnsSuccess_WhenUserFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.DeactivateUserAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            var service = CreateService(mockRepo);

            // Act
            var result = await service.DeactivateUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
        }

        [Fact]
        public async Task DeactivateUserAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.DeactivateUserAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            var service = CreateService(mockRepo);

            // Act
            var result = await service.DeactivateUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task ActivateUserAsync_ReturnsSuccess_WhenUserFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.ActivateUserAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            var service = CreateService(mockRepo);

            // Act
            var result = await service.ActivateUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
        }

        [Fact]
        public async Task ActivateUserAsync_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.ActivateUserAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            var service = CreateService(mockRepo);

            // Act
            var result = await service.ActivateUserAsync(new App.Core.DTOs.Request.ActionUserRequestDTO { UserId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // ===== Dashboard includes PendingOffers =====

        [Fact]
        public async Task GetDashboardStatisticsAsync_ReturnsPendingOffers()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.GetDashboardStatisticsAsync())
                .ReturnsAsync((PendingVerifications: 1, PendingCharityNeeds: 2, PendingOffers: 3, TotalUsers: 10, ActiveCharityNeeds: 4, ActiveOffers: 5));

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetDashboardStatisticsAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(3, result.Response.Data!.PendingOffers);
        }

        // ===== Pending Offers =====

        [Fact]
        public async Task GetPendingOffersAsync_ReturnsSuccess_WithPagination()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var fakeOffers = new List<App.Core.Domain.Entities.Offer>
            {
                new App.Core.Domain.Entities.Offer
                {
                    OfferId = Guid.NewGuid(),
                    ProductName = "Canned Food",
                    Category = ProductCategory.Food,
                    Quantity = 50,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    Status = App.Core.Enums.OfferStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                }
            };
            mockRepo.Setup(r => r.GetPendingOffersAsync(1, 10)).ReturnsAsync(fakeOffers);
            mockRepo.Setup(r => r.CountPendingOffersAsync()).ReturnsAsync(1);

            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetPendingOffersAsync(new App.Core.DTOs.Request.PendingRequestsFilterDTO { Page = 1, PageSize = 10 });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Single(result.Response.Data!);
            Assert.Equal(1, result.Response.Pagination!.TotalCount);
        }

        [Fact]
        public async Task GetPendingOffersAsync_ReturnsInvalidPage_WhenPageIsZero()
        {
            var service = CreateService();
            var result = await service.GetPendingOffersAsync(new App.Core.DTOs.Request.PendingRequestsFilterDTO { Page = 0, PageSize = 10 });
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetPendingOffersAsync_ReturnsInvalidPageSize_WhenPageSizeIsZero()
        {
            var service = CreateService();
            var result = await service.GetPendingOffersAsync(new App.Core.DTOs.Request.PendingRequestsFilterDTO { Page = 1, PageSize = 0 });
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetPendingOffersAsync_ReturnsPageSizeTooLarge_WhenPageSizeExceeds50()
        {
            var service = CreateService();
            var result = await service.GetPendingOffersAsync(new App.Core.DTOs.Request.PendingRequestsFilterDTO { Page = 1, PageSize = 51 });
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        // ===== Approve Offer =====

        [Fact]
        public async Task ApproveOfferAsync_ReturnsSuccess_AndSendsEmail()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var mockEmail = CreateMockEmailService();
            mockRepo.Setup(r => r.ApproveOfferAsync(It.IsAny<Guid>()))
                .ReturnsAsync((true, "donor@test.com", "DonorUser", "Canned Food"));

            var service = CreateService(mockRepo, mockEmail);

            // Act
            var result = await service.ApproveOfferAsync(new App.Core.DTOs.Request.ActionOfferRequestDTO { OfferId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockEmail.Verify(e => e.SendOfferApprovedAsync("donor@test.com", "DonorUser", "Canned Food"), Times.Once);
        }

        [Fact]
        public async Task ApproveOfferAsync_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.ApproveOfferAsync(It.IsAny<Guid>()))
                .ReturnsAsync((false, (string?)null, (string?)null, (string?)null));
            var service = CreateService(mockRepo);

            // Act
            var result = await service.ApproveOfferAsync(new App.Core.DTOs.Request.ActionOfferRequestDTO { OfferId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        // ===== Reject Offer =====

        [Fact]
        public async Task RejectOfferAsync_ReturnsSuccess_AndSendsEmail()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            var mockEmail = CreateMockEmailService();
            mockRepo.Setup(r => r.RejectOfferAsync(It.IsAny<Guid>()))
                .ReturnsAsync((true, "donor@test.com", "DonorUser", "Blankets"));

            var service = CreateService(mockRepo, mockEmail);

            // Act
            var result = await service.RejectOfferAsync(new App.Core.DTOs.Request.ActionOfferRequestDTO { OfferId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            mockEmail.Verify(e => e.SendOfferRejectedAsync("donor@test.com", "DonorUser", "Blankets"), Times.Once);
        }

        [Fact]
        public async Task RejectOfferAsync_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            var mockRepo = CreateMockAdminRepo();
            mockRepo.Setup(r => r.RejectOfferAsync(It.IsAny<Guid>()))
                .ReturnsAsync((false, (string?)null, (string?)null, (string?)null));
            var service = CreateService(mockRepo);

            // Act
            var result = await service.RejectOfferAsync(new App.Core.DTOs.Request.ActionOfferRequestDTO { OfferId = Guid.NewGuid() });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }
    }
}
