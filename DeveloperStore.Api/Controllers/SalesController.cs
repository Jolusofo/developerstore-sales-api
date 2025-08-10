using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _saleService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            return sale == null ? NotFound() : Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSaleDto dto)
        {
            var sale = await _saleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSaleDto dto)
        {
            var sale = await _saleService.UpdateAsync(id, dto);
            return sale == null ? NotFound() : Ok(sale);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id) =>
            await _saleService.CancelAsync(id) ? Ok(new { message = "Venda cancelada com sucesso" }) : NotFound();

        [HttpDelete("{saleId}/items/{itemId}")]
        public async Task<IActionResult> CancelItem(int saleId, int itemId) =>
            await _saleService.CancelItemAsync(saleId, itemId) ? Ok(new { message = "Item cancelado com sucesso" }) : NotFound();
    }
}
