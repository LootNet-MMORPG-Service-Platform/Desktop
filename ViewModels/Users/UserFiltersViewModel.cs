using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace desktop_app.ViewModels.Users;

public partial class UserFiltersViewModel : ViewModelBase
{
    private CancellationTokenSource? _debounceCts;

    public event Func<Task>? FiltersChanged;

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

    public string? Search =>
        string.IsNullOrWhiteSpace(SearchText) ? null : SearchText.Trim();

    public string? Role =>
        SelectedRole == "All roles" ? null : SelectedRole;

    public bool? IsBlocked => SelectedBlockedStatus switch
    {
        "Blocked" => true,
        "Active" => false,
        _ => null
    };

    [RelayCommand]
    public async Task Clear()
    {
        SearchText = "";
        SelectedRole = "All roles";
        SelectedBlockedStatus = "All statuses";

        await NotifyFiltersChangedAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        DebounceFiltersChanged();
    }

    partial void OnSelectedRoleChanged(string value)
    {
        DebounceFiltersChanged();
    }

    partial void OnSelectedBlockedStatusChanged(string value)
    {
        DebounceFiltersChanged();
    }

    private void DebounceFiltersChanged()
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();

        var token = _debounceCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(400, token);

                if (token.IsCancellationRequested)
                    return;

                await Dispatcher.UIThread.InvokeAsync(NotifyFiltersChangedAsync);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private async Task NotifyFiltersChangedAsync()
    {
        if (FiltersChanged != null)
            await FiltersChanged.Invoke();
    }
}