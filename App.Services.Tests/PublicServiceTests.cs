using App.Core.Domain.Entities;
using App.Core.Domain.IdentityEntities;
using App.Core.DTO.Request;
using App.Core.RepositoryContracts;
using App.Core.Services;
using Moq;

namespace App.Services.Tests
{
    public class PublicServiceTests
    {
        private static Mock<ICharityNeedRepository> CreateMockRepo()
            => new Mock<ICharityNeedRepository>();

        private static PublicService CreateService(Mock<ICharityNeedRepository> repo)
            => new PublicService(repo.Object);

        private static GetApprovedRequestsQueryDto CreateValidQuery(
            int page = 1, int pageSize = 10,
            string? category = null, string? search = null)
            => new() { Page = page, PageSize = pageSize, Category = category, Search = search };

        private static List<CharityNeed> CreateFakeNeeds(int count = 3)
        {
            var needs = new List<CharityNeed>();
            for (int i = 1; i <= count; i++)
                needs.Add(new CharityNeed
                {
                    CharityNeedId = Guid.NewGuid(),
                    Category = "food",
                    ProductName = $"Product {i}",
                    Quantity = i * 10,
                    Priority = "urgent",
                    Status = "approved",
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    Charity = new Charity
                    {
                        CharityName = $"Charity {i}",
                        ApplicationUser = new ApplicationUser { City = "Cairo", Governorate = "Cairo" }
                    }
                });
            return needs;
        }

        [Fact]
        public async Task GetApprovedRequestsAsync_ReturnsSuccess_WithCorrectPagination()
        {
            // Arrange
            var mockRepo = CreateMockRepo();
            mockRepo.Setup(r => r.GetApprovedRequestsAsync(null, null, 2, 5)).ReturnsAsync(CreateFakeNeeds(5));
            mockRepo.Setup(r => r.CountApprovedRequestsAsync(null, null)).ReturnsAsync(25);
            var service = CreateService(mockRepo);

            // Act
            var result = await service.GetApprovedRequestsAsync(CreateValidQuery(page: 2, pageSize: 5));

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
        public async Task GetApprovedRequestsAsync_ReturnsBadRequest_WhenPageInvalid()
        {
            var result = await CreateService(CreateMockRepo())
                .GetApprovedRequestsAsync(CreateValidQuery(page: 0));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedRequestsAsync_ReturnsBadRequest_WhenPageSizeInvalid()
        {
            var result = await CreateService(CreateMockRepo())
                .GetApprovedRequestsAsync(CreateValidQuery(pageSize: 0));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedRequestsAsync_ReturnsBadRequest_WhenPageSizeExceedsLimit()
        {
            var result = await CreateService(CreateMockRepo())
                .GetApprovedRequestsAsync(CreateValidQuery(pageSize: 51));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetApprovedRequestsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = CreateMockRepo();
            mockRepo.Setup(r => r.GetApprovedRequestsAsync(
                    It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await CreateService(mockRepo).GetApprovedRequestsAsync(CreateValidQuery());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        }
    }
}