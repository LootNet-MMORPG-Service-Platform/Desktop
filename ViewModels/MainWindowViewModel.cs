using desktop_app.Services;

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

    public void ShowHome(AuthService.LoginResult result)
    {
        HomeVm.SetUsername(result.Username);
        HomeVm.SetRole(result.Role.ToString());
        HomeVm.SetToken(result.Token);
        HomeVm.SetRefreshToken(result.RefreshToken);
        HomeVm.SetCurrentUserId(result.UserId);
        HomeVm.ShowDashboardCommand.Execute(null);
        CurrentViewModel = HomeVm;
    }
    
    public void ShowWelcome()
    {
        AuthShellVm.LoginVm.Reset();
        AuthShellVm.ShowWelcome();
        CurrentViewModel = AuthShellVm;
    }
}