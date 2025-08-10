using DeveloperStore.Api.Controllers;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using System.Collections.Generic;

namespace DeveloperStore.Tests.Controllers
{
    public class AuthControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Users.Add(new User
            {
                Id = 1,
                Username = "admin",
                Password = "123456",
                Role = "Admin"
            });
            context.SaveChanges();

            return context;
        }

        private IConfiguration GetFakeConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "super_secret_key_1234567890123456"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:ExpiresInMinutes", "60"}
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var context = GetDbContext();
            var config = GetFakeConfiguration();
            var controller = new AuthController(context, config);

            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "123456"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var tokenResponse = okResult!.Value as TokenResponseDto;
            tokenResponse.Should().NotBeNull();
            tokenResponse!.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var context = GetDbContext();
            var config = GetFakeConfiguration();
            var controller = new AuthController(context, config);

            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "wrongpassword"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            var messageObj = unauthorizedResult!.Value;
            var messageStr = messageObj?.ToString();
            Assert.Contains("Usuário ou senha inválidos", messageStr);
        }
    }
}