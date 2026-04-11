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

    public LoginViewModel(AuthShellViewModel parent, MainWindowViewModel mainWindow)
    {
        Parent = parent;
        _mainWindow = mainWindow;
        _authService = new AuthService();
    }

    public async Task LoginAsync()
    {
        ErrorMessage = "";

        var result = await _authService.LoginAsync(Username, Password);

        if (result == null || result.Role != 0)
        {
            ErrorMessage = "Invalid username or password";
            return;
        }

        _mainWindow.ShowHome(result.Username);
    }
    
    public void Reset()
    {
        Username = "";
        Password = "";
        ErrorMessage = "";
    }
}