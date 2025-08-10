using DeveloperStore.Api.Controllers;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace DeveloperStore.Tests.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetById_ShouldReturnOk_WhenUserExists()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new UserDto { Id = 1, Username = "Test" });

            var controller = new UsersController(mockService.Object);

            var result = await controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((UserDto?)null);

            var controller = new UsersController(mockService.Object);

            var result = await controller.GetById(1);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
