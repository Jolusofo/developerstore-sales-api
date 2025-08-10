using DeveloperStore.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DeveloperStore.Tests.Domain
{
    public class SaleTests
    {
        [Fact]
        public void CalculateTotals_ShouldApply10PercentDiscount_WhenQuantityBetween4And9()
        {
            // Arrange
            var sale = new Sale
            {
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = 1, Quantity = 5, UnitPrice = 100m }
                }
            };

            // Act
            sale.CalculateTotals();

            // Assert
            sale.TotalAmount.Should().Be(450m); // 5x100 = 500 - 10% = 450
            sale.Items.First().Discount.Should().Be(0.10m);
        }

        [Fact]
        public void CalculateTotals_ShouldThrow_WhenQuantityAbove20()
        {
            var sale = new Sale
            {
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = 1, Quantity = 25, UnitPrice = 100m }
                }
            };

            Action act = () => sale.CalculateTotals();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*mais de 20 unidades*");
        }
    }
}
