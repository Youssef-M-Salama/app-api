using System.Net;
using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using App.Core.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace App.Services.Tests
{
    public class CharityServiceTests
    {
        // =========================================================
        // HELPERS — factories
        // =========================================================

        private static Mock<ICharityRepository> MockCharityRepo() => new();
        private static Mock<ICharityNeedRepository> MockCharityNeedRepo() => new();
        private static Mock<INeedApplicationRepository> MockNeedAppRepo() => new();
        private static Mock<IOfferApplicationRepository> MockOfferAppRepo() => new();
        private static Mock<IOfferRepository> MockOfferRepo() => new();
        private static Mock<IProfileRepository> MockProfileRepo() => new();
        private static Mock<IFileService> MockFileService() => new();

        private static CharityService CreateService(
            Mock<ICharityRepository>? charityRepo = null,
            Mock<ICharityNeedRepository>? charityNeedRepo = null,
            Mock<INeedApplicationRepository>? needAppRepo = null,
            Mock<IOfferApplicationRepository>? offerAppRepo = null,
            Mock<IOfferRepository>? offerRepo = null,
            Mock<IProfileRepository>? profileRepo = null,
            Mock<IFileService>? fileService = null)
            => new CharityService(
                (charityRepo ?? MockCharityRepo()).Object,
                (charityNeedRepo ?? MockCharityNeedRepo()).Object,
                (needAppRepo ?? MockNeedAppRepo()).Object,
                (offerAppRepo ?? MockOfferAppRepo()).Object,
                (offerRepo ?? MockOfferRepo()).Object,
                (profileRepo ?? MockProfileRepo()).Object,
                (fileService ?? MockFileService()).Object);

        // ?? shared entity builders ????????????????????????????????????????????

        private static Charity MakeCharity(
            Guid? userId = null,
            bool isVerified = true,
            bool isActive = true)
            => new()
            {
                CharityId = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                IsVerified = isVerified,
                IsActive = isActive
            };

        private static CharityNeed MakeNeed(
            Guid? charityId = null,
            CharityNeedStatus status = CharityNeedStatus.Pending,
            string? productImage = null)
            => new()
            {
                CharityNeedId = Guid.NewGuid(),
                CharityId = charityId ?? Guid.NewGuid(),
                ProductName = "Rice",
                Category = "food",
                Quantity = 50,
                Priority = CharityNeedPriority.Normal,
                Status = status,
                ProductImage = productImage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

        private static NeedApplication MakeNeedApp(
            Guid? charityId = null,
            ApplicationStatus status = ApplicationStatus.Pending)
        {
            var cn = new CharityNeed { CharityId = charityId ?? Guid.NewGuid() };
            return new NeedApplication
            {
                NeedApplicationId = Guid.NewGuid(),
                CharityNeedId = cn.CharityNeedId,
                DonorOrganizationId = Guid.NewGuid(),
                CharityNeed = cn,
                DonorOrganization = new DonorOrganization { DonorOrganizationName = "Donor A" },
                Status = status,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static OfferApplication MakeOfferApp(
            Guid? charityId = null,
            ApplicationStatus status = ApplicationStatus.Pending)
            => new()
            {
                OfferApplicationId = Guid.NewGuid(),
                CharityId = charityId ?? Guid.NewGuid(),
                OfferId = Guid.NewGuid(),
                Offer = new Offer
                {
                    ProductName = "Pasta",
                    DonorOrganization = new DonorOrganization { DonorOrganizationName = "Donor B" }
                },
                Status = status,
                CreatedAt = DateTime.UtcNow
            };

        private static Mock<IFormFile> MockFormFile()
        {
            var mock = new Mock<IFormFile>();
            mock.Setup(f => f.FileName).Returns("test.jpg");
            mock.Setup(f => f.Length).Returns(512 * 1024);
            return mock;
        }

        // =========================================================
        // GET DASHBOARD
        // =========================================================

        [Fact]
        public async Task GetDashboardAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .GetDashboardAsync(Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task GetDashboardAsync_ReturnsSuccess_AllCountsMappedCorrectly()
        {
            var profileRepo = MockProfileRepo();
            var needRepo = MockCharityNeedRepo();
            var needAppRepo = MockNeedAppRepo();
            var offerAppRepo = MockOfferAppRepo();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);

            needRepo.Setup(r => r.GetNeedCountsByCharityIdAsync(charity.CharityId))
                    .ReturnsAsync((Total: 10, Pending: 2, Approved: 5, Rejected: 1, Fulfilled: 2));

            needAppRepo.Setup(r => r.GetReceivedCountsByCharityIdAsync(charity.CharityId))
                       .ReturnsAsync((Total: 6, Pending: 3, Accepted: 2, Rejected: 1));

            offerAppRepo.Setup(r => r.GetSentCountsByCharityIdAsync(charity.CharityId))
                        .ReturnsAsync((Total: 4, Pending: 1, Accepted: 2, Rejected: 1));

            var result = await CreateService(
                profileRepo: profileRepo, charityNeedRepo: needRepo,
                needAppRepo: needAppRepo, offerAppRepo: offerAppRepo)
                .GetDashboardAsync(userId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Response.Success);

            var data = result.Response.Data!;
            Assert.Equal(10, data.TotalCharityNeeds);
            Assert.Equal(2, data.PendingCharityNeeds);
            Assert.Equal(5, data.ApprovedCharityNeeds);
            Assert.Equal(1, data.RejectedCharityNeeds);
            Assert.Equal(2, data.FulfilledCharityNeeds);
            Assert.Equal(6, data.TotalNeedApplicationsReceived);
            Assert.Equal(3, data.PendingNeedApplicationsReceived);
            Assert.Equal(2, data.AcceptedNeedApplicationsReceived);
            Assert.Equal(1, data.RejectedNeedApplicationsReceived);
            Assert.Equal(4, data.TotalOfferApplicationsSent);
            Assert.Equal(1, data.PendingOfferApplicationsSent);
            Assert.Equal(2, data.AcceptedOfferApplicationsSent);
            Assert.Equal(1, data.RejectedOfferApplicationsSent);
        }

        [Fact]
        public async Task GetDashboardAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .GetDashboardAsync(Guid.NewGuid());

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // CREATE CHARITY NEED
        // =========================================================

        [Fact]
        public async Task CreateCharityNeedAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .CreateCharityNeedAsync(Guid.NewGuid(), new CreateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task CreateCharityNeedAsync_ReturnsForbidden_WhenNotVerified()
        {
            var profileRepo = MockProfileRepo();
            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId, isVerified: false));

            var result = await CreateService(profileRepo: profileRepo)
                .CreateCharityNeedAsync(userId, new CreateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task CreateCharityNeedAsync_ReturnsForbidden_WhenNotActive()
        {
            var profileRepo = MockProfileRepo();
            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId, isActive: false));

            var result = await CreateService(profileRepo: profileRepo)
                .CreateCharityNeedAsync(userId, new CreateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task CreateCharityNeedAsync_ReturnsBadRequest_WhenImageSaveFails()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);

            var mockFile = MockFormFile();
            fileService.Setup(s => s.SaveImageAsync(mockFile.Object, ImageFolder.Needs))
                       .ReturnsAsync(ServiceResult<string>.BadRequest("Invalid image format."));

            var request = new CreateCharityNeedRequestDTO
            {
                Category = "food",
                ProductName = "Rice",
                Quantity = 10,
                Priority = CharityNeedPriority.Normal,
                ProductImage = mockFile.Object
            };

            var result = await CreateService(
                profileRepo: profileRepo, charityNeedRepo: charityNeedRepo,
                fileService: fileService)
                .CreateCharityNeedAsync(userId, request);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            charityNeedRepo.Verify(r => r.CreateAsync(It.IsAny<CharityNeed>()), Times.Never);
        }

        [Fact]
        public async Task CreateCharityNeedAsync_ReturnsCreated_WithImage()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.CreateAsync(It.IsAny<CharityNeed>()))
                           .ReturnsAsync((CharityNeed n) => n);

            var mockFile = MockFormFile();
            fileService.Setup(s => s.SaveImageAsync(mockFile.Object, ImageFolder.Needs))
                       .ReturnsAsync(ServiceResult<string>.Success("OK", "/images/needs/img.jpg"));
            fileService.Setup(s => s.BuildFullUrl(It.IsAny<string?>()))
                       .Returns((string? p) => p == null ? null : $"https://host{p}");

            var request = new CreateCharityNeedRequestDTO
            {
                Category = "  Food  ",
                ProductName = "  Rice  ",
                Quantity = 10,
                Priority = CharityNeedPriority.High,
                ProductImage = mockFile.Object
            };

            var result = await CreateService(
                profileRepo: profileRepo, charityNeedRepo: charityNeedRepo,
                fileService: fileService)
                .CreateCharityNeedAsync(userId, request);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.True(result.Response.Success);
            Assert.Contains("/images/needs/img.jpg", result.Response.Data!.ProductImage);
        }

        [Fact]
        public async Task CreateCharityNeedAsync_ReturnsCreated_WithoutImage_ProductImageIsNull()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.CreateAsync(It.IsAny<CharityNeed>()))
                           .ReturnsAsync((CharityNeed n) => n);
            fileService.Setup(s => s.BuildFullUrl(null)).Returns((string?)null);

            var request = new CreateCharityNeedRequestDTO
            {
                Category = "food",
                ProductName = "Bread",
                Quantity = 5,
                Priority = CharityNeedPriority.Low,
                ProductImage = null
            };

            var result = await CreateService(
                profileRepo: profileRepo, charityNeedRepo: charityNeedRepo,
                fileService: fileService)
                .CreateCharityNeedAsync(userId, request);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Null(result.Response.Data!.ProductImage);
            fileService.Verify(s => s.SaveImageAsync(It.IsAny<IFormFile>(), It.IsAny<ImageFolder>()), Times.Never);
        }

        [Fact]
        public async Task CreateCharityNeedAsync_CategoryTrimmedAndLowercased()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);

            CharityNeed? saved = null;
            charityNeedRepo.Setup(r => r.CreateAsync(It.IsAny<CharityNeed>()))
                           .Callback<CharityNeed>(n => saved = n)
                           .ReturnsAsync((CharityNeed n) => n);
            fileService.Setup(s => s.BuildFullUrl(null)).Returns((string?)null);

            var request = new CreateCharityNeedRequestDTO
            {
                Category = "  FOOD  ",
                ProductName = "  Bread  ",
                Quantity = 5,
                Priority = CharityNeedPriority.Normal
            };

            await CreateService(profileRepo: profileRepo, charityNeedRepo: charityNeedRepo,
                                fileService: fileService)
                .CreateCharityNeedAsync(userId, request);

            Assert.Equal("food", saved!.Category);
            Assert.Equal("Bread", saved.ProductName);
            Assert.Equal(CharityNeedStatus.Pending, saved.Status);
        }

        [Fact]
        public async Task CreateCharityNeedAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();

            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            charityNeedRepo.Setup(r => r.CreateAsync(It.IsAny<CharityNeed>()))
                           .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .CreateCharityNeedAsync(userId, new CreateCharityNeedRequestDTO
                {
                    Category = "food",
                    ProductName = "Rice",
                    Quantity = 1,
                    Priority = CharityNeedPriority.Normal
                });

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GET MY CHARITY NEEDS
        // =========================================================

        [Fact]
        public async Task GetMyCharityNeedsAsync_ReturnsBadRequest_WhenPageInvalid()
        {
            var result = await CreateService()
                .GetMyCharityNeedsAsync(Guid.NewGuid(),
                    new MyCharityNeedsFilterDTO { Page = 0, PageSize = 10 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("INVALID_PAGE", result.Response.Error!.Code);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_ReturnsBadRequest_WhenPageSizeInvalid()
        {
            var result = await CreateService()
                .GetMyCharityNeedsAsync(Guid.NewGuid(),
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 0 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("INVALID_PAGE_SIZE", result.Response.Error!.Code);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_ReturnsBadRequest_WhenPageSizeTooLarge()
        {
            var result = await CreateService()
                .GetMyCharityNeedsAsync(Guid.NewGuid(),
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 51 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("PAGE_SIZE_LIMIT_EXCEEDED", result.Response.Error!.Code);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_ReturnsBadRequest_WhenStatusInvalid()
        {
            var result = await CreateService()
                .GetMyCharityNeedsAsync(Guid.NewGuid(),
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 10, Status = "InvalidStatus" });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .GetMyCharityNeedsAsync(Guid.NewGuid(),
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_PassesNullStatus_WhenNoFilter()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByCharityIdAsync(charity.CharityId, null, 1, 10))
                           .ReturnsAsync(new List<CharityNeed>());
            charityNeedRepo.Setup(r => r.CountByCharityIdAsync(charity.CharityId, null))
                           .ReturnsAsync(0);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .GetMyCharityNeedsAsync(userId,
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            charityNeedRepo.Verify(r =>
                r.GetByCharityIdAsync(charity.CharityId, null, 1, 10), Times.Once);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_NormalisesStatusToPascalCase()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByCharityIdAsync(charity.CharityId, "Pending", 1, 10))
                           .ReturnsAsync(new List<CharityNeed>());
            charityNeedRepo.Setup(r => r.CountByCharityIdAsync(charity.CharityId, "Pending"))
                           .ReturnsAsync(0);

            // Pass lowercase — should be normalised to "Pending"
            await CreateService(profileRepo: profileRepo, charityNeedRepo: charityNeedRepo)
                .GetMyCharityNeedsAsync(userId,
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 10, Status = "pending" });

            charityNeedRepo.Verify(r =>
                r.GetByCharityIdAsync(charity.CharityId, "Pending", 1, 10), Times.Once);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_ReturnsSuccess_WithCorrectPagination()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);

            var needs = new List<CharityNeed> { MakeNeed(charity.CharityId), MakeNeed(charity.CharityId) };
            charityNeedRepo.Setup(r => r.GetByCharityIdAsync(charity.CharityId, null, 1, 10))
                           .ReturnsAsync(needs);
            charityNeedRepo.Setup(r => r.CountByCharityIdAsync(charity.CharityId, null))
                           .ReturnsAsync(25);
            fileService.Setup(s => s.BuildFullUrl(It.IsAny<string?>()))
                       .Returns((string?)null);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo,
                                             fileService: fileService)
                .GetMyCharityNeedsAsync(userId,
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, result.Response.Data!.Count());
            Assert.Equal(25, result.Response.Pagination!.TotalCount);
            Assert.Equal(3, result.Response.Pagination.TotalPages);
        }

        [Fact]
        public async Task GetMyCharityNeedsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .GetMyCharityNeedsAsync(Guid.NewGuid(),
                    new MyCharityNeedsFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GET MY CHARITY NEED BY ID
        // =========================================================

        [Fact]
        public async Task GetMyCharityNeedByIdAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .GetMyCharityNeedByIdAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetMyCharityNeedByIdAsync_ReturnsNotFound_WhenNeedMissing()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();

            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((CharityNeed?)null);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .GetMyCharityNeedByIdAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetMyCharityNeedByIdAsync_ReturnsForbidden_WhenNotOwner()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charityId: Guid.NewGuid()); // different charity

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .GetMyCharityNeedByIdAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task GetMyCharityNeedByIdAsync_ReturnsSuccess_RegardlessOfStatus()
        {
            // Owner sees Rejected needs — not just Approved
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Rejected);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);
            fileService.Setup(s => s.BuildFullUrl(It.IsAny<string?>())).Returns((string?)null);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo,
                                             fileService: fileService)
                .GetMyCharityNeedByIdAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Rejected", result.Response.Data!.Status);
        }

        [Fact]
        public async Task GetMyCharityNeedByIdAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .GetMyCharityNeedByIdAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // UPDATE CHARITY NEED
        // =========================================================

        [Fact]
        public async Task UpdateCharityNeedAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .UpdateCharityNeedAsync(Guid.NewGuid(), Guid.NewGuid(),
                    new UpdateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateCharityNeedAsync_ReturnsNotFound_WhenNeedMissing()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((CharityNeed?)null);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .UpdateCharityNeedAsync(userId, Guid.NewGuid(),
                    new UpdateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateCharityNeedAsync_ReturnsForbidden_WhenNotOwner()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charityId: Guid.NewGuid()); // different owner

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .UpdateCharityNeedAsync(userId, need.CharityNeedId,
                    new UpdateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task UpdateCharityNeedAsync_ReturnsUnprocessable_WhenNotPending()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Approved);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .UpdateCharityNeedAsync(userId, need.CharityNeedId,
                    new UpdateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
            Assert.Equal("INVALID_STATUS", result.Response.Error!.Code);
        }

        [Fact]
        public async Task UpdateCharityNeedAsync_ReturnsBadRequest_WhenNewImageSaveFails()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Pending, "/images/old.jpg");

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var mockFile = MockFormFile();
            fileService.Setup(s => s.SaveImageAsync(mockFile.Object, ImageFolder.Needs))
                       .ReturnsAsync(ServiceResult<string>.BadRequest("File too large."));

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo,
                                             fileService: fileService)
                .UpdateCharityNeedAsync(userId, need.CharityNeedId,
                    new UpdateCharityNeedRequestDTO { ProductImage = mockFile.Object });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            fileService.Verify(s => s.DeleteImageAsync(It.IsAny<string?>()), Times.Never);
            charityNeedRepo.Verify(r => r.UpdateAsync(It.IsAny<CharityNeed>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCharityNeedAsync_ReplacesImage_DeletesOldOne()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Pending, "/images/old.jpg");

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);
            charityNeedRepo.Setup(r => r.UpdateAsync(It.IsAny<CharityNeed>()))
                           .ReturnsAsync((CharityNeed n) => n);

            var mockFile = MockFormFile();
            fileService.Setup(s => s.SaveImageAsync(mockFile.Object, ImageFolder.Needs))
                       .ReturnsAsync(ServiceResult<string>.Success("OK", "/images/new.jpg"));
            fileService.Setup(s => s.DeleteImageAsync(It.IsAny<string?>()))
                       .Returns(Task.CompletedTask);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo,
                                             fileService: fileService)
                .UpdateCharityNeedAsync(userId, need.CharityNeedId,
                    new UpdateCharityNeedRequestDTO { ProductImage = mockFile.Object });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("/images/new.jpg", need.ProductImage);
            fileService.Verify(s => s.DeleteImageAsync("/images/old.jpg"), Times.Once);
        }

        [Fact]
        public async Task UpdateCharityNeedAsync_OnlyAppliesNonNullFields()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Pending);
            need.ProductName = "OldName";
            need.Quantity = 10;

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);
            charityNeedRepo.Setup(r => r.UpdateAsync(It.IsAny<CharityNeed>()))
                           .ReturnsAsync((CharityNeed n) => n);

            // Only updating ProductName — Quantity should stay 10
            await CreateService(profileRepo: profileRepo, charityNeedRepo: charityNeedRepo)
                .UpdateCharityNeedAsync(userId, need.CharityNeedId,
                    new UpdateCharityNeedRequestDTO { ProductName = "NewName" });

            Assert.Equal("NewName", need.ProductName);
            Assert.Equal(10, need.Quantity);
        }

        [Fact]
        public async Task UpdateCharityNeedAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .UpdateCharityNeedAsync(Guid.NewGuid(), Guid.NewGuid(),
                    new UpdateCharityNeedRequestDTO());

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // DELETE CHARITY NEED
        // =========================================================

        [Fact]
        public async Task DeleteCharityNeedAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .DeleteCharityNeedAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task DeleteCharityNeedAsync_ReturnsNotFound_WhenNeedMissing()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((CharityNeed?)null);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .DeleteCharityNeedAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task DeleteCharityNeedAsync_ReturnsForbidden_WhenNotOwner()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charityId: Guid.NewGuid());

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .DeleteCharityNeedAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task DeleteCharityNeedAsync_ReturnsUnprocessable_WhenNotPending()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Approved);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .DeleteCharityNeedAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }

        [Fact]
        public async Task DeleteCharityNeedAsync_DeletesImageBeforeRecord()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var fileService = MockFileService();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Pending, "/images/img.jpg");

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);
            fileService.Setup(s => s.DeleteImageAsync("/images/img.jpg"))
                       .Returns(Task.CompletedTask);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo,
                                             fileService: fileService)
                .DeleteCharityNeedAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            fileService.Verify(s => s.DeleteImageAsync("/images/img.jpg"), Times.Once);
            charityNeedRepo.Verify(r => r.DeleteAsync(need), Times.Once);
        }

        [Fact]
        public async Task DeleteCharityNeedAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .DeleteCharityNeedAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // FULFILL CHARITY NEED
        // =========================================================

        [Fact]
        public async Task FulfillCharityNeedAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .FulfillCharityNeedAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task FulfillCharityNeedAsync_ReturnsNotFound_WhenNeedMissing()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((CharityNeed?)null);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .FulfillCharityNeedAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task FulfillCharityNeedAsync_ReturnsForbidden_WhenNotOwner()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(Guid.NewGuid(), CharityNeedStatus.Approved);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .FulfillCharityNeedAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Theory]
        [InlineData(CharityNeedStatus.Pending)]
        [InlineData(CharityNeedStatus.Rejected)]
        [InlineData(CharityNeedStatus.Fulfilled)]
        public async Task FulfillCharityNeedAsync_ReturnsUnprocessable_WhenStatusIsNotApproved(
            CharityNeedStatus status)
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, status);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .FulfillCharityNeedAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
            Assert.Equal("INVALID_STATUS", result.Response.Error!.Code);
        }

        [Fact]
        public async Task FulfillCharityNeedAsync_ReturnsSuccess_SetsStatusFulfilled()
        {
            var profileRepo = MockProfileRepo();
            var charityNeedRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var need = MakeNeed(charity.CharityId, CharityNeedStatus.Approved);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            charityNeedRepo.Setup(r => r.GetByIdWithCharityAsync(need.CharityNeedId))
                           .ReturnsAsync(need);
            charityNeedRepo.Setup(r => r.UpdateAsync(It.IsAny<CharityNeed>()))
                           .ReturnsAsync((CharityNeed n) => n);

            var result = await CreateService(profileRepo: profileRepo,
                                             charityNeedRepo: charityNeedRepo)
                .FulfillCharityNeedAsync(userId, need.CharityNeedId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(CharityNeedStatus.Fulfilled, need.Status);
            charityNeedRepo.Verify(r => r.UpdateAsync(need), Times.Once);
        }

        [Fact]
        public async Task FulfillCharityNeedAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .FulfillCharityNeedAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GET RECEIVED APPLICATIONS
        // =========================================================

        [Fact]
        public async Task GetReceivedApplicationsAsync_ReturnsBadRequest_WhenPageInvalid()
        {
            var result = await CreateService()
                .GetReceivedApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 0, PageSize = 10 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetReceivedApplicationsAsync_ReturnsBadRequest_WhenPageSizeInvalid()
        {
            var result = await CreateService()
                .GetReceivedApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 0 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetReceivedApplicationsAsync_ReturnsBadRequest_WhenPageSizeTooLarge()
        {
            var result = await CreateService()
                .GetReceivedApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 51 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetReceivedApplicationsAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .GetReceivedApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetReceivedApplicationsAsync_ReturnsSuccess_DTOFieldsMappedCorrectly()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeNeedApp(charity.CharityId, ApplicationStatus.Accepted);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            needAppRepo.Setup(r => r.GetReceivedByCharityIdAsync(charity.CharityId, 1, 10))
                       .ReturnsAsync(new List<NeedApplication> { app });
            needAppRepo.Setup(r => r.CountReceivedByCharityIdAsync(charity.CharityId))
                       .ReturnsAsync(1);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .GetReceivedApplicationsAsync(userId,
                    new PaginationFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var dto = result.Response.Data!.Single();
            Assert.Equal(app.NeedApplicationId, dto.NeedApplicationId);
            Assert.Equal(app.CharityNeed.ProductName, dto.ProductName);
            Assert.Equal(app.DonorOrganization.DonorOrganizationName, dto.DonorOrganizationName);
            Assert.Equal("Accepted", dto.Status);
        }

        [Fact]
        public async Task GetReceivedApplicationsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .GetReceivedApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // ACCEPT NEED APPLICATION
        // =========================================================

        [Fact]
        public async Task AcceptNeedApplicationAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .AcceptNeedApplicationAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task AcceptNeedApplicationAsync_ReturnsNotFound_WhenApplicationMissing()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            needAppRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((NeedApplication?)null);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .AcceptNeedApplicationAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task AcceptNeedApplicationAsync_ReturnsForbidden_WhenNotOwner()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeNeedApp(charityId: Guid.NewGuid()); // different charity owns the need

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            needAppRepo.Setup(r => r.GetByIdAsync(app.NeedApplicationId)).ReturnsAsync(app);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .AcceptNeedApplicationAsync(userId, app.NeedApplicationId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task AcceptNeedApplicationAsync_ReturnsUnprocessable_WhenNotPending()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeNeedApp(charity.CharityId, ApplicationStatus.Rejected);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            needAppRepo.Setup(r => r.GetByIdAsync(app.NeedApplicationId)).ReturnsAsync(app);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .AcceptNeedApplicationAsync(userId, app.NeedApplicationId);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }

        [Fact]
        public async Task AcceptNeedApplicationAsync_ReturnsSuccess_SetsStatusAccepted()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeNeedApp(charity.CharityId, ApplicationStatus.Pending);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            needAppRepo.Setup(r => r.GetByIdAsync(app.NeedApplicationId)).ReturnsAsync(app);
            needAppRepo.Setup(r => r.UpdateAsync(It.IsAny<NeedApplication>()))
                       .ReturnsAsync((NeedApplication a) => a);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .AcceptNeedApplicationAsync(userId, app.NeedApplicationId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(ApplicationStatus.Accepted, app.Status);
            needAppRepo.Verify(r => r.UpdateAsync(app), Times.Once);
        }

        // =========================================================
        // REJECT NEED APPLICATION
        // =========================================================

        [Fact]
        public async Task RejectNeedApplicationAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .RejectNeedApplicationAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task RejectNeedApplicationAsync_ReturnsNotFound_WhenApplicationMissing()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            needAppRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((NeedApplication?)null);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .RejectNeedApplicationAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task RejectNeedApplicationAsync_ReturnsForbidden_WhenNotOwner()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeNeedApp(charityId: Guid.NewGuid());

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            needAppRepo.Setup(r => r.GetByIdAsync(app.NeedApplicationId)).ReturnsAsync(app);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .RejectNeedApplicationAsync(userId, app.NeedApplicationId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task RejectNeedApplicationAsync_ReturnsUnprocessable_WhenNotPending()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeNeedApp(charity.CharityId, ApplicationStatus.Accepted);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            needAppRepo.Setup(r => r.GetByIdAsync(app.NeedApplicationId)).ReturnsAsync(app);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .RejectNeedApplicationAsync(userId, app.NeedApplicationId);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }

        [Fact]
        public async Task RejectNeedApplicationAsync_ReturnsSuccess_SetsStatusRejected()
        {
            var profileRepo = MockProfileRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeNeedApp(charity.CharityId, ApplicationStatus.Pending);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            needAppRepo.Setup(r => r.GetByIdAsync(app.NeedApplicationId)).ReturnsAsync(app);
            needAppRepo.Setup(r => r.UpdateAsync(It.IsAny<NeedApplication>()))
                       .ReturnsAsync((NeedApplication a) => a);

            var result = await CreateService(profileRepo: profileRepo, needAppRepo: needAppRepo)
                .RejectNeedApplicationAsync(userId, app.NeedApplicationId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(ApplicationStatus.Rejected, app.Status);
            needAppRepo.Verify(r => r.UpdateAsync(app), Times.Once);
        }

        // =========================================================
        // APPLY TO OFFER
        // =========================================================

        [Fact]
        public async Task ApplyToOfferAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .ApplyToOfferAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToOfferAsync_ReturnsForbidden_WhenNotVerified()
        {
            var profileRepo = MockProfileRepo();
            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId, isVerified: false));

            var result = await CreateService(profileRepo: profileRepo)
                .ApplyToOfferAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToOfferAsync_ReturnsForbidden_WhenNotActive()
        {
            var profileRepo = MockProfileRepo();
            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId, isActive: false));

            var result = await CreateService(profileRepo: profileRepo)
                .ApplyToOfferAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToOfferAsync_ReturnsNotFound_WhenOfferNotApproved()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            offerRepo.Setup(r => r.GetApprovedOfferByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Offer?)null);

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo)
                .ApplyToOfferAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToOfferAsync_ReturnsConflict_WhenAlreadyApplied()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var offerId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            offerRepo.Setup(r => r.GetApprovedOfferByIdAsync(offerId))
                     .ReturnsAsync(new Offer { OfferId = offerId });
            offerAppRepo.Setup(r => r.ExistsAsync(charity.CharityId, offerId))
                        .ReturnsAsync(true);

            var result = await CreateService(profileRepo: profileRepo,
                                             offerRepo: offerRepo,
                                             offerAppRepo: offerAppRepo)
                .ApplyToOfferAsync(userId, offerId);

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToOfferAsync_ReturnsCreated_WithCorrectFields()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var offerId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            offerRepo.Setup(r => r.GetApprovedOfferByIdAsync(offerId))
                     .ReturnsAsync(new Offer { OfferId = offerId });
            offerAppRepo.Setup(r => r.ExistsAsync(charity.CharityId, offerId))
                        .ReturnsAsync(false);

            OfferApplication? saved = null;
            offerAppRepo.Setup(r => r.CreateAsync(It.IsAny<OfferApplication>()))
                        .Callback<OfferApplication>(a => saved = a)
                        .ReturnsAsync((OfferApplication a) => a);

            var result = await CreateService(profileRepo: profileRepo,
                                             offerRepo: offerRepo,
                                             offerAppRepo: offerAppRepo)
                .ApplyToOfferAsync(userId, offerId);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(saved);
            Assert.Equal(offerId, saved!.OfferId);
            Assert.Equal(charity.CharityId, saved.CharityId);
            Assert.Equal(ApplicationStatus.Pending, saved.Status);
        }

        [Fact]
        public async Task ApplyToOfferAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .ApplyToOfferAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // GET SENT APPLICATIONS
        // =========================================================

        [Fact]
        public async Task GetSentApplicationsAsync_ReturnsBadRequest_WhenPageInvalid()
        {
            var result = await CreateService()
                .GetSentApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 0, PageSize = 10 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetSentApplicationsAsync_ReturnsBadRequest_WhenPageSizeInvalid()
        {
            var result = await CreateService()
                .GetSentApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 0 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetSentApplicationsAsync_ReturnsBadRequest_WhenPageSizeTooLarge()
        {
            var result = await CreateService()
                .GetSentApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 51 });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetSentApplicationsAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .GetSentApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetSentApplicationsAsync_ReturnsSuccess_DTOFieldsMappedCorrectly()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();

            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeOfferApp(charity.CharityId, ApplicationStatus.Accepted);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            offerAppRepo.Setup(r => r.GetSentByCharityIdAsync(charity.CharityId, 1, 10))
                        .ReturnsAsync(new List<OfferApplication> { app });
            offerAppRepo.Setup(r => r.CountSentByCharityIdAsync(charity.CharityId))
                        .ReturnsAsync(1);

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo)
                .GetSentApplicationsAsync(userId,
                    new PaginationFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var dto = result.Response.Data!.Single();
            Assert.Equal(app.OfferApplicationId, dto.OfferApplicationId);
            Assert.Equal(app.Offer.ProductName, dto.ProductName);
            Assert.Equal(app.Offer.DonorOrganization.DonorOrganizationName, dto.DonorOrganizationName);
            Assert.Equal("Accepted", dto.Status);
        }

        [Fact]
        public async Task GetSentApplicationsAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .GetSentApplicationsAsync(Guid.NewGuid(),
                    new PaginationFilterDTO { Page = 1, PageSize = 10 });

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        // =========================================================
        // CANCEL OFFER APPLICATION
        // =========================================================

        [Fact]
        public async Task CancelOfferApplicationAsync_ReturnsNotFound_WhenCharityMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Charity?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .CancelOfferApplicationAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task CancelOfferApplicationAsync_ReturnsNotFound_WhenApplicationMissing()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId))
                       .ReturnsAsync(MakeCharity(userId));
            offerAppRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                        .ReturnsAsync((OfferApplication?)null);

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo)
                .CancelOfferApplicationAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task CancelOfferApplicationAsync_ReturnsForbidden_WhenNotOwner()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeOfferApp(charityId: Guid.NewGuid()); // different owner

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            offerAppRepo.Setup(r => r.GetByIdAsync(app.OfferApplicationId)).ReturnsAsync(app);

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo)
                .CancelOfferApplicationAsync(userId, app.OfferApplicationId);

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task CancelOfferApplicationAsync_ReturnsUnprocessable_WhenNotPending()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeOfferApp(charity.CharityId, ApplicationStatus.Accepted);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            offerAppRepo.Setup(r => r.GetByIdAsync(app.OfferApplicationId)).ReturnsAsync(app);

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo)
                .CancelOfferApplicationAsync(userId, app.OfferApplicationId);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
            Assert.Equal("INVALID_STATUS", result.Response.Error!.Code);
        }

        [Fact]
        public async Task CancelOfferApplicationAsync_ReturnsSuccess_CallsDeleteAsync()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var charity = MakeCharity(userId);
            var app = MakeOfferApp(charity.CharityId, ApplicationStatus.Pending);

            profileRepo.Setup(r => r.GetCharityByUserIdAsync(userId)).ReturnsAsync(charity);
            offerAppRepo.Setup(r => r.GetByIdAsync(app.OfferApplicationId)).ReturnsAsync(app);
            offerAppRepo.Setup(r => r.DeleteAsync(app)).Returns(Task.CompletedTask);

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo)
                .CancelOfferApplicationAsync(userId, app.OfferApplicationId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            offerAppRepo.Verify(r => r.DeleteAsync(app), Times.Once);
        }

        [Fact]
        public async Task CancelOfferApplicationAsync_ReturnsInternalError_WhenRepositoryThrows()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetCharityByUserIdAsync(It.IsAny<Guid>()))
                       .ThrowsAsync(new Exception("DB error"));

            var result = await CreateService(profileRepo: profileRepo)
                .CancelOfferApplicationAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
    }
}