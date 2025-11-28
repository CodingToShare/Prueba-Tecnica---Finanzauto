using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto> RegisterAsync(RegisterRequestDto registerRequest);
        Task<bool> ValidateTokenAsync(string token);
    }
}
