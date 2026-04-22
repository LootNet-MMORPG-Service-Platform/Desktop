using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using desktop_app.Models;
using desktop_app.ViewModels;
using desktop_app.Services;

namespace desktop_app.Views;

public partial class HomeView : UserControl
{
    private Window GetOwner() => (Window)VisualRoot!;
    
    public HomeView()
    {
        InitializeComponent();
    }

    private async void ToggleBlockStatus_Click(object? sender, RoutedEventArgs _)
    {
        if (sender is Button btn && btn.DataContext is AdminUser user)
        {
            if (DataContext is HomeViewModel vm)
            {
                if (!user.IsBlocked)
                {
                    var result = await DialogService.ShowConfirmDialogAsync(GetOwner(), 
                        "Are you sure you want to block this user?");
                    if (!result)
                        return;
                }

                await vm.UsersVm.ToggleBlockStatusCommand.ExecuteAsync(user);
            }
        }
    }
    
    private void SelectUser_Click(object? sender, RoutedEventArgs _)
    {
        if (sender is Button btn && btn.DataContext is AdminUser user)
        {
            if (DataContext is HomeViewModel vm)
            {
                vm.UsersVm.SelectUserCommand.Execute(user);
            }
        }
    }
    
    private async void ChangeRole_Click(object? sender, RoutedEventArgs _)
    {
        try
        {
            if (DataContext is not HomeViewModel vm || vm.UsersVm.SelectedUser == null)
                return;

            var selectedUser = vm.UsersVm.SelectedUser;

            var newRole = await DialogService.ShowChangeRoleDialogAsync(
                GetOwner(),
                selectedUser.Username,
                selectedUser.Role.ToString()
            );

            if (string.IsNullOrWhiteSpace(newRole))
                return;

            await vm.UsersVm.ChangeRoleAsync(newRole);
        }
        catch (Exception ex)
        {
            await DialogService.ShowErrorDialogAsync(GetOwner(), ex.ToString());
        }
    }
    
    private async void Inventory_Click(object? sender, RoutedEventArgs _)
    {
        try
        {
            if (DataContext is not HomeViewModel vm)
                return;

            var items = await vm.UsersVm.GetInventoryAsync();

            if (items == null)
                return;

            await DialogService.ShowInventoryDialogAsync(GetOwner(), items);
        }
        catch (Exception ex)
        {
            await DialogService.ShowErrorDialogAsync(GetOwner(), ex.ToString());
        }
    }
    
    private async void Equipment_Click(object? sender, RoutedEventArgs _)
    {
        try
        {
            if (DataContext is not HomeViewModel vm)
                return;

            var eq = await vm.UsersVm.GetEquipmentAsync();

            if (eq == null)
                return;

            await DialogService.ShowEquipmentDialogAsync(GetOwner(), eq);
        }
        catch (Exception ex)
        {
            await DialogService.ShowErrorDialogAsync(GetOwner(), ex.ToString());
        }
    }
}