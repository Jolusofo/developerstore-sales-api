namespace DeveloperStore.Api.DTOs
{
    public class CreateSaleDto
    {
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Customer { get; set; }
        public string Branch { get; set; }
        public List<CreateSaleItemDto> Items { get; set; } = new();
    }
}
