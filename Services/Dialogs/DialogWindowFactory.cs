using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace desktop_app.Services;

public static class DialogWindowFactory
{
    public static IBrush GetBrush(string key, IBrush fallback)
    {
        if (Application.Current?.TryGetResource(key, Application.Current.ActualThemeVariant, out var value) == true
            && value is IBrush brush)
        {
            return brush;
        }

        return fallback;
    }

    public static Window CreateBaseDialog(Control content, double width, double height)
    {
        return new Window
        {
            Width = width,
            Height = height,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = GetBrush("SurfaceBrush", Brushes.White),
            Content = new Border
            {
                Background = GetBrush("SurfaceBrush", Brushes.White),
                BorderBrush = GetBrush("SidebarBorderBrush", Brushes.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 12 40 0 #66000000"),
                Child = content
            }
        };
    }
}