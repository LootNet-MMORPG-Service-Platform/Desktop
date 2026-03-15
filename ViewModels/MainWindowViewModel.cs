namespace desktop_app.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel = null!;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    private WelcomeViewModel WelcomeVm { get; }
    private LoginViewModel LoginVm { get; }

    public MainWindowViewModel()
    {
        WelcomeVm = new WelcomeViewModel(this);
        LoginVm = new LoginViewModel(this);

        CurrentViewModel = WelcomeVm;
    }

    public void ShowLogin()
    {
        CurrentViewModel = LoginVm;
    }

    public void ShowWelcome()
    {
        CurrentViewModel = WelcomeVm;
    }
}