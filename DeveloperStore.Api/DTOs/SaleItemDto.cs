namespace DeveloperStore.Api.DTOs
{
	public class SaleItemDto
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal Total { get; set; }
	}
}
