using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using desktop_app.Models.Generation;
using desktop_app.Services.Generation;

namespace desktop_app.ViewModels.Generation;

public partial class ItemGenerationViewModel : ViewModelBase
{
    private GenerationAdminService _service;

    public ObservableCollection<GenerationProfile> Profiles { get; } = new();
    
    public ObservableCollection<GenerationRule> Rules { get; } = new();
    
    public ObservableCollection<TypeWeight> Weights { get; } = new();
    
    public bool HasSelectedProfile => SelectedProfile != null;

    [ObservableProperty]
    private string _statusMessage = "";
    
    [ObservableProperty]
    private GenerationProfile? _selectedProfile;

    public ItemGenerationViewModel(GenerationAdminService service)
    {
        _service = service;
    }

    public void UpdateService(GenerationAdminService service)
    {
        _service = service;
    }
    
    [RelayCommand]
    private async Task SelectProfile(GenerationProfile profile)
    {
        SelectedProfile = profile;

        Rules.Clear();
        Weights.Clear();

        var rules = await _service.GetRulesAsync(profile.Id);

        if (rules != null)
        {
            foreach (var r in rules)
            {
                Rules.Add(r);
            }
        }

        var weights = await _service.GetWeightsAsync(profile.Id);

        if (weights != null)
        {
            foreach (var w in weights)
            {
                Weights.Add(w);
            }
        }
    }

    [RelayCommand]
    public async Task LoadProfilesAsync()
    {
        StatusMessage = "Loading profiles...";

        Profiles.Clear();

        var result = await _service.GetProfilesAsync();

        if (result == null)
        {
            StatusMessage = "Failed to load profiles.";
            return;
        }

        foreach (var p in result)
        {
            Profiles.Add(p);
        }

        StatusMessage = $"Loaded {Profiles.Count} profiles.";
    }
    
    partial void OnSelectedProfileChanged(GenerationProfile? value)
    {
        OnPropertyChanged(nameof(HasSelectedProfile));
    }
    
    public void ClearSelection()
    {
        Rules.Clear();
        Weights.Clear();
        SelectedProfile = null;
    }
    
    [RelayCommand]
    private void CloseSelection()
    {
        SelectedProfile = null;
    }
}