using DeveloperStore.Application.Services;
using DeveloperStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using DeveloperStore.Api.DTOs;

namespace DeveloperStore.Tests.Services
{
    public class UserServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("UserServiceTestsDb")
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddUser()
        {
            var ctx = GetDbContext();
            var service = new UserService(ctx);

            var dto = new CreateUserDto
            {
                Username = "Test",
                Email = "t@test.com",
                Password = "123",
                Role = "User",
                Status = "Active"
            };

            var result = await service.CreateAsync(dto);

            result.Id.Should().BeGreaterThan(0);
        }
    }
}
