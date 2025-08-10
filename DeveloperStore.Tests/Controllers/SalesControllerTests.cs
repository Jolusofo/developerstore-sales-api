using DeveloperStore.Api.Controllers;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DeveloperStore.Tests.Controllers
{
    public class SalesControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Create_ShouldApplyDiscountRule()
        {
            var ctx = GetDbContext();
            var logger = new Mock<ILogger<SalesController>>();
            var controller = new SalesController(ctx, logger.Object);

            var sale = new Sale
            {
                Number = "VENDA-001",
                Date = DateTime.UtcNow,
                Customer = "Lucas",
                Branch = "SP",
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = 1, Quantity = 5, UnitPrice = 100m }
                }
            };

            var result = await controller.Create(sale);
            result.Should().BeOfType<CreatedAtActionResult>();

            var saved = ctx.Sales.Include(s => s.Items).First();
            saved.TotalAmount.Should().Be(450m); // com desconto de 10%
        }
    }
}
