using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using desktop_app.Models;
using desktop_app.Services;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace desktop_app.ViewModels.Logs;

public partial class LogsViewModel : ViewModelBase
{
    private AdminService _adminService;
    private int _loadVersion;
    private CancellationTokenSource? _filtersDebounceCts;
    private bool _suppressFilterDebounce;
    private bool _adminUsersLoaded;

    public ObservableCollection<AdminLog> Logs { get; } = new();
    public ObservableCollection<AdminFilterOption> AdminOptions { get; } = new();

    public bool HasLogs => Logs.Count > 0;
    public bool CanGoNext => CurrentPage < TotalPages;
    public bool CanGoPrevious => CurrentPage > 1;

    public int TotalPages =>
        PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));

    [ObservableProperty]
    private string _statusMessage = "Logs section ready.";

    [ObservableProperty]
    private string _actionFilter = "";

    [ObservableProperty]
    private AdminFilterOption? _selectedAdminOption;

    [ObservableProperty]
    private string _targetUserIdFilter = "";

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

    private int _pageSize = 30;
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

    public LogsViewModel(AdminService adminService)
    {
        _adminService = adminService;
    }

    public void UpdateAdminService(AdminService adminService)
    {
        _adminService = adminService;
        _adminUsersLoaded = false;
        AdminOptions.Clear();
        _suppressFilterDebounce = true;
        SelectedAdminOption = null;
        _suppressFilterDebounce = false;
    }

    [RelayCommand]
    public async Task LoadLogsAsync()
    {
        var loadVersion = ++_loadVersion;

        StatusMessage = "Loading logs...";

        try
        {
            await EnsureAdminUsersLoadedAsync();

            var result = await _adminService.GetLogsAsync(
                CurrentPage,
                PageSize,
                ActionFilter,
                SelectedAdminOption?.AdminId,
                TargetUserIdFilter);

            if (loadVersion != _loadVersion)
                return;

            Logs.Clear();

            if (result?.Items == null || result.Items.Count == 0)
            {
                TotalCount = result?.TotalCount ?? 0;
                StatusMessage = "No logs found.";
                RefreshLogsState();
                return;
            }

            TotalCount = result.TotalCount;

            foreach (var log in result.Items)
                Logs.Add(log);

            StatusMessage = $"Page {CurrentPage} of {TotalPages} loaded.";
            RefreshLogsState();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == null)
        {
            StatusMessage = "Failed to load logs.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running and verify your internet connection.");
        }
        catch (Exception)
        {
            StatusMessage = "Failed to load logs.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }

    [RelayCommand]
    private async Task ClearFilters()
    {
        _filtersDebounceCts?.Cancel();
        _suppressFilterDebounce = true;
        ActionFilter = "";
        SelectedAdminOption = AdminOptions.Count > 0 ? AdminOptions[0] : null;
        TargetUserIdFilter = "";
        _suppressFilterDebounce = false;
        CurrentPage = 1;
        await LoadLogsAsync();
    }

    [RelayCommand]
    private async Task NextPage()
    {
        if (!CanGoNext)
            return;

        CurrentPage++;
        await LoadLogsAsync();
    }

    [RelayCommand]
    private async Task PreviousPage()
    {
        if (!CanGoPrevious)
            return;

        CurrentPage--;
        await LoadLogsAsync();
    }

    public void ResetSessionState()
    {
        _filtersDebounceCts?.Cancel();
        _adminUsersLoaded = false;
        _suppressFilterDebounce = true;
        ActionFilter = "";
        SelectedAdminOption = null;
        TargetUserIdFilter = "";
        _suppressFilterDebounce = false;
        AdminOptions.Clear();
        Logs.Clear();
        CurrentPage = 1;
        TotalCount = 0;
        StatusMessage = "Logs section ready.";
        RefreshLogsState();
    }

    private async Task EnsureAdminUsersLoadedAsync()
    {
        if (_adminUsersLoaded)
            return;

        var admins = await _adminService.GetAdminUsersAsync();

        AdminOptions.Clear();
        AdminOptions.Add(new AdminFilterOption(null, "All admins"));

        if (admins != null)
        {
            foreach (var admin in admins)
            {
                AdminOptions.Add(new AdminFilterOption(
                    admin.Id,
                    $"{admin.Username} ({admin.Role})"));
            }
        }

        if (SelectedAdminOption == null)
        {
            _suppressFilterDebounce = true;
            SelectedAdminOption = AdminOptions[0];
            _suppressFilterDebounce = false;
        }

        _adminUsersLoaded = true;
    }

    partial void OnActionFilterChanged(string value)
    {
        _ = value;
        DebounceFiltersChanged();
    }

    partial void OnSelectedAdminOptionChanged(AdminFilterOption? value)
    {
        _ = value;
        DebounceFiltersChanged();
    }

    partial void OnTargetUserIdFilterChanged(string value)
    {
        _ = value;
        DebounceFiltersChanged();
    }

    private void DebounceFiltersChanged()
    {
        if (_suppressFilterDebounce)
            return;

        _filtersDebounceCts?.Cancel();
        _filtersDebounceCts = new CancellationTokenSource();

        var token = _filtersDebounceCts.Token;

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
                    await LoadLogsAsync();
                });
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private void RefreshPagingState()
    {
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(CanGoPrevious));
    }

    private void RefreshLogsState()
    {
        OnPropertyChanged(nameof(HasLogs));
        RefreshPagingState();
    }
}

public class AdminFilterOption
{
    public Guid? AdminId { get; }
    public string DisplayName { get; }

    public AdminFilterOption(Guid? adminId, string displayName)
    {
        AdminId = adminId;
        DisplayName = displayName;
    }
}
