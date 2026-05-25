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
using System.Globalization;

namespace desktop_app.ViewModels.Users;

public partial class UsersViewModel : ViewModelBase
{
    public string SelectedUsername => SelectedUser?.Username ?? "";
    public string SelectedUserRoleText => SelectedUser?.Role.ToString() ?? "";
    public string SelectedUserCurrencyText =>
        SelectedUser?.Currency.ToString(CultureInfo.CurrentCulture) ?? "";
    public string SelectedUserBlockedText => SelectedUser?.IsBlocked.ToString() ?? "";

    private AdminService _adminService;
    private readonly AuthService _authService;
    private readonly AuthTokenService _authTokenService;
    private readonly Action<string> _onAccessTokenRefreshed;
    private readonly Func<Task> _onSessionExpired;

    private string _currentUserId = "";
    private string _refreshToken = "";

    private int _loadVersion;

    public UserFiltersViewModel Filters { get; } = new();

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

    public UsersViewModel(
        AdminService adminService,
        AuthService authService,
        Action<string> onAccessTokenRefreshed,
        Func<Task> onSessionExpired)
    {
        _adminService = adminService;
        _authService = authService;
        _authTokenService = new AuthTokenService();
        _onAccessTokenRefreshed = onAccessTokenRefreshed;
        _onSessionExpired = onSessionExpired;

        Filters.FiltersChanged += async () =>
        {
            CurrentPage = 1;
            SelectedUser = null;
            await LoadUsersAsync();
        };
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

        var isBlocked = Filters.IsBlocked;
        var role = Filters.Role;
        var search = Filters.Search;

        PagedResult<AdminUser>? result;

        try
        {
            result = await ExecuteAuthorizedAsync(
                () => _adminService.GetUsersAsync(
                CurrentPage,
                PageSize,
                search,
                role,
                isBlocked));
        }
        catch (HttpRequestException)
        {
            StatusMessage = "Failed to load users.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
            return;
        }
        catch (Exception)
        {
            StatusMessage = "Failed to load users.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
            return;
        }

        if (result == null)
        {
            StatusMessage = "Failed to load users.";
            return;
        }

        if (loadVersion != _loadVersion)
            return;

        Users.Clear();

        if (result.Items == null || result.Items.Count == 0)
        {
            TotalCount = result.TotalCount;
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
            var changed = await ExecuteAuthorizedAsync(async () =>
            {
                if (user.IsBlocked)
                    await _adminService.UnblockUserAsync(user.Id);
                else
                    await _adminService.BlockUserAsync(user.Id);
            });

            if (changed)
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

        var changed = await ExecuteAuthorizedAsync(
            () => _adminService.ChangeRoleAsync(selectedUserId, (int)parsedRole));

        if (!changed)
            return;

        await LoadUsersAsync();

        SelectedUser = Users.FirstOrDefault(u => u.Id == selectedUserId);
    }

    public async Task<ItemCollectionDTO?> GetInventoryAsync()
    {
        if (SelectedUser == null)
            return null;

        return await ExecuteAuthorizedAsync(
            () => _adminService.GetInventoryAsync(SelectedUser.Id));
    }

    public async Task<EquipmentResponseDTO?> GetEquipmentAsync()
    {
        if (SelectedUser == null)
            return null;

        return await ExecuteAuthorizedAsync(
            () => _adminService.GetEquipmentAsync(SelectedUser.Id));
    }

    public void ClearSelection()
    {
        SelectedUser = null;
    }

    public void ResetSessionState()
    {
        _currentUserId = "";
        _refreshToken = "";
        Users.Clear();
        SelectedUser = null;
        CurrentPage = 1;
        TotalCount = 0;
        StatusMessage = "Users section ready.";
        RefreshPagingState();
    }

    [RelayCommand]
    private void CloseSelection()
    {
        SelectedUser = null;
    }

    partial void OnSelectedUserChanged(AdminUser? value)
    {
        OnPropertyChanged(nameof(HasSelectedUser));
        OnPropertyChanged(nameof(CanChangeSelectedUserRole));

        OnPropertyChanged(nameof(SelectedUsername));
        OnPropertyChanged(nameof(SelectedUserRoleText));
        OnPropertyChanged(nameof(SelectedUserCurrencyText));
        OnPropertyChanged(nameof(SelectedUserBlockedText));
    }

    private void RefreshPagingState()
    {
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(CanGoPrevious));
    }

    private async Task<bool> ExecuteAuthorizedAsync(Func<Task> action)
    {
        try
        {
            await action();
            return true;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (!await RefreshSessionAsync())
                return false;

            try
            {
                await action();
                return true;
            }
            catch (HttpRequestException retryEx) when (retryEx.StatusCode == HttpStatusCode.Forbidden)
            {
                ShowAccessDenied();
                return false;
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            ShowAccessDenied();
            return false;
        }
    }

    private async Task<T?> ExecuteAuthorizedAsync<T>(Func<Task<T?>> action)
    {
        try
        {
            return await action();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (!await RefreshSessionAsync())
                return default;

            try
            {
                return await action();
            }
            catch (HttpRequestException retryEx) when (retryEx.StatusCode == HttpStatusCode.Forbidden)
            {
                ShowAccessDenied();
                return default;
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            ShowAccessDenied();
            return default;
        }
    }

    private static void ShowAccessDenied()
    {
        NotificationService.Instance.ShowError("Access denied. Your permissions may have changed.");
    }

    private async Task<bool> RefreshSessionAsync()
    {
        AuthService.LoginResult? refreshed;

        try
        {
            refreshed = await _authService.RefreshAsync(_refreshToken);
        }
        catch (HttpRequestException)
        {
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
            return false;
        }
        catch (Exception)
        {
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
            return false;
        }

        if (refreshed == null)
        {
            await _onSessionExpired.Invoke();
            return false;
        }

        _onAccessTokenRefreshed.Invoke(refreshed.Token);
        _refreshToken = refreshed.RefreshToken;
        _currentUserId = refreshed.UserId;

        await _authTokenService.SaveRefreshTokenAsync(refreshed.RefreshToken);

        return true;
    }
}
