using Avalonia.Controls;
using desktop_app.ViewModels;

namespace desktop_app.Views;

public partial class WelcomeView : UserControl
{
    public WelcomeView()
    {
        InitializeComponent();

        GoToLoginButton.Click += (_, _) =>
        {
            if (DataContext is WelcomeViewModel welcomeVm)
            {
                welcomeVm.Parent.ShowLogin();
            }
        };
    }
}