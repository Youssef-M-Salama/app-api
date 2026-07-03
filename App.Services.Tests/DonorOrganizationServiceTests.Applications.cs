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
        public async Task GetReceivedApplicationsAsync_ReturnsNotFound_WhenDonorMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync((DonorOrganization?)null);

            var result = await CreateService(profileRepo: profileRepo).GetReceivedApplicationsAsync(Guid.NewGuid(), new PaginationFilterDTO { Page = 1, PageSize = 10 });
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetReceivedApplicationsAsync_ReturnsSuccess()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerAppRepo.Setup(r => r.GetReceivedByDonorOrganizationIdAsync(donor.DonorOrganizationId, 1, 10))
                        .ReturnsAsync(new List<OfferApplication> 
                        { 
                            new OfferApplication 
                            { 
                                OfferApplicationId = Guid.NewGuid(), 
                                Offer = new Offer { ProductName = "Test", DonorOrganization = donor },
                                Charity = new Charity 
                                { 
                                    CharityName = "Charity X",
                                    ApplicationUser = new App.Core.Domain.IdentityEntities.ApplicationUser { Email = "c@c.com", PhoneNumber = "01000000000" }
                                }
                            } 
                        });
            offerAppRepo.Setup(r => r.GetReceivedCountsByDonorOrganizationIdAsync(donor.DonorOrganizationId))
                        .ReturnsAsync((1, 0, 0, 0));

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo).GetReceivedApplicationsAsync(userId, new PaginationFilterDTO { Page = 1, PageSize = 10 });
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Single(result.Response.Data!);
            Assert.Equal("Charity X", result.Response.Data!.First().CharityName);
        }

        [Fact]
        public async Task AcceptOfferApplicationAsync_ReturnsNotFound_WhenNotOwned()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var appId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerAppRepo.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync(new OfferApplication { Offer = new Offer { DonorOrganizationId = Guid.NewGuid() } });

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo).AcceptOfferApplicationAsync(userId, appId);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task AcceptOfferApplicationAsync_ReturnsError_WhenNotPending()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var appId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerAppRepo.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync(new OfferApplication { Status = ApplicationStatus.Accepted, Offer = new Offer { DonorOrganizationId = donor.DonorOrganizationId } });

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo).AcceptOfferApplicationAsync(userId, appId);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }

        [Fact]
        public async Task AcceptOfferApplicationAsync_ReturnsSuccess_AndSendsEmail()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var emailService = MockEmailService();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var appId = Guid.NewGuid();
            var app = new OfferApplication 
            { 
                OfferApplicationId = appId, 
                Status = ApplicationStatus.Pending, 
                Offer = new Offer { DonorOrganizationId = donor.DonorOrganizationId, ProductName = "Test Offer" },
                Charity = new Charity { ApplicationUser = new App.Core.Domain.IdentityEntities.ApplicationUser { Email = "c@c.com", UserName = "charity" } }
            };

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerAppRepo.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync(app);
            offerAppRepo.Setup(r => r.UpdateAsync(It.IsAny<OfferApplication>())).Returns(Task.CompletedTask);

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo, emailService: emailService).AcceptOfferApplicationAsync(userId, appId);
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(ApplicationStatus.Accepted, app.Status);
            emailService.Verify(e => e.SendOfferApplicationAcceptedAsync("c@c.com", "charity", "Test Offer"), Times.Once);
        }

        [Fact]
        public async Task RejectOfferApplicationAsync_ReturnsSuccess_AndSendsEmail()
        {
            var profileRepo = MockProfileRepo();
            var offerAppRepo = MockOfferAppRepo();
            var emailService = MockEmailService();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var appId = Guid.NewGuid();
            var app = new OfferApplication 
            { 
                OfferApplicationId = appId, 
                Status = ApplicationStatus.Pending, 
                Offer = new Offer { DonorOrganizationId = donor.DonorOrganizationId, ProductName = "Test Offer" },
                Charity = new Charity { ApplicationUser = new App.Core.Domain.IdentityEntities.ApplicationUser { Email = "c@c.com", UserName = "charity" } }
            };

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            offerAppRepo.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync(app);
            offerAppRepo.Setup(r => r.UpdateAsync(It.IsAny<OfferApplication>())).Returns(Task.CompletedTask);

            var result = await CreateService(profileRepo: profileRepo, offerAppRepo: offerAppRepo, emailService: emailService).RejectOfferApplicationAsync(userId, appId);
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(ApplicationStatus.Rejected, app.Status);
            emailService.Verify(e => e.SendOfferApplicationRejectedAsync("c@c.com", "charity", "Test Offer"), Times.Once);
        }
    }
}
