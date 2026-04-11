using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace desktop_app.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new System.Uri("http://localhost:5179")
        };
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

        var me = await GetMeAsync(result.Token);

        if (me is null)
            return null;

        return new LoginResult
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            Username = me.Username,
            Role = me.Role
        };
    }

    private async Task<MeResponse?> GetMeAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("/api/auth/me");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<MeResponse>();
    }

    public class LoginResult
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string Username { get; set; } = "";
        public int Role { get; set; }
    }

    private class LoginResponse
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }

    private class MeResponse
    {
        public string Username { get; set; } = "";
        public int Role { get; set; }
    }
}