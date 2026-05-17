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
            Content = new Border
            {
                Background = GetBrush("WhiteBrush", Brushes.White),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 10 30 0 #14000000"),
                Child = content
            }
        };
    }
}