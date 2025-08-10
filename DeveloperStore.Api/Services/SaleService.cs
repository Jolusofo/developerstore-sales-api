using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SaleService> _logger;

        public SaleService(AppDbContext context, ILogger<SaleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SaleDto>> GetAllAsync()
        {
            var sales = await _context.Sales.Include(s => s.Items).ToListAsync();
            return sales.Select(MapToDto).ToList();
        }

        public async Task<SaleDto?> GetByIdAsync(int id)
        {
            var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
            return sale == null ? null : MapToDto(sale);
        }

        public async Task<SaleDto> CreateAsync(CreateSaleDto dto)
        {
            var sale = new Sale
            {
                Number = dto.Number,
                Date = dto.Date,
                Customer = dto.Customer,
                Branch = dto.Branch,
                Items = dto.Items.Select(i => new SaleItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            ApplyDiscounts(sale);
            sale.CalculateTotals();

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: SaleCreated - Venda #{Number}", sale.Number);

            return MapToDto(sale);
        }

        public async Task<SaleDto?> UpdateAsync(int id, UpdateSaleDto dto)
        {
            var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null) return null;

            sale.Number = dto.Number;
            sale.Date = dto.Date;
            sale.Customer = dto.Customer;
            sale.Branch = dto.Branch;

            sale.Items.Clear();
            sale.Items.AddRange(dto.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }));

            ApplyDiscounts(sale);
            sale.CalculateTotals();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: SaleModified - Venda #{Number}", sale.Number);

            return MapToDto(sale);
        }

        public async Task<bool> CancelAsync(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return false;

            sale.IsCancelled = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: SaleCancelled - Venda #{Number}", sale.Number);
            return true;
        }

        public async Task<bool> CancelItemAsync(int saleId, int itemId)
        {
            var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) return false;

            var item = sale.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return false;

            sale.Items.Remove(item);
            ApplyDiscounts(sale);
            sale.CalculateTotals();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: ItemCancelled - Venda #{Number}, Item {ItemId}", sale.Number, itemId);

            return true;
        }

        private void ApplyDiscounts(Sale sale)
        {
            foreach (var item in sale.Items)
            {
                if (item.Quantity >= 4 && item.Quantity < 10)
                    item.Discount = item.UnitPrice * 0.10m;
                else if (item.Quantity >= 10 && item.Quantity <= 20)
                    item.Discount = item.UnitPrice * 0.20m;
                else
                    item.Discount = 0;

                if (item.Quantity > 20)
                    throw new InvalidOperationException("Não é permitido vender mais de 20 itens iguais.");
            }
        }

        private SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                Number = sale.Number,
                Date = sale.Date,
                Customer = sale.Customer,
                Branch = sale.Branch,
                TotalAmount = sale.TotalAmount,
                IsCancelled = sale.IsCancelled,
                Items = sale.Items.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount,
                    Total = i.Total
                }).ToList()
            };
        }
    }
}
