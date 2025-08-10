using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SalesController> _logger;

        public SalesController(AppDbContext context, ILogger<SalesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET /sales
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _context.Sales.Include(s => s.Items).ToListAsync();
            return Ok(sales);
        }

        // GET /sales/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        // POST /sales
        [HttpPost]
        public async Task<IActionResult> Create(Sale sale)
        {
            sale.CalculateTotals();

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: SaleCreated - Venda #{Number}", sale.Number);

            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
        }

        // PUT /sales/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Sale updatedSale)
        {
            var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null) return NotFound();

            // Atualiza propriedades
            sale.Number = updatedSale.Number;
            sale.Date = updatedSale.Date;
            sale.Customer = updatedSale.Customer;
            sale.Branch = updatedSale.Branch;
            sale.Items = updatedSale.Items;

            sale.CalculateTotals();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: SaleModified - Venda #{Number}", sale.Number);

            return Ok(sale);
        }

        // DELETE /sales/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            sale.IsCancelled = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: SaleCancelled - Venda #{Number}", sale.Number);

            return Ok(new { message = "Venda cancelada com sucesso" });
        }

        // DELETE /sales/{saleId}/items/{itemId}
        [HttpDelete("{saleId}/items/{itemId}")]
        public async Task<IActionResult> CancelItem(int saleId, int itemId)
        {
            var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) return NotFound();

            var item = sale.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return NotFound();

            sale.Items.Remove(item);
            sale.CalculateTotals();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Evento: ItemCancelled - Venda #{Number}, Item {ItemId}", sale.Number, itemId);

            return Ok(new { message = "Item cancelado com sucesso" });
        }
    }
}
