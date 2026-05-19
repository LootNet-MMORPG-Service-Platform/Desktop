using desktop_app.Services;
using System.Threading.Tasks;

namespace desktop_app.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public NotificationService Notifications { get; } = NotificationService.Instance;
    private readonly AuthTokenService _authTokenService = new();

    private ViewModelBase _currentViewModel = null!;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (SetProperty(ref _currentViewModel, value))
                OnPropertyChanged(nameof(IsLoggedIn));
        }
    }

    private AuthShellViewModel AuthShellVm { get; }
    private HomeViewModel HomeVm { get; }

    public bool IsLoggedIn => CurrentViewModel == HomeVm;

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

    public async Task AutoLogoutDueToInactivityAsync()
    {
        await LogoutAsync("You have been logged out due to inactivity.", true);
    }

    public async Task LogoutAsync(string message, bool isWarning)
    {
        await _authTokenService.ClearRefreshTokenAsync();
        HomeVm.ResetSessionState();
        ShowWelcome();

        if (isWarning)
            NotificationService.Instance.ShowWarning(message);
        else
            NotificationService.Instance.ShowInfo(message, "Logout");
    }
}
