using DeveloperStore.Api.Controllers;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeveloperStore.Tests.Controllers
{
    public class CartsControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Carts.AddRange(
                new Cart { Id = 1, UserId = 10, Date = new DateTime(2025, 8, 1), Products = new List<CartItem>() },
                new Cart { Id = 2, UserId = 20, Date = new DateTime(2025, 8, 2), Products = new List<CartItem>() },
                new Cart { Id = 3, UserId = 10, Date = new DateTime(2025, 8, 3), Products = new List<CartItem>() }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetAll_ShouldReturnPaginatedCarts()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var result = await controller.GetAll(_page: 1, _size: 2);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var value = okResult!.Value;
            var dataProp = value!.GetType().GetProperty("data")!.GetValue(value, null) as IEnumerable<Cart>;
            var totalItems = (int)value.GetType().GetProperty("totalItems")!.GetValue(value, null)!;
            var currentPage = (int)value.GetType().GetProperty("currentPage")!.GetValue(value, null)!;
            var totalPages = (int)value.GetType().GetProperty("totalPages")!.GetValue(value, null)!;

            dataProp.Should().HaveCount(2);
            totalItems.Should().Be(3);
            currentPage.Should().Be(1);
            totalPages.Should().Be(2);
        }

        [Fact]
        public async Task GetAll_ShouldFilterByUserId()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var result = await controller.GetAll(userId: 10);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var value = okResult!.Value;
            var dataProp = value!.GetType().GetProperty("data")!.GetValue(value, null) as IEnumerable<Cart>;
            dataProp.Should().OnlyContain(c => c.UserId == 10);
        }

        [Fact]
        public async Task GetById_ShouldReturnCart_WhenExists()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var result = await controller.GetById(1);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var cart = okResult!.Value as Cart;
            cart.Should().NotBeNull();
            cart!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNotExists()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var result = await controller.GetById(999);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldAddCart()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var newCart = new Cart { UserId = 30, Date = DateTime.UtcNow, Products = new List<CartItem>() };
            var result = await controller.Create(newCart);

            var created = result as CreatedAtActionResult;
            created.Should().NotBeNull();
            var cart = created!.Value as Cart;
            cart.Should().NotBeNull();
            cart!.UserId.Should().Be(30);
            context.Carts.CountAsync().Result.Should().Be(4);
        }

        [Fact]
        public async Task Update_ShouldModifyCart_WhenExists()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var updatedCart = new Cart { Id = 1, UserId = 99, Date = DateTime.UtcNow, Products = new List<CartItem>() };
            var result = await controller.Update(1, updatedCart);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var cart = okResult!.Value as Cart;
            cart.Should().NotBeNull();
            cart!.UserId.Should().Be(99);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenNotExists()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var updatedCart = new Cart { Id = 999, UserId = 99, Date = DateTime.UtcNow, Products = new List<CartItem>() };
            var result = await controller.Update(999, updatedCart);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ShouldRemoveCart_WhenExists()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var result = await controller.Delete(1);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            context.Carts.Find(1).Should().BeNull();
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenNotExists()
        {
            var context = GetDbContext();
            var controller = new CartsController(context);

            var result = await controller.Delete(999);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}