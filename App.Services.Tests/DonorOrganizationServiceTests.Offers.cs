using System.Net;
using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using Microsoft.AspNetCore.Http;
using Moq;

namespace App.Services.Tests
{
    public partial class DonorOrganizationServiceTests
    {
        [Fact]
        public async Task GetDashboardAsync_ReturnsNotFound_WhenDonorMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync((DonorOrganization?)null);

            var result = await CreateService(profileRepo: profileRepo).GetDashboardAsync(Guid.NewGuid());
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetDashboardAsync_ReturnsSuccess_WithCorrectCounts()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var needAppRepo = MockNeedAppRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.GetOfferCountsByDonorOrganizationIdAsync(donor.DonorOrganizationId))
                     .ReturnsAsync((10, 2, 5, 1, 2, 0));
            needAppRepo.Setup(r => r.GetSentCountsByDonorOrganizationIdAsync(donor.DonorOrganizationId))
                       .ReturnsAsync((4, 2, 1, 1));
            offerAppRepo.Setup(r => r.GetReceivedCountsByDonorOrganizationIdAsync(donor.DonorOrganizationId))
                        .ReturnsAsync((3, 1, 1, 1));

            var result = await CreateService(
                profileRepo: profileRepo,
                offerRepo: offerRepo,
                needAppRepo: needAppRepo,
                offerAppRepo: offerAppRepo).GetDashboardAsync(userId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(10, result.Response.Data!.TotalOffers);
            Assert.Equal(4, result.Response.Data.TotalNeedApplicationsSent);
            Assert.Equal(3, result.Response.Data.TotalOfferApplicationsReceived);
        }

        [Fact]
        public async Task CreateOfferAsync_ReturnsNotFound_WhenDonorMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync((DonorOrganization?)null);

