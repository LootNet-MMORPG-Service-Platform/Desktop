namespace desktop_app.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel = null!;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    private AuthShellViewModel AuthShellVm { get; }
    private HomeViewModel HomeVm { get; }

    public MainWindowViewModel()
    {
        AuthShellVm = new AuthShellViewModel(this);
        HomeVm = new HomeViewModel(this);

        CurrentViewModel = AuthShellVm;
    }

    public void ShowHome(string username)
    {
        HomeVm.SetUsername(username);
        CurrentViewModel = HomeVm;
    }
    
    public void ShowWelcome()
    {
        AuthShellVm.LoginVm.Reset();
        AuthShellVm.ShowWelcome();
        CurrentViewModel = AuthShellVm;
    }
}