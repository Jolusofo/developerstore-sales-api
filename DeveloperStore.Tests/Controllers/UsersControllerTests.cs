using DeveloperStore.Api.Controllers;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeveloperStore.Tests.Controllers
{
    public class UsersControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new AppDbContext(options);
            ctx.Users.Add(new User { Id = 1, Username = "admin", Password = "123" });
            ctx.SaveChanges();
            return ctx;
        }

        [Fact]
        public async Task GetById_ShouldReturnUser_WhenExists()
        {
            var ctx = GetDbContext();
            var controller = new UsersController(ctx);

            var result = await controller.GetById(1);
            var ok = result as OkObjectResult;

            ok.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldAddUser()
        {
            var ctx = GetDbContext();
            var controller = new UsersController(ctx);

            var user = new User { Username = "new", Password = "pass" };
            var result = await controller.Create(user);

            result.Should().BeOfType<CreatedAtActionResult>();
            ctx.Users.Count().Should().Be(2);
        }
    }
}
