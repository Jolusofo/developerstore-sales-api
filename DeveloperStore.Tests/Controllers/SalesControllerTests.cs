using DeveloperStore.Api.Controllers;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace DeveloperStore.Tests.Controllers
{
    public class SalesControllerTests
    {
        [Fact]
        public async Task GetById_ShouldReturnOk_WhenSaleExists()
        {
            var mockService = new Mock<ISaleService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new SaleDto { Id = 1 });

            var controller = new SalesController(mockService.Object);

            var result = await controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenSaleDoesNotExist()
        {
            var mockService = new Mock<ISaleService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((SaleDto?)null);

            var controller = new SalesController(mockService.Object);

            var result = await controller.GetById(1);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
