using Microsoft.Extensions.Configuration;
using MobileProviderAPI.Common;
using MobileProviderAPI.Models.DTOs;

namespace MobileProviderAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthenticationService(JwtService jwtService, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        // Simple authentication - in production, use proper user store
        Dictionary<string, string> validUsers;
        
        if (_configuration != null)
        {
            validUsers = _configuration.GetSection("ValidUsers").Get<Dictionary<string, string>>() 
                ?? new Dictionary<string, string>();
        }
        else
        {
            validUsers = new Dictionary<string, string>();
        }

        // Default users if configuration is empty
        if (validUsers.Count == 0)
        {
            validUsers = new Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "mobileapp", "mobile123" },
                { "bankingapp", "banking123" }
            };
        }

        if (!validUsers.ContainsKey(request.Username) || 
            validUsers[request.Username] != request.Password)
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        // Determine role based on username
        var role = request.Username switch
        {
            "admin" => "Admin",
            "mobileapp" => "MobileApp",
            "bankingapp" => "BankingApp",
            _ => "User"
        };

        if (_jwtService == null)
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        var token = _jwtService.GenerateToken(request.Username, role);
        var expirationMinutes = _configuration != null 
            ? int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60")
            : 60;

        return Task.FromResult<LoginResponse?>(new LoginResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        });
    }
}

