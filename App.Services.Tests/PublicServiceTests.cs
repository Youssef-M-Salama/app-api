using App.Core.Domain.Entities;
using App.Core.Domain.IdentityEntities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.Enums;
using App.Core.Services;
using App.Core.ServiceContracts;
using Moq;
using Microsoft.Extensions.Logging;

namespace App.Services.Tests
{
    public class PublicServiceTests
    {
        // =========================================================
        // HELPERS
        // =========================================================

        private static Mock<ICharityNeedRepository> CreateMockCharityNeedRepo()
            => new Mock<ICharityNeedRepository>();

        private static Mock<IOfferRepository> CreateMockOfferRepo()
            => new Mock<IOfferRepository>();

        private static Mock<ICharityRepository> CreateMockCharityRepo()
            => new Mock<ICharityRepository>();

        private static Mock<IDonorOrganizationRepository> CreateMockDonorRepo()
            => new Mock<IDonorOrganizationRepository>();

        private static Mock<IFileService> CreateMockFileService()
        {
            var mock = new Mock<IFileService>();
            mock.Setup(f => f.GetImageUrl(It.IsAny<string?>()))
                .Returns<string?>(path => path);
            return mock;
        }

        private static PublicService CreateService(
            Mock<ICharityNeedRepository>? charityNeedRepo = null,
            Mock<IOfferRepository>? offerRepo = null,
            Mock<ICharityRepository>? charityRepo = null,
            Mock<IDonorOrganizationRepository>? donorRepo = null,
            Mock<IFileService>? fileService = null)
            => new PublicService(
                (charityNeedRepo ?? CreateMockCharityNeedRepo()).Object,
                (offerRepo ?? CreateMockOfferRepo()).Object,
                (charityRepo ?? CreateMockCharityRepo()).Object,
                (donorRepo ?? CreateMockDonorRepo()).Object,
                (fileService ?? CreateMockFileService()).Object,
                new Mock<ILogger<PublicService>>().Object);

        // =========================================================
        // FAKE DATA HELPERS
        // =========================================================

        private static ApprovedCharityNeedsRequestDTO CreateValidCharityNeedsQuery(
            int page = 1, int pageSize = 10,
            ProductCategory? category = null, string? city = null,
            string? governorate = null, string? search = null)
            => new()
            {
                Page = page,
                PageSize = pageSize,
                Category = category,
                City = city,
                Governorate = governorate,
                Search = search
            };

        private static ApprovedOffersRequestDTO CreateValidOffersQuery(
            int page = 1, int pageSize = 10,
            ProductCategory? category = null, string? city = null,
            string? governorate = null, string? search = null)
            => new()
            {
                Page = page,
                PageSize = pageSize,
                Category = category,
                City = city,
                Governorate = governorate,
                Search = search
            };

        private static List<CharityNeed> CreateFakeNeeds(int count = 3)
        {
            var needs = new List<CharityNeed>();
            for (int i = 1; i <= count; i++)
                needs.Add(new CharityNeed
                {
                    CharityNeedId = Guid.NewGuid(),
                    Category = ProductCategory.Food,
                    ProductName = $"Product {i}",
                    Quantity = i * 10,
                    Priority = CharityNeedPriority.Urgent,
                    Status = CharityNeedStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    Charity = new Charity
                    {
                        CharityName = $"Charity {i}",
                        ApplicationUser = new ApplicationUser
                        {
                            City = "Cairo",
                            Governorate = "Cairo"
                        }
                    }
                });
            return needs;
        }

        private static List<Offer> CreateFakeOffers(int count = 3)
        {
            var offers = new List<Offer>();
            for (int i = 1; i <= count; i++)
                offers.Add(new Offer
                {
                    OfferId = Guid.NewGuid(),
                    Category = ProductCategory.Food,
                    ProductName = $"Offer Product {i}",
                    Quantity = i * 20,
                    ExpiryDate = DateTime.UtcNow.AddMonths(i),
                    Status = OfferStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    DonorOrganization = new DonorOrganization
                    {
                        DonorOrganizationName = $"Donor {i}",
                        ApplicationUser = new ApplicationUser
                        {
                            City = "Cairo",
                            Governorate = "Cairo"
                        }
                    }
                });
            return offers;
        }

        private static CharityNeed CreateFakeNeed()
            => new CharityNeed
            {
                CharityNeedId = Guid.NewGuid(),
                Category = ProductCategory.Food,
                ProductName = "Rice Bags",
                Quantity = 100,
                Priority = CharityNeedPriority.Urgent,
                Status = CharityNeedStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Charity = new Charity
                {
                    CharityName = "Hope Foundation",
                    ApplicationUser = new ApplicationUser
                    {
                        City = "Cairo",
                        Governorate = "Cairo"
                    }
                }
            };

