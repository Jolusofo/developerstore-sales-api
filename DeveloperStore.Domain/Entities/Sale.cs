namespace DeveloperStore.Domain.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;  // Número da venda
        public DateTime Date { get; set; }                  // Data da venda
        public string Customer { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }            // Valor total da venda
        public bool IsCancelled { get; set; } = false;
        public List<SaleItem> Items { get; set; } = new();

        // Método para calcular totais e aplicar regras
        public void CalculateTotals()
        {
            decimal total = 0;

            foreach (var item in Items)
            {
                // Regras de desconto:
                if (item.Quantity < 4)
                {
                    item.Discount = 0;
                }
                else if (item.Quantity >= 4 && item.Quantity < 10)
                {
                    item.Discount = 0.10m; // 10%
                }
                else if (item.Quantity >= 10 && item.Quantity <= 20)
                {
                    item.Discount = 0.20m; // 20%
                }
                else
                {
                    throw new InvalidOperationException($"Não é possível vender mais de 20 unidades do produto {item.ProductId}");
                }

                // Cálculo total por item
                item.Total = (item.UnitPrice * item.Quantity) * (1 - item.Discount);

                total += item.Total;
            }

            TotalAmount = total;
        }
    }

    public class SaleItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
