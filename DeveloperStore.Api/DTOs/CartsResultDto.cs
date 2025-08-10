namespace DeveloperStore.Api.DTOs
{
    public class CartsResultDto
    {
        public IEnumerable<CartDto> Data { get; set; } = new List<CartDto>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
