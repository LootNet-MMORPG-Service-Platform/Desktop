using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using desktop_app.Models;

namespace desktop_app.Services;

public class AdminService
{
    private readonly HttpClient _httpClient;
    
    private readonly string _token;

    public AdminService(string token)
    {
        _token = token;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7124")
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    public async Task<PagedResult<AdminUser>?> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<PagedResult<AdminUser>>("/api/admin/users");
    }
    
    public async Task BlockUserAsync(string userId)
    {
        await _httpClient.PostAsJsonAsync($"/api/admin/users/{userId}/block", new
        {
            days = (int?)null,
            reason = "Blocked from admin panel"
        });
    }

    public async Task UnblockUserAsync(string userId)
    {
        await _httpClient.PostAsync($"/api/admin/users/{userId}/unblock", null);
    }
    
    public async Task ChangeRoleAsync(string userId, int role)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/admin/users/{userId}/role", new
        {
            role
        });

        response.EnsureSuccessStatusCode();
    }
    
    public async Task<ItemCollectionDTO?> GetInventoryAsync(string userId)
    {
        return await _httpClient.GetFromJsonAsync<ItemCollectionDTO>(
            $"/api/admin/users/{userId}/inventory"
        );
    }
    
    public async Task<EquipmentResponseDTO?> GetEquipmentAsync(string userId)
    {
        return await _httpClient.GetFromJsonAsync<EquipmentResponseDTO>(
            $"/api/admin/users/{userId}/equipment"
        );
    }
}