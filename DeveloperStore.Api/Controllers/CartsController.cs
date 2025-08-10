using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int _page = 1, int _size = 10, string? _order = null, int? userId = null, DateTime? _minDate = null, DateTime? _maxDate = null)
        {
            var result = await _cartService.GetAllAsync(_page, _size, _order, userId, _minDate, _maxDate);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCartDto dto)
        {
            var cart = await _cartService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = cart.Id }, cart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCartDto dto)
        {
            var cart = await _cartService.UpdateAsync(id, dto);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _cartService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return Ok(new { message = "Cart deleted successfully" });
        }
    }
}
