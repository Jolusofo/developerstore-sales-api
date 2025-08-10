using DeveloperStore.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DeveloperStore.Tests.Domain
{
    public class ProductTests
    {
        [Fact]
        public void Product_Should_Instantiate_With_Default_Values()
        {
            var product = new Product();

            product.Id.Should().Be(0);
            product.Title.Should().BeEmpty();
            product.Price.Should().Be(0);
            product.Description.Should().BeEmpty();
            product.Category.Should().BeEmpty();
            product.Image.Should().BeEmpty();
            product.RatingRate.Should().Be(0);
            product.RatingCount.Should().Be(0);
        }

        [Fact]
        public void Product_Should_Assign_Properties_Correctly()
        {
            var product = new Product
            {
                Id = 10,
                Title = "Mouse Gamer",
                Price = 199.99m,
                Description = "Mouse óptico de alta precisão",
                Category = "Periféricos",
                Image = "mouse.jpg",
                RatingRate = 4.8,
                RatingCount = 120
            };

            product.Id.Should().Be(10);
            product.Title.Should().Be("Mouse Gamer");
            product.Price.Should().Be(199.99m);
            product.Description.Should().Be("Mouse óptico de alta precisão");
            product.Category.Should().Be("Periféricos");
            product.Image.Should().Be("mouse.jpg");
            product.RatingRate.Should().Be(4.8);
            product.RatingCount.Should().Be(120);
        }
    }
}