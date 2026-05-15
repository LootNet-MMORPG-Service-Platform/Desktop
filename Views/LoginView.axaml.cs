using System;
using Avalonia.Controls;
using desktop_app.ViewModels;
using Avalonia.Interactivity;
using desktop_app.Services;

namespace desktop_app.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();

        BackButton.Click += (_, _) =>
        {
            if (DataContext is LoginViewModel loginVm)
            {
                loginVm.Parent.ShowWelcome();
            }
        };
    }
    
    private async void LoginButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is LoginViewModel loginVm)
            {
                await loginVm.LoginAsync();
            }
        }
        catch (Exception ex)
        {
            NotificationService.Instance.ShowApiError(ex, "Operation failed. Please try again.");
        }
    }
}
