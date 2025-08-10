using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductsResultDto> GetAllAsync(
            int page, int size, string? order, string? title, string? category, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (title.StartsWith("*"))
                    query = query.Where(p => p.Title.EndsWith(title.Trim('*')));
                else if (title.EndsWith("*"))
                    query = query.Where(p => p.Title.StartsWith(title.Trim('*')));
                else
                    query = query.Where(p => p.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                if (category.StartsWith("*"))
                    query = query.Where(p => p.Category.EndsWith(category.Trim('*')));
                else if (category.EndsWith("*"))
                    query = query.Where(p => p.Category.StartsWith(category.Trim('*')));
                else
                    query = query.Where(p => p.Category == category);
            }

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Ordenação
            query = ApplyOrdering(query, order);

            // Paginação
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            // Mapeamento para DTO
            var data = items.Select(p => new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Category = p.Category
            });

            return new ProductsResultDto
            {
                Data = data,
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        private IQueryable<Product> ApplyOrdering(IQueryable<Product> query, string? order)
        {
            if (!string.IsNullOrWhiteSpace(order))
            {
                var orderClauses = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var clause in orderClauses.Select((val, idx) => new { val, idx }))
                {
                    var parts = clause.val.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = parts[0];
                    var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "descending" : "ascending";

                    query = (field.ToLower(), direction.ToLower()) switch
                    {
                        ("title", "ascending") => clause.idx == 0 ? query.OrderBy(p => p.Title) : ((IOrderedQueryable<Product>)query).ThenBy(p => p.Title),
                        ("title", "descending") => clause.idx == 0 ? query.OrderByDescending(p => p.Title) : ((IOrderedQueryable<Product>)query).ThenByDescending(p => p.Title),
                        ("price", "ascending") => clause.idx == 0 ? query.OrderBy(p => p.Price) : ((IOrderedQueryable<Product>)query).ThenBy(p => p.Price),
                        ("price", "descending") => clause.idx == 0 ? query.OrderByDescending(p => p.Price) : ((IOrderedQueryable<Product>)query).ThenByDescending(p => p.Price),
                        _ => query
                    };
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            return query;
        }
    }
}
