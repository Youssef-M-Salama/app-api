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
    public class AuthControllerTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AuthController(_mockAccountService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // ==========================================
        // LOGIN TESTS
        // ==========================================

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            var request = new LoginDTO { UsernameOrEmail = "test@test.com", Password = "Password123!" };
            var responseDto = new AuthResponseDto { Token = "dummy-token", Email = "test@test.com" };
            _mockAccountService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(ServiceResult<AuthResponseDto>.Success("Success", responseDto));

            var result = await _controller.Login(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            var request = new LoginDTO { UsernameOrEmail = "wrong@test.com", Password = "WrongPassword" };
            _mockAccountService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(ServiceResult<AuthResponseDto>.Unauthorized("Invalid credentials"));

            var result = await _controller.Login(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(401, objectResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsForbidden_WhenEmailNotVerified()
        {
            var request = new LoginDTO { UsernameOrEmail = "unverified@test.com", Password = "Password123!" };
            _mockAccountService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(ServiceResult<AuthResponseDto>.Forbidden("Verify email first"));

            var result = await _controller.Login(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objectResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsInternalError_WhenServiceCrashes()
        {
            var request = new LoginDTO { UsernameOrEmail = "test@test.com", Password = "Password123!" };
            _mockAccountService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(ServiceResult<AuthResponseDto>.Internal("Unexpected database error"));

            var result = await _controller.Login(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        // ==========================================
        // REGISTRATION TESTS
        // ==========================================

        [Fact]
        public async Task Register_ReturnsCreated_WhenValid()
        {
            var request = new RegisterDTO { Email = "new@test.com" };
            _mockAccountService.Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(ServiceResult<object>.Created("Created"));

            var result = await _controller.Register(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsConflict_WhenUserExists()
        {
            var request = new RegisterDTO { Email = "existing@test.com" };
            _mockAccountService.Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(ServiceResult<object>.Conflict("User already exists"));

            var result = await _controller.Register(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, objectResult.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenFieldValidationFails()
        {
            var request = new RegisterDTO { Email = "invalid-email" };
            _mockAccountService.Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(ServiceResult<object>.ValidationError("Validation failed", errors => {
                    errors.Add(new FieldError { Field = "Email", Message = "Invalid email format" });
                }));

            var result = await _controller.Register(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<object>>(objectResult.Value);
            Assert.NotNull(apiResponse.Error?.Details);
        }

        // ==========================================
        // VERIFICATION TESTS
        // ==========================================

        [Fact]
        public async Task VerifyEmail_ReturnsHtmlSuccess_WhenTokenIsValid()
        {
            var userId = Guid.NewGuid();
            var token = "valid-token";
            _mockAccountService.Setup(s => s.VerifyEmailAsync(userId, token))
                .ReturnsAsync(ServiceResult<object>.Success("Verified"));

            var result = await _controller.VerifyEmail(userId, token);

            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Contains("تم بنجاح!", contentResult.Content);
        }

        [Fact]
        public async Task VerifyEmail_ReturnsHtmlError_WhenTokenIsInvalid()
        {
            var userId = Guid.NewGuid();
            var token = "bad-token";
            _mockAccountService.Setup(s => s.VerifyEmailAsync(userId, token))
                .ReturnsAsync(ServiceResult<object>.BadRequest("Invalid token"));

            var result = await _controller.VerifyEmail(userId, token);

            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Contains("عذراً، حدث خطأ", contentResult.Content);
        }

        [Fact]
        public async Task ResendVerification_ReturnsNotFound_WhenEmailDoesNotExist()
        {
            var request = new ResendVerificationRequestDto { Email = "nonexistent@test.com" };
            _mockAccountService.Setup(s => s.ResendVerificationEmailAsync(request.Email))
                .ReturnsAsync(ServiceResult<object>.NotFound("User not found"));

            var result = await _controller.ResendVerification(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }
    }
}
