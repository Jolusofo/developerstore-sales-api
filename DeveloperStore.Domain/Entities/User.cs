namespace DeveloperStore.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public Name Name { get; set; } = new();
        public Address Address { get; set; } = new();
        public string Phone { get; set; } = "";
        public string Status { get; set; } = "Active"; // Active, Inactive, Suspended
        public string Role { get; set; } = "Customer"; // Customer, Manager, Admin
    }

    public class Name
    {
        public string Firstname { get; set; } = "";
        public string Lastname { get; set; } = "";
    }

    public class Address
    {
        public string City { get; set; } = "";
        public string Street { get; set; } = "";
        public int Number { get; set; }
        public string Zipcode { get; set; } = "";
        public GeoLocation Geolocation { get; set; } = new();
    }

    public class GeoLocation
    {
        public string Lat { get; set; } = "";
        public string Long { get; set; } = "";
    }
}
