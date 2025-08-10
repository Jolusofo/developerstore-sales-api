using DeveloperStore.Api.Controllers;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace DeveloperStore.Tests.Controllers
{
    public class CartsControllerTests
    {
        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCartExists()
        {
            var mockService = new Mock<ICartService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new CartDto { Id = 1 });

            var controller = new CartsController(mockService.Object);

            var result = await controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCartDoesNotExist()
        {
            var mockService = new Mock<ICartService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((CartDto?)null);

            var controller = new CartsController(mockService.Object);

            var result = await controller.GetById(1);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
