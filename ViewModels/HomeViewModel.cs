using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace desktop_app.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public MainWindowViewModel Parent { get; }

    public HomeViewModel(MainWindowViewModel parent)
    {
        Parent = parent;
    }
    
    public bool IsDashboardActive => ActiveSection == "Dashboard";
    public bool IsUsersActive => ActiveSection == "Users";
    public bool IsItemsActive => ActiveSection == "Items";
    public bool IsEconomyActive => ActiveSection == "Economy";
    public bool IsLogsActive => ActiveSection == "Logs";
    
    [ObservableProperty]
    private string _username = "admin";
    
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
        ActiveSection = "Dashboard";
        CurrentSectionTitle = "Dashboard";
        CurrentSectionDescription = "Overview of the administrative panel and key management areas.";
        CurrentSectionMessage = "Choose a section from the sidebar to manage the game as an administrator.";
    }

    [RelayCommand]
    private void ShowUsers()
    {
        ActiveSection = "Users";
        CurrentSectionTitle = "Users";
        CurrentSectionDescription = "Manage user accounts, roles and access.";
        CurrentSectionMessage = "User management section is ready for future API integration.";
    }

    [RelayCommand]
    private void ShowItems()
    {
        ActiveSection = "Items";
        CurrentSectionTitle = "Items";
        CurrentSectionDescription = "Manage item generation and item properties.";
        CurrentSectionMessage = "Items section is ready for future API integration.";
    }

    [RelayCommand]
    private void ShowEconomy()
    {
        ActiveSection = "Economy";
        CurrentSectionTitle = "Economy";
        CurrentSectionDescription = "Manage economy settings and game balance.";
        CurrentSectionMessage = "Economy section is ready for future API integration.";
    }

    [RelayCommand]
    private void ShowLogs()
    {
        ActiveSection = "Logs";
        CurrentSectionTitle = "Logs";
        CurrentSectionDescription = "Review logs, statistics and important system events.";
        CurrentSectionMessage = "Logs section is ready for future API integration.";
    }
    
    public void SetUsername(string username)
    {
        Username = username;
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
        OnPropertyChanged(nameof(IsItemsActive));
        OnPropertyChanged(nameof(IsEconomyActive));
        OnPropertyChanged(nameof(IsLogsActive));
    }
}