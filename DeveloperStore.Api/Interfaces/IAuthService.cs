using DeveloperStore.Api.DTOs;

namespace DeveloperStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> AuthenticateAsync(LoginDto login);
    }
}
