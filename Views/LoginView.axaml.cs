using Avalonia.Controls;
using desktop_app.ViewModels;

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
}