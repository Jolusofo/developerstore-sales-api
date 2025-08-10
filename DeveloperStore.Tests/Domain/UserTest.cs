using DeveloperStore.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DeveloperStore.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void User_Should_Instantiate_With_Default_Values()
        {
            var user = new User();

            user.Id.Should().Be(0);
            user.Email.Should().BeEmpty();
            user.Username.Should().BeEmpty();
            user.Password.Should().BeEmpty();
            user.Name.Should().NotBeNull();
            user.Address.Should().NotBeNull();
            user.Phone.Should().BeEmpty();
            user.Status.Should().Be("Active");
            user.Role.Should().Be("Customer");
        }

        [Fact]
        public void User_Should_Assign_Properties_Correctly()
        {
            var user = new User
            {
                Id = 10,
                Email = "test@email.com",
                Username = "testuser",
                Password = "123456",
                Name = new Name { Firstname = "John", Lastname = "Doe" },
                Address = new Address
                {
                    City = "CityX",
                    Street = "StreetY",
                    Number = 123,
                    Zipcode = "00000-000",
                    Geolocation = new GeoLocation { Lat = "1.23", Long = "4.56" }
                },
                Phone = "99999-9999",
                Status = "Inactive",
                Role = "Admin"
            };

            user.Id.Should().Be(10);
            user.Email.Should().Be("test@email.com");
            user.Username.Should().Be("testuser");
            user.Password.Should().Be("123456");
            user.Name.Firstname.Should().Be("John");
            user.Name.Lastname.Should().Be("Doe");
            user.Address.City.Should().Be("CityX");
            user.Address.Street.Should().Be("StreetY");
            user.Address.Number.Should().Be(123);
            user.Address.Zipcode.Should().Be("00000-000");
            user.Address.Geolocation.Lat.Should().Be("1.23");
            user.Address.Geolocation.Long.Should().Be("4.56");
            user.Phone.Should().Be("99999-9999");
            user.Status.Should().Be("Inactive");
            user.Role.Should().Be("Admin");
        }
    }
}