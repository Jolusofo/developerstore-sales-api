using DeveloperStore.Api.Controllers;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace DeveloperStore.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Login_ShouldReturnOk_WhenValidCredentials()
        {
            var mockService = new Mock<IAuthService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<LoginDto>()))
                       .ReturnsAsync(new TokenResponseDto { Token = "fake-token" });

            var controller = new AuthController(mockService.Object);

            var result = await controller.Login(new LoginDto { Username = "user", Password = "pass" });

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeOfType<TokenResponseDto>().Subject;
            dto.Token.Should().Be("fake-token");
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            var mockService = new Mock<IAuthService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<LoginDto>()))
                       .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockService.Object);

            var result = await controller.Login(new LoginDto { Username = "user", Password = "wrong" });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}
