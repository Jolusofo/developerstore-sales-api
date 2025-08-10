using DeveloperStore.Api.DTOs;

namespace DeveloperStore.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartsResultDto> GetAllAsync(int page, int size, string? order, int? userId, DateTime? minDate, DateTime? maxDate);
        Task<CartDto?> GetByIdAsync(int id);
        Task<CartDto> CreateAsync(CreateCartDto dto);
        Task<CartDto?> UpdateAsync(int id, UpdateCartDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
