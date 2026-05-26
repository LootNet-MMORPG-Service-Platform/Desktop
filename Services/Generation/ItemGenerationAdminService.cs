using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using desktop_app.Models.Generation;
using desktop_app.Enums;

namespace desktop_app.Services.Generation;

public class ItemGenerationAdminService
{
    private readonly HttpClient _httpClient;

    public ItemGenerationAdminService(string token)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = ApiSettings.BaseUrl
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

    public async Task CreateTypeWeightAsync(
        Guid profileId,
        ItemCategory category,
        WeaponType? weaponType,
        ArmorType? armorType,
        double weight)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/admin/generation/profiles/{profileId}/weights",
            new
            {
                category,
                weaponType,
                armorType,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTypeWeightAsync(Guid weightId)
    {
        var response = await _httpClient.DeleteAsync(
            $"/api/admin/generation/weights/{weightId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateTypeWeightAsync(
        Guid weightId,
        ItemCategory category,
        WeaponType? weaponType,
        ArmorType? armorType,
        double weight)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "/api/admin/generation/weights",
            new
            {
                id = weightId,
                category,
                weaponType,
                armorType,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProfileAsync(Guid profileId)
    {
        var response = await _httpClient.DeleteAsync(
            $"/api/admin/generation/profiles/{profileId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task CreateProfileAsync(string name)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/admin/generation/profiles",
            new
            {
                name
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateProfileAsync(Guid profileId, string name)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "/api/admin/generation/profiles",
            new
            {
                id = profileId,
                name
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task CreateRuleAsync(Guid profileId, ItemCategory category, WeaponType? weaponType, ArmorType? armorType, bool isFallback)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/admin/generation/profiles/{profileId}/rules",
            new
            {
                category,
                weaponType,
                armorType,
                isFallback
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRuleAsync(Guid ruleId, ItemCategory category, WeaponType? weaponType, ArmorType? armorType, bool isFallback)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "/api/admin/generation/rules",
            new
            {
                id = ruleId,
                category,
                weaponType,
                armorType,
                isFallback
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRuleAsync(Guid ruleId)
    {
        var response = await _httpClient.DeleteAsync(
            $"/api/admin/generation/rules/{ruleId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<GenerationParameter>?> GetParametersAsync(Guid ruleId)
    {
        return await _httpClient.GetFromJsonAsync<List<GenerationParameter>>(
            $"/api/admin/generation/rules/{ruleId}/parameters");
    }

    public async Task CreateParameterAsync(
        Guid ruleId,
        ItemParameter parameter,
        List<CreateSegmentInput> segments)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/admin/generation/rules/{ruleId}/parameters",
            new
            {
                parameter,
                segments
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteParameterAsync(Guid parameterId)
    {
        var response = await _httpClient.DeleteAsync(
            $"/api/admin/generation/parameters/{parameterId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateParameterAsync(
        Guid parameterId,
        ItemParameter parameter,
        List<CreateSegmentInput> segments)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "/api/admin/generation/parameters",
            new
            {
                id = parameterId,
                parameter,
                segments
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<GenerationElement>?> GetElementsAsync(Guid ruleId)
    {
        return await _httpClient.GetFromJsonAsync<List<GenerationElement>>(
            $"/api/admin/generation/rules/{ruleId}/elements");
    }

    public async Task CreateElementAsync(
        Guid ruleId,
        ItemElementType elementType,
        List<CreateSegmentInput> segments)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/admin/generation/rules/{ruleId}/elements",
            new
            {
                elementType,
                segments
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteElementAsync(Guid elementId)
    {
        var response = await _httpClient.DeleteAsync(
            $"/api/admin/generation/elements/{elementId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateElementAsync(
        Guid elementId,
        ItemElementType elementType,
        List<CreateSegmentInput> segments)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "/api/admin/generation/elements",
            new
            {
                id = elementId,
                elementType,
                segments
            });

        response.EnsureSuccessStatusCode();
    }
}
