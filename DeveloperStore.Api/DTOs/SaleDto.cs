namespace DeveloperStore.Api.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Customer { get; set; }
        public string Branch { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
    }
}
