using DeveloperStore.Api.DTOs;

namespace DeveloperStore.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductsResultDto> GetAllAsync(
            int page, int size, string? order, string? title, string? category, decimal? minPrice, decimal? maxPrice);
    }
}
