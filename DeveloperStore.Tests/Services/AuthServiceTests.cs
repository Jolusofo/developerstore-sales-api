using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Services;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DeveloperStore.Tests.Services
{
    public class AuthServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private IConfiguration GetConfiguration(int expiresInMinutes = 60)
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-super-secreta-32caracteres-minimo!!", // >= 32 chars
                ["Jwt:Issuer"] = "DeveloperStore",
                ["Jwt:Audience"] = "DeveloperStoreUsers",
                ["Jwt:ExpiresInMinutes"] = expiresInMinutes.ToString()
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact(DisplayName = "Deve autenticar usuário válido e retornar token")]
        public async Task Authenticate_ValidUser_ReturnsToken()
        {
            var ctx = GetDbContext();
            ctx.Users.Add(new User
            {
                Username = "testuser",
                Password = "123456",
                Role = "Admin",
                Email = "test@example.com",
                Status = "Active"
            });
            await ctx.SaveChangesAsync();

            var configuration = GetConfiguration();
            var service = new AuthService(ctx, configuration);

            var result = await service.AuthenticateAsync(new LoginDto { Username = "testuser", Password = "123456" });

            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "Não deve autenticar usuário inválido")]
        public async Task Authenticate_InvalidUser_ReturnsNull()
        {
            var ctx = GetDbContext();
            ctx.Users.Add(new User
            {
                Username = "testuser",
                Password = "123456",
                Role = "Admin",
                Email = "test@example.com",
                Status = "Active"
            });
            await ctx.SaveChangesAsync();

            var configuration = GetConfiguration();
            var service = new AuthService(ctx, configuration);

            var result = await service.AuthenticateAsync(new LoginDto { Username = "wrong", Password = "wrong" });

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Token deve conter as claims corretas")]
        public async Task Authenticate_ShouldContainCorrectClaims()
        {
            var ctx = GetDbContext();
            ctx.Users.Add(new User
            {
                Username = "testuser",
                Password = "123456",
                Role = "Admin",
                Email = "test@example.com",
                Status = "Active"
            });
            await ctx.SaveChangesAsync();

            var configuration = GetConfiguration();
            var service = new AuthService(ctx, configuration);

            var result = await service.AuthenticateAsync(new LoginDto { Username = "testuser", Password = "123456" });

            result.Should().NotBeNull();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result!.Token);

            var nameClaim = token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
            var roleClaim = token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            nameClaim.Should().Be("testuser");
            roleClaim.Should().Be("Admin");
        }

        [Fact(DisplayName = "Expiração do token deve respeitar configuração")]
        public async Task Token_ShouldExpireCloseToConfigured()
        {
            var expiresIn = 30;
            var ctx = GetDbContext();
            ctx.Users.Add(new User
            {
                Username = "testuser",
                Password = "123456",
                Role = "Admin",
                Email = "test@example.com",
                Status = "Active"
            });
            await ctx.SaveChangesAsync();

            var configuration = GetConfiguration(expiresIn);
            var service = new AuthService(ctx, configuration);

            var result = await service.AuthenticateAsync(new LoginDto { Username = "testuser", Password = "123456" });

            result.Should().NotBeNull();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result!.Token);

            var expClaim = token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            expClaim.Should().NotBeNull();

            var expUnix = long.Parse(expClaim!);
            var expDateUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            var expected = DateTime.UtcNow.AddMinutes(expiresIn);

            Math.Abs((expDateUtc - expected).TotalSeconds).Should().BeLessThan(10);
        }
    }
}
