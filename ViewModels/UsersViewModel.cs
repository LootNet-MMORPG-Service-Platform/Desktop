using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using desktop_app.Models;
using desktop_app.Services;
using System.Threading;
using Avalonia.Threading;

namespace desktop_app.ViewModels;

public partial class UsersViewModel : ViewModelBase
{
    private AdminService _adminService;
    private readonly AuthService _authService;
    private readonly Action _onUnauthorized;

    private string _currentUserId = "";
    private string _refreshToken = "";
    
    private int _loadVersion = 0;
    
    private CancellationTokenSource? _searchDebounceCts;

    private int _currentPage = 1;
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
                RefreshPagingState();
        }
    }

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value))
                RefreshPagingState();
        }
    }

    private int _totalCount;
    public int TotalCount
    {
        get => _totalCount;
        set
        {
            if (SetProperty(ref _totalCount, value))
                RefreshPagingState();
        }
    }

    public int TotalPages =>
        PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));

    public bool CanGoNext => CurrentPage < TotalPages;
    public bool CanGoPrevious => CurrentPage > 1;

    public ObservableCollection<AdminUser> Users { get; } = new();

    [ObservableProperty]
    private string _statusMessage = "Users section ready.";

    [ObservableProperty]
    private AdminUser? _selectedUser;

    public bool HasSelectedUser => SelectedUser != null;
    
    public bool CanChangeSelectedUserRole =>
        SelectedUser != null &&
        !SelectedUser.IsSelf &&
        SelectedUser.Role != UserRole.SuperAdmin;

    public List<string> RoleOptions { get; } = new()
    {
        "All roles",
        "Player",
        "GameModerator",
        "Admin",
        "SuperAdmin"
    };

    public List<string> StatusOptions { get; } = new()
    {
        "All statuses",
        "Active",
        "Blocked"
    };

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private string _selectedRole = "All roles";

    [ObservableProperty]
    private string _selectedBlockedStatus = "All statuses";

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
        var loadVersion = ++_loadVersion;
        
        StatusMessage = "Loading users...";

        var isBlocked = SelectedBlockedStatus switch
        {
            "Blocked" => true,
            "Active" => false,
            _ => (bool?)null
        };

        var role = SelectedRole == "All roles" ? null : SelectedRole;
        var search = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText.Trim();

        PagedResult<AdminUser>? result;

        try
        {
            result = await _adminService.GetUsersAsync(
                CurrentPage,
                PageSize,
                search,
                role,
                isBlocked);
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

            result = await _adminService.GetUsersAsync(
                CurrentPage,
                PageSize,
                search,
                role,
                isBlocked);
        }

        if (loadVersion != _loadVersion)
            return;

        Users.Clear();

        if (result?.Items == null || result.Items.Count == 0)
        {
            TotalCount = result?.TotalCount ?? 0;
            SelectedUser = null;
            StatusMessage = "No users found.";
            return;
        }

        TotalCount = result.TotalCount;

        foreach (var user in result.Items)
        {
            user.IsSelf = user.Id == _currentUserId;
            Users.Add(user);
        }

        StatusMessage = $"Page {CurrentPage} of {TotalPages} loaded.";
    }

    [RelayCommand]
    private async Task NextPage()
    {
        if (!CanGoNext)
            return;

        CurrentPage++;
        await LoadUsersAsync();
    }

    [RelayCommand]
    private async Task PreviousPage()
    {
        if (!CanGoPrevious)
            return;

        CurrentPage--;
        await LoadUsersAsync();
    }

    [RelayCommand]
    private async Task ClearFilters()
    {
        SearchText = "";
        SelectedRole = "All roles";
        SelectedBlockedStatus = "All statuses";

        CurrentPage = 1;
        SelectedUser = null;
        await LoadUsersAsync();
    }
    
    partial void OnSearchTextChanged(string value)
    {
        DebounceSearch();
    }

    partial void OnSelectedRoleChanged(string value)
    {
        DebounceSearch();
    }

    partial void OnSelectedBlockedStatusChanged(string value)
    {
        DebounceSearch();
    }

    private void DebounceSearch()
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts = new CancellationTokenSource();

        var token = _searchDebounceCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(400, token);

                if (token.IsCancellationRequested)
                    return;

                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    CurrentPage = 1;
                    SelectedUser = null;
                    await LoadUsersAsync();
                });
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    [RelayCommand]
    private void SelectUser(AdminUser user)
    {
        SelectedUser = user;
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

    partial void OnSelectedUserChanged(AdminUser? value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasSelectedUser));
        OnPropertyChanged(nameof(CanChangeSelectedUserRole));
    }

    private void RefreshPagingState()
    {
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(CanGoPrevious));
    }
}