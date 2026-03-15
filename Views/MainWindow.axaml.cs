using Avalonia.Controls;

namespace desktop_app.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Opened += (_, _) =>
        {
            WindowState = WindowState.Maximized;
        };
    }
}