        private static Offer CreateFakeOffer()
            => new Offer
            {
                OfferId = Guid.NewGuid(),
                Category = ProductCategory.Food,
                ProductName = "Pasta Boxes",
                Quantity = 500,
                ExpiryDate = DateTime.UtcNow.AddMonths(3),
                Status = OfferStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                DonorOrganization = new DonorOrganization
                {
                    DonorOrganizationName = "EgyFood Corp",
                    ApplicationUser = new ApplicationUser
                    {
                        City = "Cairo",
                        Governorate = "Cairo"
                    }
                }
            };

        // =========================================================
        // GetApprovedCharityNeedsAsync
        // =========================================================

        [Fact]
        public async Task GetApprovedCharityNeedsAsync_ReturnsSuccess_WithCorrectPagination()
        {
            // Arrange
            var mockRepo = CreateMockCharityNeedRepo();
            mockRepo
                .Setup(r => r.GetApprovedCharityNeedsAsync(null, null, null, null, 2, 5))
                .ReturnsAsync(CreateFakeNeeds(5));
            mockRepo
                .Setup(r => r.CountApprovedCharityNeedsAsync(null, null, null, null))
                .ReturnsAsync(25);

            var service = CreateService(charityNeedRepo: mockRepo);

            // Act
            var result = await service.GetApprovedCharityNeedsAsync(
                CreateValidCharityNeedsQuery(page: 2, pageSize: 5));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal(5, result.Response.Data!.Count());
            Assert.Equal(2, result.Response.Pagination!.Page);
            Assert.Equal(25, result.Response.Pagination.TotalCount);
            Assert.Equal(5, result.Response.Pagination.TotalPages);
            Assert.True(result.Response.Pagination.HasNext);
            Assert.True(result.Response.Pagination.HasPrevious);
        }

