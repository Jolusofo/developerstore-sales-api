using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Services;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DeveloperStore.Tests.Services
{
    public class ProductServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            var ctx = GetDbContext();
            ctx.Products.Add(new DeveloperStore.Domain.Entities.Product
            {
                Title = "Mouse Gamer",
                Price = 150,
                Category = "Periféricos"
            });
            ctx.SaveChanges();

            var service = new ProductService(ctx);
            var result = await service.GetAllAsync(1, 10, null, null, null, null, null);

            result.Data.Should().HaveCount(1);
            result.Data.Should().ContainSingle(p => p.Title == "Mouse Gamer");
        }
    }
}
