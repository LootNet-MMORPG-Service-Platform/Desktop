using System.Threading.Tasks;
using desktop_app.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace desktop_app.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    public AuthShellViewModel Parent { get; }
    private readonly MainWindowViewModel _mainWindow;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _errorMessage = "";

    private readonly AuthService _authService;
    private readonly AuthTokenService _authTokenService;

    public LoginViewModel(AuthShellViewModel parent, MainWindowViewModel mainWindow)
    {
        Parent = parent;
        _mainWindow = mainWindow;
        _authService = new AuthService();
        _authTokenService = new AuthTokenService();
    }

    public async Task LoginAsync()
    {
        ErrorMessage = "";

        var result = await _authService.LoginAsync(Username, Password);

        if (result == null ||
            result.Role == Models.UserRole.Player)
        {
            ErrorMessage = "Invalid username or password";
            return;
        }

        await _authTokenService.SaveRefreshTokenAsync(result.RefreshToken);

        _mainWindow.ShowHome(result);
        NotificationService.Instance.ShowSuccess($"Welcome, {result.Username}.", "Login successful");
    }
    
    public void Reset()
    {
        Username = "";
        Password = "";
        ErrorMessage = "";
    }
}
