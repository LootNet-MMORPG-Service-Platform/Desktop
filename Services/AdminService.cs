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

    public AdminService(string token)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = ApiSettings.BaseUrl
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<PagedResult<AdminUser>?> GetUsersAsync(
        int page,
        int pageSize,
        string? search,
        string? role,
        bool? isBlocked)
    {
        var query = $"?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        if (!string.IsNullOrWhiteSpace(role) && role != "All roles")
            query += $"&role={Uri.EscapeDataString(role)}";

        if (isBlocked.HasValue)
            query += $"&isBlocked={isBlocked.Value.ToString().ToLower()}";

        return await _httpClient.GetFromJsonAsync<PagedResult<AdminUser>>($"/api/admin/users{query}");
    }

    public async Task<PagedResult<AdminLog>?> GetLogsAsync(
        int page,
        int pageSize,
        string? action,
        Guid? adminId,
        string? targetUserId)
    {
        var query = $"?Page={page}&PageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(action))
            query += $"&Action={Uri.EscapeDataString(action)}";

        if (adminId.HasValue)
            query += $"&AdminId={adminId.Value}";

        if (!string.IsNullOrWhiteSpace(targetUserId))
            query += $"&TargetUserId={Uri.EscapeDataString(targetUserId)}";

        return await _httpClient.GetFromJsonAsync<PagedResult<AdminLog>>($"/api/admin/logs{query}");
    }

    public async Task<List<AdminUserList>?> GetAdminUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<AdminUserList>>(
            "/api/admin/users/admins");
    }

    public async Task BlockUserAsync(string userId)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/admin/users/{userId}/block", new
        {
            days = (int?)null,
            reason = "Blocked from admin panel"
        });

        response.EnsureSuccessStatusCode();
    }

    public async Task UnblockUserAsync(string userId)
    {
        var response = await _httpClient.PostAsync($"/api/admin/users/{userId}/unblock", null);

        response.EnsureSuccessStatusCode();
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
