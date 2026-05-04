using System.Threading.Tasks;
using Avalonia.Controls;
using desktop_app.Models;

namespace desktop_app.Services;

public static class DialogService
{
    public static Task<bool> ShowConfirmDialogAsync(Window owner, string message)
    {
        return CommonDialogService.ShowConfirmDialogAsync(owner, message);
    }

    public static Task ShowErrorDialogAsync(Window owner, string message)
    {
        return CommonDialogService.ShowErrorDialogAsync(owner, message);
    }

    public static Task<string?> ShowChangeRoleDialogAsync(Window owner, string username, string currentRole)
    {
        return UserDialogService.ShowChangeRoleDialogAsync(owner, username, currentRole);
    }

    public static Task ShowInventoryDialogAsync(Window owner, ItemCollectionDTO items)
    {
        return UserDialogService.ShowInventoryDialogAsync(owner, items);
    }

    public static Task ShowEquipmentDialogAsync(Window owner, EquipmentResponseDTO eq)
    {
        return UserDialogService.ShowEquipmentDialogAsync(owner, eq);
    }
}