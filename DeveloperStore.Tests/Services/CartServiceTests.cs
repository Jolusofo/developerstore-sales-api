using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using DeveloperStore.Application.Services;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DeveloperStore.Tests.Services
{
    public class CartServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddCart()
        {
            var ctx = GetDbContext();
            var service = new CartService(ctx);

            var dto = new CreateCartDto
            {
                UserId = 1,
                Date = DateTime.UtcNow
            };

            var result = await service.CreateAsync(dto);

            result.Should().NotBeNull();
            result.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var ctx = GetDbContext();
            var service = new CartService(ctx);

            var result = await service.GetByIdAsync(99);
            result.Should().BeNull();
        }
    }
}
