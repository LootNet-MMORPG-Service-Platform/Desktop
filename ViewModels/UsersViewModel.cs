using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using desktop_app.Models;
using desktop_app.Services;

namespace desktop_app.ViewModels;

public partial class UsersViewModel : ViewModelBase
{
    private AdminService _adminService;
    private readonly AuthService _authService;
    private readonly Action _onUnauthorized;

    private string _currentUserId = "";
    private string _refreshToken = "";

    public ObservableCollection<AdminUser> Users { get; } = new();

    [ObservableProperty]
    private string _statusMessage = "Users section ready.";
    
    [ObservableProperty]
    private AdminUser? _selectedUser;
    
    public bool HasSelectedUser => SelectedUser != null;

    partial void OnSelectedUserChanged(AdminUser? value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasSelectedUser));
    }

    public UsersViewModel(AdminService adminService, AuthService authService, Action onUnauthorized)
    {
        _adminService = adminService;
        _authService = authService;
        _onUnauthorized = onUnauthorized;
    }

    public void UpdateAdminService(AdminService adminService)
    {
        _adminService = adminService;
    }

    [RelayCommand]
    private void SelectUser(AdminUser user)
    {
        SelectedUser = user;
    }
    
    public void SetCurrentUserId(string userId)
    {
        _currentUserId = userId;
    }

    public void SetRefreshToken(string refreshToken)
    {
        _refreshToken = refreshToken;
    }

    public async Task LoadUsersAsync()
    {
        StatusMessage = "Loading users...";

        PagedResult<AdminUser>? result = null;

        try
        {
            result = await _adminService.GetUsersAsync();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await _authService.RefreshAsync(_refreshToken);

            if (refreshed == null)
            {
                _onUnauthorized.Invoke();
                return;
            }

            _adminService = new AdminService(refreshed.Token);
            _refreshToken = refreshed.RefreshToken;
            _currentUserId = refreshed.UserId;

            result = await _adminService.GetUsersAsync();
        }

        Users.Clear();

        if (result?.Items == null)
        {
            StatusMessage = "No users found.";
            return;
        }

        foreach (var user in result.Items)
        {
            user.IsSelf = user.Id == _currentUserId;
            Users.Add(user);
        }

        StatusMessage = $"Loaded {Users.Count} users.";
    }

    [RelayCommand]
    private async Task ToggleBlockStatus(AdminUser user)
    {
        if (user.Id == _currentUserId)
            return;

        user.IsBusy = true;

        try
        {
            if (user.IsBlocked)
                await _adminService.UnblockUserAsync(user.Id);
            else
                await _adminService.BlockUserAsync(user.Id);

            await LoadUsersAsync();
        }
        finally
        {
            user.IsBusy = false;
        }
    }
    
    public async Task ChangeRoleAsync(string newRole)
    {
        if (SelectedUser == null)
            return;

        if (!Enum.TryParse<UserRole>(newRole, out var parsedRole))
            return;

        var selectedUserId = SelectedUser.Id;

        await _adminService.ChangeRoleAsync(selectedUserId, (int)parsedRole);

        await LoadUsersAsync();

        SelectedUser = Users.FirstOrDefault(u => u.Id == selectedUserId);
    }
    
    public async Task<ItemCollectionDTO?> GetInventoryAsync()
    {
        if (SelectedUser == null)
            return null;

        return await _adminService.GetInventoryAsync(SelectedUser.Id);
    }
    
    public async Task<EquipmentResponseDTO?> GetEquipmentAsync()
    {
        if (SelectedUser == null)
            return null;

        return await _adminService.GetEquipmentAsync(SelectedUser.Id);
    }
    
    public void ClearSelection()
    {
        SelectedUser = null;
    }
    
    [RelayCommand]
    private void CloseSelection()
    {
        SelectedUser = null;
    }
}