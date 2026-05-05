using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using desktop_app.Services;
using desktop_app.ViewModels.Users;
using desktop_app.Services.Generation;
using desktop_app.ViewModels.Generation;

namespace desktop_app.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public MainWindowViewModel Parent { get; }

    private AdminService _adminService;
    private GenerationAdminService _generationAdminService;
    
    private string _token = "";

    public UsersViewModel UsersVm { get; }
    public ItemGenerationViewModel ItemGenerationVm { get; }

    public HomeViewModel(MainWindowViewModel parent)
    {
        Parent = parent;
        _adminService = null!;
        var authService = new AuthService();
        
        _generationAdminService = null!;
        ItemGenerationVm = new ItemGenerationViewModel(_generationAdminService);

        UsersVm = new UsersViewModel(
            _adminService,
            authService,
            () => Parent.ShowWelcome());
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
    private void ShowEconomy()
    {
        ItemGenerationVm.ClearSelection();
        UsersVm.ClearSelection();
        
        ActiveSection = "Economy";
        CurrentSectionTitle = "Economy";
        CurrentSectionDescription = "Manage economy settings and game balance.";
        CurrentSectionMessage = "Economy section is ready for future API integration.";
    }

    [RelayCommand]
    private void ShowLogs()
    {
        UsersVm.ClearSelection();
        ItemGenerationVm.ClearSelection();
        
        ActiveSection = "Logs";
        CurrentSectionTitle = "Logs";
        CurrentSectionDescription = "Review logs, statistics and important system events.";
        CurrentSectionMessage = "Logs section is ready for future API integration.";
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
        _generationAdminService = new GenerationAdminService(_token);

        UsersVm.UpdateAdminService(_adminService);
        ItemGenerationVm.UpdateService(_generationAdminService);
    }

    public void SetRefreshToken(string refreshToken)
    {
        UsersVm.SetRefreshToken(refreshToken);
    }

    [RelayCommand]
    private void Logout()
    {
        Parent.ShowWelcome();
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