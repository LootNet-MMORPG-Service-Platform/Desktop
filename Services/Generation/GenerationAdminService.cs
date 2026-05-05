using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using desktop_app.Models.Generation;

namespace desktop_app.Services.Generation;

public class GenerationAdminService
{
    private readonly HttpClient _httpClient;

    public GenerationAdminService(string token)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7124")
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<GenerationProfile>?> GetProfilesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<GenerationProfile>>(
            "/api/admin/generation/profiles");
    }
    
    public async Task<List<GenerationRule>?> GetRulesAsync(Guid profileId)
    {
        return await _httpClient.GetFromJsonAsync<List<GenerationRule>>(
            $"/api/admin/generation/profiles/{profileId}/rules");
    }
    
    public async Task<List<TypeWeight>?> GetWeightsAsync(Guid profileId)
    {
        return await _httpClient.GetFromJsonAsync<List<TypeWeight>>(
            $"/api/admin/generation/profiles/{profileId}/weights");
    }
}