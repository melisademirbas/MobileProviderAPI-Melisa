using MobileProviderAPI.Models.DTOs;

namespace MobileProviderAPI.Services;

public interface IAuthenticationService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}

