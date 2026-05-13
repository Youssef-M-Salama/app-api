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
    public class CharityControllerTests
    {
        private readonly Mock<ICharityService> _mockCharityService;
        private readonly Mock<IVerificationDataService> _mockVerificationDataService;
        private readonly CharityController _controller;

        public CharityControllerTests()
        {
            _mockCharityService = new Mock<ICharityService>();
            _mockVerificationDataService = new Mock<IVerificationDataService>();
            _controller = new CharityController(_mockCharityService.Object, _mockVerificationDataService.Object);

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
        public async Task CreateCharityNeed_ReturnsCreated_WhenValid()
        {
            var request = new CreateCharityNeedRequestDTO { ProductName = "Food" };
            var response = new CharityNeedDetailResponseDTO { ProductName = "Food" };
            _mockCharityService.Setup(s => s.CreateCharityNeedAsync(It.IsAny<Guid>(), request))
                .ReturnsAsync(ServiceResult<CharityNeedDetailResponseDTO>.Created("Created", response));

            var result = await _controller.CreateCharityNeed(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateCharityNeed_ReturnsInternalError_WhenFileServiceCrashes()
        {
            var request = new CreateCharityNeedRequestDTO { ProductName = "Food" };
            _mockCharityService.Setup(s => s.CreateCharityNeedAsync(It.IsAny<Guid>(), request))
                .ReturnsAsync(ServiceResult<CharityNeedDetailResponseDTO>.Internal("Storage service unavailable"));

            var result = await _controller.CreateCharityNeed(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCharityNeed_ReturnsForbidden_WhenNotOwner()
        {
            var id = Guid.NewGuid();
            var request = new UpdateCharityNeedRequestDTO();
            _mockCharityService.Setup(s => s.UpdateCharityNeedAsync(It.IsAny<Guid>(), id, request))
                .ReturnsAsync(ServiceResult<object>.Forbidden("Not your post"));

            var result = await _controller.UpdateCharityNeed(id, request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCharityNeed_ReturnsUnprocessable_WhenNotPending()
        {
            var id = Guid.NewGuid();
            _mockCharityService.Setup(s => s.DeleteCharityNeedAsync(It.IsAny<Guid>(), id))
                .ReturnsAsync(ServiceResult<object>.UnprocessableEntity("Cannot delete approved post"));

            var result = await _controller.DeleteCharityNeed(id);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(422, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetDashboard_ReturnsInternalError_WhenDatabaseFails()
        {
            _mockCharityService.Setup(s => s.GetDashboardAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ServiceResult<CharityDashboardResponseDTO>.Internal("DB Error"));

            var result = await _controller.GetDashboard();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task AcceptNeedApplication_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockCharityService.Setup(s => s.AcceptNeedApplicationAsync(It.IsAny<Guid>(), id))
                .ReturnsAsync(ServiceResult<object>.NotFound("Application not found"));

            var result = await _controller.AcceptNeedApplication(id);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }
    }
}
