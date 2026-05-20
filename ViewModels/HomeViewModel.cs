using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using desktop_app.Services;
using desktop_app.ViewModels.Users;
using desktop_app.Services.Generation;
using desktop_app.ViewModels.Generation;
using desktop_app.Services.Economy;
using desktop_app.ViewModels.Economy;
using desktop_app.ViewModels.Logs;

namespace desktop_app.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public MainWindowViewModel Parent { get; }

    private AdminService _adminService;
    private ItemGenerationAdminService _itemGenerationAdminService;
    private EconomyAdminService _economyAdminService;
    private readonly AuthService _authService;
    
    private string _token = "";

    public UsersViewModel UsersVm { get; }
    public ItemGenerationViewModel ItemGenerationVm { get; }
    public EconomyViewModel EconomyVm { get; }
    public LogsViewModel LogsVm { get; }

    public HomeViewModel(MainWindowViewModel parent)
    {
        Parent = parent;
        _adminService = null!;
        _authService = new AuthService();
        
        _itemGenerationAdminService = null!;
        ItemGenerationVm = new ItemGenerationViewModel(_itemGenerationAdminService);
        
        _economyAdminService = null!;
        EconomyVm = new EconomyViewModel(_economyAdminService);

        UsersVm = new UsersViewModel(
            _adminService,
            _authService,
            () => Parent.ShowWelcome());

        LogsVm = new LogsViewModel(_adminService);
    }

    public bool CanAccessUsers => Role == "SuperAdmin" || Role == "Admin";
    public bool CanAccessItemGeneration => Role == "SuperAdmin" || Role == "GameModerator";

    public bool IsDashboardActive => ActiveSection == "Dashboard";
    public bool IsUsersActive => ActiveSection == "Users";
    public bool IsItemGenerationActive => ActiveSection == "ItemGeneration";
    public bool IsEconomyActive => ActiveSection == "Economy";
    public bool IsLogsActive => ActiveSection == "Logs";

    [ObservableProperty]
    private string _username = "admin";

    [ObservableProperty]
    private string _role = "";

    [ObservableProperty]
    private string _activeSection = "Dashboard";

    [ObservableProperty]
    private string _currentSectionTitle = "Dashboard";

    [ObservableProperty]
    private string _currentSectionDescription =
        "Overview of the administrative panel and key management areas.";

    [ObservableProperty]
    private string _currentSectionMessage =
        "Choose a section from the sidebar to manage the game as an administrator.";

    [RelayCommand]
    private void ShowDashboard()
    {
        UsersVm.ClearSelection();
        ItemGenerationVm.ClearSelection();
        
        ActiveSection = "Dashboard";
        CurrentSectionTitle = "Dashboard";
        CurrentSectionDescription = "Overview of the administrative panel and key management areas.";
        CurrentSectionMessage = "Choose a section from the sidebar to manage the game as an administrator.";
    }

    [RelayCommand]
    private async Task ShowUsers()
    {
        ItemGenerationVm.ClearSelection();
        
        ActiveSection = "Users";
        CurrentSectionTitle = "Users";
        CurrentSectionDescription = "Manage user accounts, roles and access.";

        await UsersVm.LoadUsersAsync();
        CurrentSectionMessage = UsersVm.StatusMessage;
    }

    [RelayCommand]
    private async Task ShowItemGeneration()
    {
        UsersVm.ClearSelection();

        ActiveSection = "ItemGeneration";
        CurrentSectionTitle = "Item Generation";
        CurrentSectionDescription = "Manage item generation profiles, rules and type weights.";

        await ItemGenerationVm.LoadProfilesAsync();
    }

    [RelayCommand]
    private async Task ShowEconomy()
    {
        ItemGenerationVm.ClearSelection();
        UsersVm.ClearSelection();
        
        ActiveSection = "Economy";
        CurrentSectionTitle = "Economy";
        CurrentSectionDescription = "Manage economy settings and game balance.";

        await EconomyVm.LoadAsync();
    }

    [RelayCommand]
    private async Task ShowLogs()
    {
        UsersVm.ClearSelection();
        ItemGenerationVm.ClearSelection();
        
        ActiveSection = "Logs";
        CurrentSectionTitle = "Logs";
        CurrentSectionDescription = "Review logs, statistics and important system events.";

        await LogsVm.LoadLogsAsync();
    }

    public void SetRole(string role)
    {
        Role = role;
    }

    public void SetUsername(string username)
    {
        Username = username;
    }

    public void SetCurrentUserId(string userId)
    {
        UsersVm.SetCurrentUserId(userId);
    }

    public void SetToken(string token)
    {
        _token = token;

        _adminService = new AdminService(_token);
        _itemGenerationAdminService = new ItemGenerationAdminService(_token);
        _economyAdminService = new EconomyAdminService(_token);

        UsersVm.UpdateAdminService(_adminService);
        ItemGenerationVm.UpdateService(_itemGenerationAdminService);
        EconomyVm.UpdateService(_economyAdminService);
        LogsVm.UpdateAdminService(_adminService);
    }

    public void SetRefreshToken(string refreshToken)
    {
        UsersVm.SetRefreshToken(refreshToken);
    }

    [RelayCommand]
    private async Task Logout()
    {
        await Parent.LogoutAsync("You have been logged out.", false);
    }

    public void ResetSessionState()
    {
        _token = "";
        Username = "admin";
        Role = "";
        UsersVm.ResetSessionState();
        ItemGenerationVm.ResetSessionState();
        EconomyVm.ResetSessionState();
        LogsVm.ResetSessionState();

        ActiveSection = "Dashboard";
        CurrentSectionTitle = "Dashboard";
        CurrentSectionDescription = "Overview of the administrative panel and key management areas.";
        CurrentSectionMessage = "Choose a section from the sidebar to manage the game as an administrator.";
    }

    public async Task ChangePasswordAsync(string oldPassword, string newPassword)
    {
        try
        {
            await _authService.ResetPasswordAsync(oldPassword, newPassword, _token);
            NotificationService.Instance.ShowSuccess("Password changed.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            NotificationService.Instance.ShowError("Wrong password.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == null)
        {
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
        }
        catch (HttpRequestException)
        {
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }

    partial void OnActiveSectionChanged(string value)
    {
        _ = value;

        OnPropertyChanged(nameof(IsDashboardActive));
        OnPropertyChanged(nameof(IsUsersActive));
        OnPropertyChanged(nameof(IsItemGenerationActive));
        OnPropertyChanged(nameof(IsEconomyActive));
        OnPropertyChanged(nameof(IsLogsActive));
    }

    partial void OnRoleChanged(string value)
    {
        _ = value;
        OnPropertyChanged(nameof(CanAccessUsers));
        OnPropertyChanged(nameof(CanAccessItemGeneration));
    }
}
