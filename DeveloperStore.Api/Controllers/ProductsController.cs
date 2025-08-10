using DeveloperStore.Domain.Entities;
using DeveloperStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeveloperStore.Api.DTOs;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /products?_page=1&_size=10&_order="price desc, title asc"&category=Eletrônicos&_minPrice=100&_maxPrice=500
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int _page = 1,
            int _size = 10,
            string? _order = null,
            string? title = null,
            string? category = null,
            decimal? _minPrice = null,
            decimal? _maxPrice = null)
        {
            var query = _context.Products.AsQueryable();

            // --- Filtro por título (suporta wildcard *) ---
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (title.StartsWith("*"))
                    query = query.Where(p => p.Title.EndsWith(title.Trim('*')));
                else if (title.EndsWith("*"))
                    query = query.Where(p => p.Title.StartsWith(title.Trim('*')));
                else
                    query = query.Where(p => p.Title.Contains(title));
            }

            // --- Filtro por categoria ---
            if (!string.IsNullOrWhiteSpace(category))
            {
                if (category.StartsWith("*"))
                    query = query.Where(p => p.Category.EndsWith(category.Trim('*')));
                else if (category.EndsWith("*"))
                    query = query.Where(p => p.Category.StartsWith(category.Trim('*')));
                else
                    query = query.Where(p => p.Category == category);
            }

            // --- Filtro por faixa de preço ---
            if (_minPrice.HasValue)
                query = query.Where(p => p.Price >= _minPrice.Value);
            if (_maxPrice.HasValue)
                query = query.Where(p => p.Price <= _maxPrice.Value);

            // --- Ordenação dinâmica ---
            if (!string.IsNullOrWhiteSpace(_order))
            {
                var orderClauses = _order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var clause in orderClauses.Select((val, idx) => new { val, idx }))
                {
                    var parts = clause.val.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = parts[0];
                    var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "descending" : "ascending";

                    query = ApplyOrdering(query, field, direction, clause.idx == 0);
                }
            }
            else
            {
                // Ordenação padrão
                query = query.OrderBy(p => p.Id);
            }

            // --- Paginação ---
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)_size);
            var items = await query.Skip((_page - 1) * _size).Take(_size).ToListAsync();

            return Ok(new ProductsResultDto
            {
                Data = items,
                TotalItems = totalItems,
                CurrentPage = _page,
                TotalPages = totalPages
            });
        }

        // Função auxiliar para ordenação dinâmica
        private static IQueryable<Product> ApplyOrdering(IQueryable<Product> source, string field, string direction, bool firstOrder)
        {
            return (field.ToLower(), direction.ToLower()) switch
            {
                ("title", "ascending") => firstOrder ? source.OrderBy(p => p.Title) : ((IOrderedQueryable<Product>)source).ThenBy(p => p.Title),
                ("title", "descending") => firstOrder ? source.OrderByDescending(p => p.Title) : ((IOrderedQueryable<Product>)source).ThenByDescending(p => p.Title),
                ("price", "ascending") => firstOrder ? source.OrderBy(p => p.Price) : ((IOrderedQueryable<Product>)source).ThenBy(p => p.Price),
                ("price", "descending") => firstOrder ? source.OrderByDescending(p => p.Price) : ((IOrderedQueryable<Product>)source).ThenByDescending(p => p.Price),
                _ => source
            };
        }
    }
}