        [Fact]
        public async Task GetApprovedCharityNeedsAsync_ReturnsBadRequest_WhenPageInvalid()
        {
            var result = await CreateService()
                .GetApprovedCharityNeedsAsync(CreateValidCharityNeedsQuery(page: 0));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedCharityNeedsAsync_ReturnsBadRequest_WhenPageSizeInvalid()
        {
            var result = await CreateService()
                .GetApprovedCharityNeedsAsync(CreateValidCharityNeedsQuery(pageSize: 0));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedCharityNeedsAsync_ReturnsBadRequest_WhenPageSizeExceedsLimit()
        {
            var result = await CreateService()
                .GetApprovedCharityNeedsAsync(CreateValidCharityNeedsQuery(pageSize: 51));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedCharityNeedsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = CreateMockCharityNeedRepo();
            mockRepo
                .Setup(r => r.GetApprovedCharityNeedsAsync(
                    It.IsAny<ProductCategory?>(), It.IsAny<string?>(),
                    It.IsAny<string?>(), It.IsAny<string?>(),
                    It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await CreateService(charityNeedRepo: mockRepo)
                .GetApprovedCharityNeedsAsync(CreateValidCharityNeedsQuery());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GetApprovedCharityNeedByIdAsync
        // =========================================================

        [Fact]
        public async Task GetApprovedCharityNeedByIdAsync_ReturnsSuccess_WhenFound()
        {
            // Arrange
            var mockRepo = CreateMockCharityNeedRepo();
            var fakeNeed = CreateFakeNeed();
            mockRepo
                .Setup(r => r.GetApprovedCharityNeedByIdAsync(fakeNeed.CharityNeedId))
                .ReturnsAsync(fakeNeed);

            // Act
            var result = await CreateService(charityNeedRepo: mockRepo)
                .GetApprovedCharityNeedByIdAsync(fakeNeed.CharityNeedId);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal(fakeNeed.ProductName, result.Response.Data!.ProductName);
            Assert.Equal(fakeNeed.CharityNeedId, result.Response.Data.CharityNeedId);
        }

        [Fact]
        public async Task GetApprovedCharityNeedByIdAsync_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockRepo = CreateMockCharityNeedRepo();
            mockRepo
                .Setup(r => r.GetApprovedCharityNeedByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((CharityNeed?)null);

            // Act
            var result = await CreateService(charityNeedRepo: mockRepo)
                .GetApprovedCharityNeedByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task GetApprovedCharityNeedByIdAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = CreateMockCharityNeedRepo();
            mockRepo
                .Setup(r => r.GetApprovedCharityNeedByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await CreateService(charityNeedRepo: mockRepo)
                .GetApprovedCharityNeedByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GetApprovedOffersAsync
        // =========================================================

        [Fact]
        public async Task GetApprovedOffersAsync_ReturnsSuccess_WithCorrectPagination()
        {
            // Arrange
            var mockRepo = CreateMockOfferRepo();
            mockRepo
                .Setup(r => r.GetApprovedOffersAsync(null, null, null, null, 2, 5))
                .ReturnsAsync(CreateFakeOffers(5));
            mockRepo
                .Setup(r => r.CountApprovedOffersAsync(null, null, null, null))
                .ReturnsAsync(25);

            var service = CreateService(offerRepo: mockRepo);

            // Act
            var result = await service.GetApprovedOffersAsync(
                CreateValidOffersQuery(page: 2, pageSize: 5));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal(5, result.Response.Data!.Count());
            Assert.Equal(2, result.Response.Pagination!.Page);
            Assert.Equal(25, result.Response.Pagination.TotalCount);
            Assert.Equal(5, result.Response.Pagination.TotalPages);
            Assert.True(result.Response.Pagination.HasNext);
            Assert.True(result.Response.Pagination.HasPrevious);
        }

        [Fact]
        public async Task GetApprovedOffersAsync_ReturnsBadRequest_WhenPageInvalid()
        {
            var result = await CreateService()
                .GetApprovedOffersAsync(CreateValidOffersQuery(page: 0));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedOffersAsync_ReturnsBadRequest_WhenPageSizeInvalid()
        {
            var result = await CreateService()
                .GetApprovedOffersAsync(CreateValidOffersQuery(pageSize: 0));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedOffersAsync_ReturnsBadRequest_WhenPageSizeExceedsLimit()
        {
            var result = await CreateService()
                .GetApprovedOffersAsync(CreateValidOffersQuery(pageSize: 51));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedOffersAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = CreateMockOfferRepo();
            mockRepo
                .Setup(r => r.GetApprovedOffersAsync(
                    It.IsAny<ProductCategory?>(), It.IsAny<string?>(),
                    It.IsAny<string?>(), It.IsAny<string?>(),
                    It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await CreateService(offerRepo: mockRepo)
                .GetApprovedOffersAsync(CreateValidOffersQuery());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GetApprovedOfferByIdAsync
        // =========================================================

        [Fact]
        public async Task GetApprovedOfferByIdAsync_ReturnsSuccess_WhenFound()
        {
            // Arrange
            var mockRepo = CreateMockOfferRepo();
            var fakeOffer = CreateFakeOffer();
            mockRepo
                .Setup(r => r.GetApprovedOfferByIdAsync(fakeOffer.OfferId))
                .ReturnsAsync(fakeOffer);

            // Act
            var result = await CreateService(offerRepo: mockRepo)
                .GetApprovedOfferByIdAsync(fakeOffer.OfferId);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal(fakeOffer.ProductName, result.Response.Data!.ProductName);
            Assert.Equal(fakeOffer.OfferId, result.Response.Data.OfferId);
        }

        [Fact]
        public async Task GetApprovedOfferByIdAsync_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockRepo = CreateMockOfferRepo();
            mockRepo
                .Setup(r => r.GetApprovedOfferByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Offer?)null);

            // Act
            var result = await CreateService(offerRepo: mockRepo)
                .GetApprovedOfferByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task GetApprovedOfferByIdAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = CreateMockOfferRepo();
            mockRepo
                .Setup(r => r.GetApprovedOfferByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await CreateService(offerRepo: mockRepo)
                .GetApprovedOfferByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GetStatisticsAsync
        // =========================================================

        [Fact]
        public async Task GetStatisticsAsync_ReturnsSuccess_WithCorrectValues()
        {
            // Arrange
            var mockCharityNeedRepo = CreateMockCharityNeedRepo();
            var mockOfferRepo = CreateMockOfferRepo();
            var mockCharityRepo = CreateMockCharityRepo();
            var mockDonorRepo = CreateMockDonorRepo();

            mockCharityNeedRepo.Setup(r => r.CountFulfilledCharityNeedsAsync()).ReturnsAsync(3);
            mockCharityNeedRepo.Setup(r => r.CountActiveCharityNeedsAsync()).ReturnsAsync(10);
            mockCharityNeedRepo.Setup(r => r.SumFulfilledCharityNeedsQuantityAsync()).ReturnsAsync(150);
            mockOfferRepo.Setup(r => r.CountFulfilledOffersAsync()).ReturnsAsync(2);
            mockOfferRepo.Setup(r => r.CountActiveOffersAsync()).ReturnsAsync(8);
            mockOfferRepo.Setup(r => r.SumFulfilledOffersQuantityAsync()).ReturnsAsync(100);
            mockCharityRepo.Setup(r => r.CountTotalCharitiesAsync()).ReturnsAsync(5);
            mockDonorRepo.Setup(r => r.CountTotalDonorsAsync()).ReturnsAsync(7);

            var service = CreateService(
                mockCharityNeedRepo, mockOfferRepo,
                mockCharityRepo, mockDonorRepo);

            // Act
            var result = await service.GetStatisticsAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Equal(5, result.Response.Data!.TotalDonations);
            Assert.Equal(5, result.Response.Data!.TotalCharities);
            Assert.Equal(7, result.Response.Data!.TotalDonors);
            Assert.Equal(10, result.Response.Data!.ActiveCharityNeeds);
            Assert.Equal(8, result.Response.Data!.ActiveOffers);
            Assert.Equal(250, result.Response.Data!.TotalItemsDonated);
        }

        [Fact]
        public async Task GetStatisticsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockCharityNeedRepo = CreateMockCharityNeedRepo();
            mockCharityNeedRepo
                .Setup(r => r.CountFulfilledCharityNeedsAsync())
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await CreateService(charityNeedRepo: mockCharityNeedRepo)
                .GetStatisticsAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        }
    }
}