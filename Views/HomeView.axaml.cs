using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using desktop_app.Services;
using desktop_app.ViewModels;

namespace desktop_app.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private async void ChangePassword_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not HomeViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowChangePasswordDialogAsync(owner);

        if (result == null)
            return;

        await vm.ChangePasswordAsync(result.OldPassword, result.NewPassword);
    }
}
