using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /carts?_page=1&_size=10&_order="id desc"
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int _page = 1,
            int _size = 10,
            string? _order = null,
            int? userId = null,
            DateTime? _minDate = null,
            DateTime? _maxDate = null)
        {
            var query = _context.Carts.AsQueryable();

            // Filtros
            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);
            if (_minDate.HasValue)
                query = query.Where(c => c.Date >= _minDate.Value);
            if (_maxDate.HasValue)
                query = query.Where(c => c.Date <= _maxDate.Value);

            // Ordenação
            if (!string.IsNullOrWhiteSpace(_order))
            {
                var parts = _order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts.Select((o, i) => new { o, i }))
                {
                    var info = part.o.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = info[0];
                    var desc = info.Length > 1 && info[1].ToLower() == "desc";

                    if (field.ToLower() == "id")
                        query = desc
                            ? (part.i == 0 ? query.OrderByDescending(c => c.Id) : ((IOrderedQueryable<Cart>)query).ThenByDescending(c => c.Id))
                            : (part.i == 0 ? query.OrderBy(c => c.Id) : ((IOrderedQueryable<Cart>)query).ThenBy(c => c.Id));
                    if (field.ToLower() == "userid")
                        query = desc
                            ? (part.i == 0 ? query.OrderByDescending(c => c.UserId) : ((IOrderedQueryable<Cart>)query).ThenByDescending(c => c.UserId))
                            : (part.i == 0 ? query.OrderBy(c => c.UserId) : ((IOrderedQueryable<Cart>)query).ThenBy(c => c.UserId));
                }
            }

            // Paginação
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)_size);
            var items = await query.Skip((_page - 1) * _size).Take(_size).ToListAsync();

            return Ok(new
            {
                data = items,
                totalItems,
                currentPage = _page,
                totalPages
            });
        }

        // GET /carts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        // POST /carts
        [HttpPost]
        public async Task<IActionResult> Create(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cart.Id }, cart);
        }

        // PUT /carts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Cart updatedCart)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return NotFound();

            _context.Entry(cart).CurrentValues.SetValues(updatedCart);
            await _context.SaveChangesAsync();
            return Ok(updatedCart);
        }

        // DELETE /carts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return NotFound();

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart deleted successfully" });
        }
    }
}
