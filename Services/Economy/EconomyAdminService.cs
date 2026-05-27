using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using desktop_app.Models.Economy;

namespace desktop_app.Services.Economy;

public class EconomyAdminService
{
    private readonly HttpClient _httpClient;

    public EconomyAdminService(string token, HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();

        _httpClient.BaseAddress ??= ApiSettings.BaseUrl;

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<EconomySettings?> GetEconomyAsync()
    {
        return await _httpClient.GetFromJsonAsync<EconomySettings>(
            "/api/admin/market/economy");
    }

    public async Task UpdateEconomyAsync(UpdateEconomySettingsRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "/api/admin/market/economy",
            request);

        response.EnsureSuccessStatusCode();
    }

    public async Task<MarketStats?> GetStatsAsync()
    {
        return await _httpClient.GetFromJsonAsync<MarketStats>(
            "/api/admin/market/stats");
    }
}