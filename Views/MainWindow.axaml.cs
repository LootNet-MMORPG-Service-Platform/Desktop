using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using desktop_app.ViewModels;
using System;

namespace desktop_app.Views;

public partial class MainWindow : Window
{
    private static readonly TimeSpan InactivityTimeout = TimeSpan.FromMinutes(5);
    private readonly DispatcherTimer _inactivityTimer;
    private DateTime _lastActivityUtc = DateTime.UtcNow;
    private bool _autoLogoutInProgress;

    public MainWindow()
    {
        InitializeComponent();

        AddHandler(InputElement.PointerMovedEvent, OnActivity, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(InputElement.PointerPressedEvent, OnActivity, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(InputElement.PointerReleasedEvent, OnActivity, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(InputElement.KeyDownEvent, OnActivity, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(InputElement.TextInputEvent, OnActivity, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);

        _inactivityTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _inactivityTimer.Tick += InactivityTimer_Tick;
        _inactivityTimer.Start();

        Opened += (_, _) =>
        {
            WindowState = WindowState.Maximized;
        };
    }

    private void OnActivity(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        _lastActivityUtc = DateTime.UtcNow;
    }

    private async void InactivityTimer_Tick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;

        if (_autoLogoutInProgress)
            return;

        if (DataContext is not MainWindowViewModel vm || !vm.IsLoggedIn)
        {
            _lastActivityUtc = DateTime.UtcNow;
            return;
        }

        if (DateTime.UtcNow - _lastActivityUtc < InactivityTimeout)
            return;

        _autoLogoutInProgress = true;

        try
        {
            await vm.AutoLogoutDueToInactivityAsync();
            _lastActivityUtc = DateTime.UtcNow;
        }
        finally
        {
            _autoLogoutInProgress = false;
        }
    }
}
