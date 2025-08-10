using DeveloperStore.Application.Services;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DeveloperStore.Tests.Services
{
    public class SaleServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact(DisplayName = "Deve criar venda com desconto aplicado")]
        public async Task CreateAsync_ShouldApplyDiscountRule()
        {
            var ctx = GetDbContext();
            var loggerMock = new Mock<ILogger<SaleService>>();
            var service = new SaleService(ctx, loggerMock.Object);

            var dto = new Api.DTOs.CreateSaleDto
            {
                Number = "VENDA-001",
                Date = DateTime.UtcNow,
                Customer = "Lucas",
                Branch = "SP",
                Items = new List<Api.DTOs.CreateSaleItemDto>
                {
                    new Api.DTOs.CreateSaleItemDto { ProductId = 1, Quantity = 5, UnitPrice = 100m }
                }
            };

            var result = await service.CreateAsync(dto);

            result.Should().NotBeNull();
            result.TotalAmount.Should().Be(450m);
        }
    }
}
