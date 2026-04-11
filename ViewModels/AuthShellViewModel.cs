namespace desktop_app.ViewModels;

public class AuthShellViewModel : ViewModelBase
{
    private ViewModelBase _currentAuthViewModel = null!;

    public ViewModelBase CurrentAuthViewModel
    {
        get => _currentAuthViewModel;
        set => SetProperty(ref _currentAuthViewModel, value);
    }

    public WelcomeViewModel WelcomeVm { get; }
    public LoginViewModel LoginVm { get; }

    public AuthShellViewModel(MainWindowViewModel mainWindow)
    {
        WelcomeVm = new WelcomeViewModel(this);
        LoginVm = new LoginViewModel(this, mainWindow);

        CurrentAuthViewModel = WelcomeVm;
    }

    public void ShowWelcome()
    {
        CurrentAuthViewModel = WelcomeVm;
    }

    public void ShowLogin()
    {
        CurrentAuthViewModel = LoginVm;
    }
}