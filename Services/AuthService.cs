using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using desktop_app.Models;

namespace desktop_app.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly JwtParser _jwtParser;

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7124")
        };
        
        _jwtParser = new JwtParser();
    }

    public async Task<LoginResult?> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            username,
            password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result is null || string.IsNullOrWhiteSpace(result.Token))
            return null;

        var user = _jwtParser.Parse(result.Token);

        if (user is null)
            return null;

        return new LoginResult
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
    }
    
    public async Task<LoginResult?> RefreshAsync(string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", refreshToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result is null || string.IsNullOrWhiteSpace(result.Token))
            return null;

        var user = _jwtParser.Parse(result.Token);

        if (user is null)
            return null;

        return new LoginResult
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
    }
    
    public class LoginResult
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        public UserRole Role { get; set; }
    }

    private class LoginResponse
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}