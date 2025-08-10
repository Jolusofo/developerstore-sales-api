using DeveloperStore.Domain.Entities;
using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;

namespace DeveloperStore.Tests.Domain
{
    public class CartTests
    {
        [Fact]
        public void Cart_Should_Instantiate_With_Default_Values()
        {
            var cart = new Cart();

            cart.Id.Should().Be(0);
            cart.UserId.Should().Be(0);
            cart.Date.Should().Be(default(DateTime));
            cart.Products.Should().NotBeNull();
            cart.Products.Should().BeEmpty();
        }

        [Fact]
        public void Cart_Should_Assign_Properties_Correctly()
        {
            var date = new DateTime(2025, 8, 9);
            var cart = new Cart
            {
                Id = 5,
                UserId = 10,
                Date = date,
                Products = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 2 },
                    new CartItem { ProductId = 2, Quantity = 3 }
                }
            };

            cart.Id.Should().Be(5);
            cart.UserId.Should().Be(10);
            cart.Date.Should().Be(date);
            cart.Products.Should().HaveCount(2);
            cart.Products[0].ProductId.Should().Be(1);
            cart.Products[0].Quantity.Should().Be(2);
            cart.Products[1].ProductId.Should().Be(2);
            cart.Products[1].Quantity.Should().Be(3);
        }

        [Fact]
        public void CartItem_Should_Assign_Properties_Correctly()
        {
            var item = new CartItem
            {
                ProductId = 99,
                Quantity = 7
            };

            item.ProductId.Should().Be(99);
            item.Quantity.Should().Be(7);
        }
    }
}