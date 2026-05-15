using System;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using desktop_app.Models;
using desktop_app.Services;
using desktop_app.ViewModels.Users;

namespace desktop_app.Views;

public partial class UsersView : UserControl
{
    private Window GetOwner() => (Window)VisualRoot!;

    public UsersView()
    {
        InitializeComponent();
    }

    private UsersViewModel? Vm => DataContext as UsersViewModel;

    private async void ToggleBlockStatus_Click(object? sender, RoutedEventArgs _)
    {
        if (sender is not Button btn || btn.DataContext is not AdminUser user || Vm == null)
            return;

        if (!user.IsBlocked)
        {
            var result = await DialogService.ShowConfirmDialogAsync(
                GetOwner(),
                "Are you sure you want to block this user?");

            if (!result)
                return;
        }

        await RunUsersActionAsync(() => Vm.ToggleBlockStatusCommand.ExecuteAsync(user));
    }

    private void SelectUser_Click(object? sender, RoutedEventArgs _)
    {
        if (sender is Button btn && btn.DataContext is AdminUser user)
            Vm?.SelectUserCommand.Execute(user);
    }

    private async void ChangeRole_Click(object? sender, RoutedEventArgs _)
    {
        if (Vm?.SelectedUser == null)
            return;

        var selectedUser = Vm.SelectedUser;

        var newRole = await DialogService.ShowChangeRoleDialogAsync(
            GetOwner(),
            selectedUser.Username,
            selectedUser.Role.ToString());

        if (string.IsNullOrWhiteSpace(newRole))
            return;

        await RunUsersActionAsync(() => Vm.ChangeRoleAsync(newRole));
    }

    private async void Inventory_Click(object? sender, RoutedEventArgs _)
    {
        await RunUsersActionAsync(async () =>
        {
            if (Vm == null)
                return;

            var items = await Vm.GetInventoryAsync();

            if (items == null)
                return;

            await DialogService.ShowInventoryDialogAsync(GetOwner(), items);
        });
    }

    private async void Equipment_Click(object? sender, RoutedEventArgs _)
    {
        await RunUsersActionAsync(async () =>
        {
            if (Vm == null)
                return;

            var eq = await Vm.GetEquipmentAsync();

            if (eq == null)
                return;

            await DialogService.ShowEquipmentDialogAsync(GetOwner(), eq);
        });
    }

    private static async Task RunUsersActionAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (HttpRequestException)
        {
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
        }
        catch (Exception)
        {
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }
}
