using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using desktop_app.Models;
using desktop_app.Services;

namespace desktop_app.Views;

public partial class LogsView : UserControl
{
    public LogsView()
    {
        InitializeComponent();
    }

    private async void Details_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: AdminLog log })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        await DialogService.ShowReadonlyTextDialogAsync(
            owner,
            "Log data",
            log.DataDetailsText);
    }
}
