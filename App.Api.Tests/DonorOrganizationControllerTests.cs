using App.Api.Controllers.v1;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace App.Api.Tests
{
    public class DonorOrganizationControllerTests
    {
        private readonly Mock<IDonorOrganizationService> _mockService;
        private readonly Mock<IVerificationDataService> _mockVerificationDataService;
        private readonly DonorOrganizationController _controller;

        public DonorOrganizationControllerTests()
        {
            _mockService = new Mock<IDonorOrganizationService>();
            _mockVerificationDataService = new Mock<IVerificationDataService>();
            _controller = new DonorOrganizationController(_mockService.Object, _mockVerificationDataService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateOffer_ReturnsCreated_WhenValid()
        {
            var request = new CreateOfferRequestDTO { ProductName = "Water" };
            var response = new OfferDetailResponseDTO { ProductName = "Water" };
            _mockService.Setup(s => s.CreateOfferAsync(It.IsAny<Guid>(), request))
                .ReturnsAsync(ServiceResult<OfferDetailResponseDTO>.Created("Created", response));

            var result = await _controller.CreateOffer(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateOffer_ReturnsBadRequest_WhenValidationError()
        {
            var request = new CreateOfferRequestDTO { ProductName = "" };
            _mockService.Setup(s => s.CreateOfferAsync(It.IsAny<Guid>(), request))
                .ReturnsAsync(ServiceResult<OfferDetailResponseDTO>.ValidationError("Validation failed", e => e.Add(new FieldError { Field = "ProductName", Message = "Required" })));

            var result = await _controller.CreateOffer(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task ApplyToCharityNeed_ReturnsConflict_WhenAlreadyApplied()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.ApplyToCharityNeedAsync(It.IsAny<Guid>(), id))
                .ReturnsAsync(ServiceResult<object>.Conflict("Already applied"));

            var result = await _controller.ApplyToCharityNeed(id);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, objectResult.StatusCode);
        }

        [Fact]
        public async Task FulfillOffer_ReturnsUnprocessable_WhenAlreadyFulfilled()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.FulfillOfferAsync(It.IsAny<Guid>(), id))
                .ReturnsAsync(ServiceResult<object>.UnprocessableEntity("Offer already fulfilled"));

            var result = await _controller.FulfillOffer(id);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(422, objectResult.StatusCode);
        }

        [Fact]
        public async Task AcceptOfferApplication_ReturnsForbidden_WhenNotOwner()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.AcceptOfferApplicationAsync(It.IsAny<Guid>(), id))
                .ReturnsAsync(ServiceResult<object>.Forbidden("Not your offer"));

            var result = await _controller.AcceptOfferApplication(id);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetDashboard_ReturnsInternalError_WhenServiceFails()
        {
            _mockService.Setup(s => s.GetDashboardAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ServiceResult<DonorDashboardResponseDTO>.Internal("Service failure"));

            var result = await _controller.GetDashboard();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}
