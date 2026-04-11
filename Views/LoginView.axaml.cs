using System;
using Avalonia.Controls;
using desktop_app.ViewModels;
using Avalonia.Interactivity;

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
            Console.WriteLine(ex); // tymczasowo
        }
    }
}