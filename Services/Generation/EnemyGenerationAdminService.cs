using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using desktop_app.Enums;
using desktop_app.Models.EnemyGeneration;

namespace desktop_app.Services.Generation;

public class EnemyGenerationAdminService
{
    private const string BaseRoute = "/api/admin/enemy-generation";
    private readonly HttpClient _httpClient;

    public EnemyGenerationAdminService(string token)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = ApiSettings.BaseUrl
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<StageProfile>?> GetStageProfilesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<StageProfile>>(
            $"{BaseRoute}/profiles");
    }

    public async Task CreateStageProfileAsync(
        string name,
        int stageIndex,
        double weight,
        double falloff,
        int threshold)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseRoute}/profiles",
            new
            {
                name,
                stageIndex,
                weight,
                falloff,
                threshold
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStageProfileAsync(
        Guid id,
        string name,
        int stageIndex,
        double weight,
        double falloff,
        int threshold)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseRoute}/profiles",
            new
            {
                id,
                name,
                stageIndex,
                weight,
                falloff,
                threshold
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStageProfileAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseRoute}/profiles/{id}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<StageScenario>?> GetStageScenariosAsync(Guid stageProfileId)
    {
        return await _httpClient.GetFromJsonAsync<List<StageScenario>>(
            $"{BaseRoute}/profiles/{stageProfileId}/scenarios");
    }

    public async Task CreateStageScenarioAsync(
        Guid stageProfileId,
        int enemyCount,
        double weight)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseRoute}/profiles/{stageProfileId}/scenarios",
            new
            {
                enemyCount,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStageScenarioAsync(
        Guid id,
        int enemyCount,
        double weight)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseRoute}/scenarios",
            new
            {
                id,
                enemyCount,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStageScenarioAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseRoute}/scenarios/{id}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ScenarioSlot>?> GetScenarioSlotsAsync(Guid scenarioId)
    {
        return await _httpClient.GetFromJsonAsync<List<ScenarioSlot>>(
            $"{BaseRoute}/scenarios/{scenarioId}/slots");
    }

    public async Task CreateScenarioSlotAsync(
        Guid scenarioId,
        int position,
        Guid classProfileId,
        double weight)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseRoute}/scenarios/{scenarioId}/slots",
            new
            {
                position,
                classProfileId,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateScenarioSlotAsync(
        Guid id,
        int position,
        Guid classProfileId,
        double weight)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseRoute}/slots",
            new
            {
                id,
                position,
                classProfileId,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteScenarioSlotAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseRoute}/slots/{id}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<EnemyClassProfile>?> GetEnemyClassProfilesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<EnemyClassProfile>>(
            $"{BaseRoute}/class-profiles");
    }

    public async Task CreateEnemyClassProfileAsync(
        string name,
        EnemyClass enemyClass,
        List<int> allowedColumns,
        Guid generationProfileId,
        double weight)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseRoute}/class-profiles",
            new
            {
                name,
                @class = enemyClass,
                allowedColumns,
                generationProfileId,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateEnemyClassProfileAsync(
        Guid id,
        string name,
        EnemyClass enemyClass,
        List<int> allowedColumns,
        Guid generationProfileId,
        double weight)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseRoute}/class-profiles",
            new
            {
                id,
                name,
                @class = enemyClass,
                allowedColumns,
                generationProfileId,
                weight
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteEnemyClassProfileAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseRoute}/class-profiles/{id}");

        response.EnsureSuccessStatusCode();
    }
}
