using DeveloperStore.Api.DTOs;

namespace DeveloperStore.Application.Interfaces
{
    public interface ISaleService
    {
        Task<List<SaleDto>> GetAllAsync();
        Task<SaleDto?> GetByIdAsync(int id);
        Task<SaleDto> CreateAsync(CreateSaleDto dto);
        Task<SaleDto?> UpdateAsync(int id, UpdateSaleDto dto);
        Task<bool> CancelAsync(int id);
        Task<bool> CancelItemAsync(int saleId, int itemId);
    }
}
