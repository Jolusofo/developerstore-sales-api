namespace DeveloperStore.Domain.Entities
{
	public class Product
	{
		public int Id { get; set; }                // Identificador �nico
		public string Title { get; set; } = "";    // Nome do produto
		public decimal Price { get; set; }         // Pre�o
		public string Description { get; set; } = "";
		public string Category { get; set; } = "";
		public string Image { get; set; } = "";
		public double RatingRate { get; set; }     // M�dia de avalia��es
		public int RatingCount { get; set; }       // Quantidade de avalia��es
	}
}