            var result = await CreateService(profileRepo: profileRepo).CreateOfferAsync(Guid.NewGuid(), new CreateOfferRequestDTO { ProductName = "Test", Category = ProductCategory.Food, Quantity = 1m, Unit = MeasurementUnit.Piece });
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task CreateOfferAsync_ReturnsForbidden_WhenNotVerified()
        {
            var profileRepo = MockProfileRepo();
            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(MakeDonor(userId, isVerified: false));

            var result = await CreateService(profileRepo: profileRepo).CreateOfferAsync(userId, new CreateOfferRequestDTO { ProductName = "Test", Category = ProductCategory.Food, Quantity = 1m, Unit = MeasurementUnit.Piece });
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task CreateOfferAsync_ReturnsCreated_WhenValid()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            
            Offer? saved = null;
            offerRepo.Setup(r => r.CreateAsync(It.IsAny<Offer>()))
                     .Callback<Offer>(o => saved = o)
                     .ReturnsAsync((Offer o) => o);

            var req = new CreateOfferRequestDTO { ProductName = "Apples", Category = ProductCategory.Food, Quantity = 50.5m, Unit = MeasurementUnit.Kg };
            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).CreateOfferAsync(userId, req);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(saved);
            Assert.Equal("Apples", saved!.ProductName);
            Assert.Equal(donor.DonorOrganizationId, saved.DonorOrganizationId);
            Assert.Equal(OfferStatus.Pending, saved.Status);
        }

        [Fact]
        public async Task GetMyOffersAsync_ReturnsNotFound_WhenDonorMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync((DonorOrganization?)null);

            var result = await CreateService(profileRepo: profileRepo).GetMyOffersAsync(Guid.NewGuid(), new App.Core.DTOs.Request.MyOffersFilterDTO { Page = 1, PageSize = 10 });
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetMyOffersAsync_ReturnsSuccess()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.CountByDonorOrganizationIdAsync(donor.DonorOrganizationId, null)).ReturnsAsync(1);
            offerRepo.Setup(r => r.GetByDonorOrganizationIdAsync(donor.DonorOrganizationId, null, 1, 10))
                     .ReturnsAsync(new List<Offer> { new Offer { OfferId = Guid.NewGuid(), ProductName = "Test" } });

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).GetMyOffersAsync(userId, new App.Core.DTOs.Request.MyOffersFilterDTO { Page = 1, PageSize = 10 });
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Single(result.Response.Data!);
        }

        [Fact]
        public async Task GetMyOfferByIdAsync_ReturnsNotFound_WhenDonorMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync((DonorOrganization?)null);

            var result = await CreateService(profileRepo: profileRepo).GetMyOfferByIdAsync(Guid.NewGuid(), Guid.NewGuid());
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetMyOfferByIdAsync_ReturnsNotFound_WhenOfferMissing()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(MakeDonor(userId));
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(It.IsAny<Guid>())).ReturnsAsync((Offer?)null);

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).GetMyOfferByIdAsync(userId, Guid.NewGuid());
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetMyOfferByIdAsync_ReturnsNotFound_WhenOfferNotOwned()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(MakeDonor(userId));
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(It.IsAny<Guid>())).ReturnsAsync(new Offer { DonorOrganizationId = Guid.NewGuid() });

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).GetMyOfferByIdAsync(userId, Guid.NewGuid());
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetMyOfferByIdAsync_ReturnsSuccess()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var offerId = Guid.NewGuid();
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(offerId)).ReturnsAsync(new Offer { OfferId = offerId, DonorOrganizationId = donor.DonorOrganizationId, ProductName = "Test" });

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).GetMyOfferByIdAsync(userId, offerId);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Test", result.Response.Data!.ProductName);
        }

        [Fact]
        public async Task UpdateOfferAsync_ReturnsNotFound_WhenOfferMissing()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(MakeDonor(userId));
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(It.IsAny<Guid>())).ReturnsAsync((Offer?)null);

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).UpdateOfferAsync(userId, Guid.NewGuid(), new UpdateOfferRequestDTO());
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateOfferAsync_ReturnsError_WhenStatusNotPending()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var offerId = Guid.NewGuid();
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(offerId)).ReturnsAsync(new Offer { OfferId = offerId, DonorOrganizationId = donor.DonorOrganizationId, Status = OfferStatus.Approved });

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).UpdateOfferAsync(userId, offerId, new UpdateOfferRequestDTO());
            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }

        [Fact]
        public async Task UpdateOfferAsync_ReturnsSuccess_WhenValid()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var offerId = Guid.NewGuid();
            var offer = new Offer { OfferId = offerId, DonorOrganizationId = donor.DonorOrganizationId, Status = OfferStatus.Pending, ProductName = "Old Name" };
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(offerId)).ReturnsAsync(offer);
            offerRepo.Setup(r => r.UpdateAsync(It.IsAny<Offer>())).Returns(Task.CompletedTask);

            var req = new UpdateOfferRequestDTO { ProductName = "New Name" };
            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).UpdateOfferAsync(userId, offerId, req);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("New Name", offer.ProductName);
        }

        [Fact]
        public async Task DeleteOfferAsync_ReturnsSuccess()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var offerId = Guid.NewGuid();
            var offer = new Offer { OfferId = offerId, DonorOrganizationId = donor.DonorOrganizationId, Status = OfferStatus.Pending };
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(offerId)).ReturnsAsync(offer);
            offerRepo.Setup(r => r.DeleteAsync(offer)).Returns(Task.CompletedTask);

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).DeleteOfferAsync(userId, offerId);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            offerRepo.Verify(r => r.DeleteAsync(offer), Times.Once);
        }

        [Fact]
        public async Task FulfillOfferAsync_ReturnsError_WhenNotApproved()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var offerId = Guid.NewGuid();
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(offerId)).ReturnsAsync(new Offer { OfferId = offerId, DonorOrganizationId = donor.DonorOrganizationId, Status = OfferStatus.Pending });

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).FulfillOfferAsync(userId, offerId);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }

        [Fact]
        public async Task FulfillOfferAsync_ReturnsSuccess()
        {
            var profileRepo = MockProfileRepo();
            var offerRepo = MockOfferRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var offerId = Guid.NewGuid();
            var offer = new Offer { OfferId = offerId, DonorOrganizationId = donor.DonorOrganizationId, Status = OfferStatus.Approved };
            
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerRepo.Setup(r => r.GetByIdWithDonorAsync(offerId)).ReturnsAsync(offer);
            offerRepo.Setup(r => r.UpdateAsync(It.IsAny<Offer>())).Returns(Task.CompletedTask);

            var result = await CreateService(profileRepo: profileRepo, offerRepo: offerRepo).FulfillOfferAsync(userId, offerId);
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(OfferStatus.Fulfilled, offer.Status);
        }
    }
}
