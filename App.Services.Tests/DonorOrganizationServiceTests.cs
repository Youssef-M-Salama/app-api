using System.Net;
using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.Domain.Enums;
using App.Core.DTOs.Request;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using App.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace App.Services.Tests
{
    public partial class DonorOrganizationServiceTests
    {
        private static Mock<IProfileRepository> MockProfileRepo() => new();
        private static Mock<IOfferRepository> MockOfferRepo() => new();
        private static Mock<IOfferApplicationRepository> MockOfferAppRepo() => new();
        private static Mock<INeedApplicationRepository> MockNeedAppRepo() => new();
        private static Mock<ICharityNeedRepository> MockCharityNeedRepo() => new();
        private static Mock<IFileService> MockFileService() => new();
        private static Mock<IEmailService> MockEmailService() => new();

        private static DonorOrganizationService CreateService(
            Mock<IProfileRepository>? profileRepo = null,
            Mock<IOfferRepository>? offerRepo = null,
            Mock<IOfferApplicationRepository>? offerAppRepo = null,
            Mock<INeedApplicationRepository>? needAppRepo = null,
            Mock<ICharityNeedRepository>? charityNeedRepo = null,
            Mock<IFileService>? fileService = null,
            Mock<IEmailService>? emailService = null,
            Mock<ICacheService>? cacheService = null)
            => new DonorOrganizationService(
                (profileRepo ?? MockProfileRepo()).Object,
                (offerRepo ?? MockOfferRepo()).Object,
                (offerAppRepo ?? MockOfferAppRepo()).Object,
                (needAppRepo ?? MockNeedAppRepo()).Object,
                (charityNeedRepo ?? MockCharityNeedRepo()).Object,
                (fileService ?? MockFileService()).Object,
                (emailService ?? MockEmailService()).Object,
                new Mock<ILogger<DonorOrganizationService>>().Object,
                (cacheService ?? new Mock<ICacheService>()).Object);

        private static DonorOrganization MakeDonor(
            Guid? userId = null,
            VerificationState verificationState = VerificationState.Verified,
            bool isActive = true)
            => new()
            {
                DonorOrganizationId = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                DonorOrganizationName = "Test Donor",
                VerificationState = verificationState,
                IsActive = isActive
            };

        [Fact]
        public async Task ApplyToCharityNeedAsync_ReturnsNotFound_WhenDonorMissing()
        {
            var profileRepo = MockProfileRepo();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((DonorOrganization?)null);

            var result = await CreateService(profileRepo: profileRepo)
                .ApplyToCharityNeedAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Response.Success);
        }

        [Fact]
        public async Task ApplyToCharityNeedAsync_ReturnsForbidden_WhenNotVerified()
        {
            var profileRepo = MockProfileRepo();
            var userId = Guid.NewGuid();
            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId))
                       .ReturnsAsync(MakeDonor(userId, verificationState: VerificationState.Pending));

            var result = await CreateService(profileRepo: profileRepo)
                .ApplyToCharityNeedAsync(userId, Guid.NewGuid());

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToCharityNeedAsync_ReturnsNotFound_WhenNeedNotApproved()
        {
            var profileRepo = MockProfileRepo();
            var needRepo = MockCharityNeedRepo();
            var userId = Guid.NewGuid();
            var needId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId))
                       .ReturnsAsync(MakeDonor(userId));
            needRepo.Setup(r => r.GetApprovedCharityNeedByIdAsync(needId))
                    .ReturnsAsync((CharityNeed?)null);

            var result = await CreateService(profileRepo: profileRepo, charityNeedRepo: needRepo)
                .ApplyToCharityNeedAsync(userId, needId);

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToCharityNeedAsync_ReturnsConflict_WhenAlreadyApplied()
        {
            var profileRepo = MockProfileRepo();
            var needRepo = MockCharityNeedRepo();
            var needAppRepo = MockNeedAppRepo();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var needId = Guid.NewGuid();

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            needRepo.Setup(r => r.GetApprovedCharityNeedByIdAsync(needId))
                    .ReturnsAsync(new CharityNeed { Status = CharityNeedStatus.Approved });
            needAppRepo.Setup(r => r.ExistsAsync(donor.DonorOrganizationId, needId)).ReturnsAsync(true);

            var result = await CreateService(profileRepo: profileRepo, charityNeedRepo: needRepo, needAppRepo: needAppRepo)
                .ApplyToCharityNeedAsync(userId, needId);

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task ApplyToCharityNeedAsync_ReturnsCreated_AndSendsEmail()
        {
            var profileRepo = MockProfileRepo();
            var needRepo = MockCharityNeedRepo();
            var needAppRepo = MockNeedAppRepo();
            var emailService = MockEmailService();
            var userId = Guid.NewGuid();
            var donor = MakeDonor(userId);
            var needId = Guid.NewGuid();

            var charityNeed = new CharityNeed
            {
                Status = CharityNeedStatus.Approved,
                ProductName = "Food Box",
                Charity = new Charity
                {
                    ApplicationUser = new App.Core.Domain.IdentityEntities.ApplicationUser { Email = "charity@test.com", UserName = "charity" }
                }
            };

            profileRepo.Setup(r => r.GetDonorOrganizationByUserIdAsync(userId)).ReturnsAsync(donor);
            needRepo.Setup(r => r.GetApprovedCharityNeedByIdAsync(needId)).ReturnsAsync(charityNeed);
            needAppRepo.Setup(r => r.ExistsAsync(donor.DonorOrganizationId, needId)).ReturnsAsync(false);

            NeedApplication? saved = null;
            needAppRepo.Setup(r => r.CreateAsync(It.IsAny<NeedApplication>()))
                       .Callback<NeedApplication>(a => saved = a)
                       .ReturnsAsync((NeedApplication a) => a);

            var result = await CreateService(
                profileRepo: profileRepo,
                charityNeedRepo: needRepo,
                needAppRepo: needAppRepo,
                emailService: emailService)
                .ApplyToCharityNeedAsync(userId, needId);

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(saved);
            Assert.Equal(needId, saved!.CharityNeedId);
            Assert.Equal(donor.DonorOrganizationId, saved.DonorOrganizationId);
            Assert.Equal(ApplicationStatus.Pending, saved.Status);

            emailService.Verify(e => e.SendNeedApplicationReceivedAsync(
                "charity@test.com", "charity", donor.DonorOrganizationName, "Food Box"), Times.Once);
        }
    }
}
