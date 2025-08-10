namespace DeveloperStore.Api.DTOs
{
    public class UpdateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string Role { get; set; } = "Customer";
    }
}
