namespace DeveloperStore.Domain.Entities
{
	public class Product
	{
		public int Id { get; set; }                // Identificador único
		public string Title { get; set; } = "";    // Nome do produto
		public decimal Price { get; set; }         // Preço
		public string Description { get; set; } = "";
		public string Category { get; set; } = "";
		public string Image { get; set; } = "";
		public double RatingRate { get; set; }     // Média de avaliações
		public int RatingCount { get; set; }       // Quantidade de avaliações
	}
}
