using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Application.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartsResultDto> GetAllAsync(int page, int size, string? order, int? userId, DateTime? minDate, DateTime? maxDate)
        {
            var query = _context.Carts.AsQueryable();

            // Filtros
            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);
            if (minDate.HasValue)
                query = query.Where(c => c.Date >= minDate.Value);
            if (maxDate.HasValue)
                query = query.Where(c => c.Date <= maxDate.Value);

            // Ordenação
            if (!string.IsNullOrWhiteSpace(order))
            {
                var parts = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
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

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var data = items.Select(c => new CartDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Date = c.Date
            });

            return new CartsResultDto
            {
                Data = data,
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<CartDto?> GetByIdAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            return cart == null ? null : new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Date = cart.Date
            };
        }

        public async Task<CartDto> CreateAsync(CreateCartDto dto)
        {
            var cart = new Cart
            {
                UserId = dto.UserId,
                Date = dto.Date
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Date = cart.Date
            };
        }

        public async Task<CartDto?> UpdateAsync(int id, UpdateCartDto dto)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return null;

            cart.UserId = dto.UserId;
            cart.Date = dto.Date;

            await _context.SaveChangesAsync();

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Date = cart.Date
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return false;

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
