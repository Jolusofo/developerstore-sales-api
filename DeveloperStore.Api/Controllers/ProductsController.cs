using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

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
            var result = await _productService.GetAllAsync(_page, _size, _order, title, category, _minPrice, _maxPrice);
            return Ok(result);
        }
    }
}
