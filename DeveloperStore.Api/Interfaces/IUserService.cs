using DeveloperStore.Api.DTOs;

namespace DeveloperStore.Application.Interfaces
{
    public interface IUserService
    {
        Task<UsersResultDto> GetAllAsync(int page, int size, string? order, string? username, string? email, string? status, string? role);
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
