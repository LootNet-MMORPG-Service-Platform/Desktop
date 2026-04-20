using CommunityToolkit.Mvvm.ComponentModel;

namespace desktop_app.Models;

public partial class AdminUser : ObservableObject
{
    public string Id { get; set; } = "";
    public string Username { get; set; } = "";
    public UserRole Role { get; set; }
    public decimal Currency { get; set; }
    public bool IsBlocked { get; set; }

    [ObservableProperty]
    private bool _isSelf;

    [ObservableProperty]
    private bool _isBusy;

    public bool CanToggleBlock => !IsSelf && !IsBusy;

    partial void OnIsSelfChanged(bool value)
    {
        _ = value;
        OnPropertyChanged(nameof(CanToggleBlock));
    }

    partial void OnIsBusyChanged(bool value)
    {
        _ = value;
        OnPropertyChanged(nameof(CanToggleBlock));
    }
}