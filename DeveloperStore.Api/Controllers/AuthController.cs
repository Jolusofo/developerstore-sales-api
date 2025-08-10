using DeveloperStore.Api.DTOs;
using DeveloperStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var result = await _authService.AuthenticateAsync(login);
            if (result == null)
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            return Ok(result);
        }
    }
}
