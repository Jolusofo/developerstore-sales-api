namespace DeveloperStore.Api.DTOs
{
    public class CreateCartDto
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
