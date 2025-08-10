using DeveloperStore.Api.Controllers;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace DeveloperStore.Tests.Controllers
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var mockService = new Mock<IProductService>();
            mockService.Setup(s =>
                s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(new ProductsResultDto
                {
                    Data = new List<ProductDto>()
                });

            var controller = new ProductsController(mockService.Object);

            var result = await controller.GetAll(1, 10, null, null, null, null, null);

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
