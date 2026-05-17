using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia;

namespace desktop_app.Services.Dialogs.Generation;

public static class GenerationDialogUiFactory
{
    public static IBrush GetBrush(string key, IBrush fallback)
    {
        return DialogWindowFactory.GetBrush(key, fallback);
    }

    public static Window CreateBaseDialog(Control content, double width, double height)
    {
        return DialogWindowFactory.CreateBaseDialog(content, width, height);
    }

    public static Button CreateDialogButton(string text, string className)
    {
        return new Button
        {
            Content = text,
            Width = 90,
            Height = 36,
            Classes = { className }
        };
    }

    public static StackPanel CreateButtonRow(params Button[] buttons)
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 12,
            Margin = new Thickness(0, 6, 0, 0)
        };

        foreach (var button in buttons)
            row.Children.Add(button);

        return row;
    }
}
