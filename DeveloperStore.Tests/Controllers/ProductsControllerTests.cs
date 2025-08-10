using DeveloperStore.Api.Controllers;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeveloperStore.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Products.AddRange(
                new Product { Id = 1, Title = "Notebook", Price = 3000, Category = "Eletrônicos" },
                new Product { Id = 2, Title = "Mouse", Price = 100, Category = "Eletrônicos" },
                new Product { Id = 3, Title = "Cadeira", Price = 500, Category = "Móveis" }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetAll_ShouldReturnPaginatedProducts()
        {
            var context = GetDbContext();
            var controller = new ProductsController(context);

            var result = await controller.GetAll(_page: 1, _size: 2);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var data = okResult!.Value as ProductsResultDto;
            data.Should().NotBeNull();
            Assert.Equal(2, data!.Data.Count);
            Assert.Equal(3, data.TotalItems);
            Assert.Equal(1, data.CurrentPage);
            Assert.Equal(2, data.TotalPages);
        }

        [Fact]
        public async Task GetAll_ShouldFilterByCategory()
        {
            var context = GetDbContext();
            var controller = new ProductsController(context);

            var result = await controller.GetAll(category: "Eletrônicos");
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var data = okResult!.Value as ProductsResultDto;
            data.Should().NotBeNull();
            Assert.All(data!.Data, p => Assert.Equal("Eletrônicos", p.Category));
        }

        [Fact]
        public async Task GetAll_ShouldFilterByPriceRange()
        {
            var context = GetDbContext();
            var controller = new ProductsController(context);

            var result = await controller.GetAll(_minPrice: 200, _maxPrice: 3500);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var data = okResult!.Value as ProductsResultDto;
            data.Should().NotBeNull();
            Assert.All(data!.Data, p => Assert.InRange(p.Price, 200, 3500));
        }

        [Fact]
        public async Task GetAll_ShouldOrderByPriceDescending()
        {
            var context = GetDbContext();
            var controller = new ProductsController(context);

            var result = await controller.GetAll(_order: "price desc");
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var data = okResult!.Value as ProductsResultDto;
            data.Should().NotBeNull();
            var products = data!.Data.ToList();
            products.Should().BeInDescendingOrder(p => p.Price);
        }
    }
}