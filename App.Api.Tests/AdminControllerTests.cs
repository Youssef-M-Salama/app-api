using App.Api.Controllers.V1;
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
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _mockAdminService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);

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
        public async Task GetDashboardStatistics_ReturnsOk_WhenSuccessful()
        {
            var stats = new AdminDashboardResponseDTO { TotalUsers = 100 };
            _mockAdminService.Setup(s => s.GetDashboardStatisticsAsync())
                .ReturnsAsync(ServiceResult<AdminDashboardResponseDTO>.Success("Success", stats));

            var result = await _controller.GetDashboardStatistics();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetDashboardStatistics_ReturnsInternalError_WhenDatabaseFails()
        {
            _mockAdminService.Setup(s => s.GetDashboardStatisticsAsync())
                .ReturnsAsync(ServiceResult<AdminDashboardResponseDTO>.Internal("Database connection error"));

            var result = await _controller.GetDashboardStatistics();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task VerifyUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var request = new ActionUserRequestDTO { UserId = Guid.NewGuid() };
            _mockAdminService.Setup(s => s.VerifyUserAsync(request))
                .ReturnsAsync(ServiceResult<object>.NotFound("المستخدم غير موجود"));

            var result = await _controller.VerifyUser(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task ApproveCharityNeed_ReturnsUnprocessable_WhenNotPending()
        {
            var request = new ActionCharityNeedRequestDTO { CharityNeedId = Guid.NewGuid() };
            _mockAdminService.Setup(s => s.ApproveCharityNeedAsync(request))
                .ReturnsAsync(ServiceResult<object>.UnprocessableEntity("الطلب ليس في حالة انتظار"));

            var result = await _controller.ApproveCharityNeed(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(422, objectResult.StatusCode);
        }

        [Fact]
        public async Task ApproveCharityNeed_ReturnsInternalError_WhenEmailServiceFails()
        {
            var request = new ActionCharityNeedRequestDTO { CharityNeedId = Guid.NewGuid() };
            _mockAdminService.Setup(s => s.ApproveCharityNeedAsync(request))
                .ReturnsAsync(ServiceResult<object>.Internal("Failed to send approval email"));

            var result = await _controller.ApproveCharityNeed(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WithEmptyList_WhenNoUsersMatch()
        {
            var query = new UsersFilterRequestDTO();
            var emptyList = new List<UserResponseDTO>();
            _mockAdminService.Setup(s => s.GetAllUsersAsync(query))
                .ReturnsAsync(ServiceResult<IEnumerable<UserResponseDTO>>.SuccessPaginated("Success", emptyList, new PaginationInfo()));

            var result = await _controller.GetAllUsers(query);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<UserResponseDTO>>>(objectResult.Value);
            Assert.Empty(apiResponse.Data!);
        }

        [Fact]
        public async Task DeactivateUser_ReturnsBadRequest_WhenValidationFails()
        {
            var request = new ActionUserRequestDTO { UserId = Guid.Empty };
            _mockAdminService.Setup(s => s.DeactivateUserAsync(request))
                .ReturnsAsync(ServiceResult<object>.ValidationError("Validation error", e => e.Add(new FieldError { Field = "UserId", Message = "Invalid ID" })));

            var result = await _controller.DeactivateUser(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }
    }
}
