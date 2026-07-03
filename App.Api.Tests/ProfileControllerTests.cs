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
    public class ProfileControllerTests
    {
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly ProfileController _controller;

        public ProfileControllerTests()
        {
            _mockProfileService = new Mock<IProfileService>();
            _controller = new ProfileController(_mockProfileService.Object);

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
        public async Task GetProfile_ReturnsOk_WithData()
        {
            var profile = new ProfileResponseDTO { Email = "test@test.com" };
            _mockProfileService.Setup(s => s.GetProfileAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ServiceResult<ProfileResponseDTO>.Success("Success", profile));

            var result = await _controller.GetProfile();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetProfile_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _mockProfileService.Setup(s => s.GetProfileAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ServiceResult<ProfileResponseDTO>.NotFound("User not found"));

            var result = await _controller.GetProfile();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateProfile_ReturnsBadRequest_WhenValidationErrorOccurs()
        {
            var request = new UpdateProfileRequestDTO();
            _mockProfileService.Setup(s => s.UpdateProfileAsync(It.IsAny<Guid>(), request))
                .ReturnsAsync(ServiceResult<object>.ValidationError("Validation failed", e => e.Add(new FieldError { Field = "Phone", Message = "Invalid" })));

            var result = await _controller.UpdateProfile(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenOldPasswordIncorrect()
        {
            var request = new ChangePasswordRequestDTO { CurrentPassword = "Wrong" };
            _mockProfileService.Setup(s => s.ChangePasswordAsync(It.IsAny<Guid>(), request))
                .ReturnsAsync(ServiceResult<object>.BadRequest("Current password incorrect"));

            var result = await _controller.ChangePassword(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateProfileImage_ReturnsInternalError_WhenUploadFails()
        {
            var request = new UpdateProfileImageRequestDTO();
            _mockProfileService.Setup(s => s.UpdateProfileImageAsync(It.IsAny<Guid>(), request))
                .ReturnsAsync(ServiceResult<object>.Internal("Upload service error"));

            var result = await _controller.UpdateProfileImage(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetProfile_ReturnsInternalError_WhenServiceFails()
        {
            _mockProfileService.Setup(s => s.GetProfileAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ServiceResult<ProfileResponseDTO>.Internal("Service crash"));

            var result = await _controller.GetProfile();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}
