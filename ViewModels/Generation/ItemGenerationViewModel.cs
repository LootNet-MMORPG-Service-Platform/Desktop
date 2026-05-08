using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using desktop_app.Models.Generation;
using desktop_app.Services.Generation;
using desktop_app.Enums;

namespace desktop_app.ViewModels.Generation;

public partial class ItemGenerationViewModel : ViewModelBase
{
    private GenerationAdminService _service;

    public ObservableCollection<GenerationProfile> Profiles { get; } = new();
    public ObservableCollection<GenerationRule> Rules { get; } = new();
    public ObservableCollection<TypeWeight> Weights { get; } = new();

    public bool HasRules => Rules.Count > 0;
    public bool HasWeights => Weights.Count > 0;
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
        RefreshDetailsState();

        var rules = await _service.GetRulesAsync(profile.Id);

        if (rules != null)
        {
            foreach (var r in rules)
                Rules.Add(r);
        }

        var weights = await _service.GetWeightsAsync(profile.Id);

        if (weights != null)
        {
            foreach (var w in weights)
                Weights.Add(w);
        }

        RefreshDetailsState();
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
            Profiles.Add(p);

        StatusMessage = $"Loaded {Profiles.Count} profiles.";
    }

    public async Task DeleteSelectedProfileAsync()
    {
        if (SelectedProfile == null)
            return;

        await _service.DeleteProfileAsync(SelectedProfile.Id);

        SelectedProfile = null;
        Rules.Clear();
        Weights.Clear();
        RefreshDetailsState();

        await LoadProfilesAsync();
    }

    public async Task CreateProfileAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        await _service.CreateProfileAsync(name.Trim());

        SelectedProfile = null;
        Rules.Clear();
        Weights.Clear();
        RefreshDetailsState();

        await LoadProfilesAsync();
    }
    
    public async Task CreateRuleAsync(ItemCategory category, WeaponType? weaponType, ArmorType? armorType, bool isFallback)
    {
        if (SelectedProfile == null)
            return;

        await _service.CreateRuleAsync(
            SelectedProfile.Id,
            category,
            weaponType,
            armorType,
            isFallback);

        await SelectProfile(SelectedProfile);
    }

    public async Task CreateTypeWeightAsync(
        ItemCategory category,
        WeaponType? weaponType,
        ArmorType? armorType,
        double weight)
    {
        if (SelectedProfile == null)
            return;

        await _service.CreateTypeWeightAsync(
            SelectedProfile.Id,
            category,
            weaponType,
            armorType,
            weight);

        await RefreshWeightsAsync();
    }

    public async Task DeleteTypeWeightAsync(TypeWeight weight)
    {
        await _service.DeleteTypeWeightAsync(weight.Id);

        await RefreshWeightsAsync();
    }
    
    public async Task DeleteRuleAsync(GenerationRule rule)
    {
        await _service.DeleteRuleAsync(rule.Id);

        if (SelectedProfile != null)
        {
            await SelectProfile(SelectedProfile);
        }
    }
    
    public async Task CreateParameterAsync(
        GenerationRule rule,
        ItemParameter parameter,
        List<CreateSegmentInput> segments)
    {
        await _service.CreateParameterAsync(
            rule.Id,
            parameter,
            segments);

        if (SelectedProfile != null)
        {
            await SelectProfile(SelectedProfile);
        }
    }
    
    public async Task CreateElementAsync(
        GenerationRule rule,
        ItemElementType elementType,
        List<CreateSegmentInput> segments)
    {
        await _service.CreateElementAsync(
            rule.Id,
            elementType,
            segments);

        if (SelectedProfile != null)
        {
            await SelectProfile(SelectedProfile);
        }
    }

    public void ClearSelection()
    {
        Rules.Clear();
        Weights.Clear();
        SelectedProfile = null;
        RefreshDetailsState();
    }

    [RelayCommand]
    private void CloseSelection()
    {
        ClearSelection();
    }

    partial void OnSelectedProfileChanged(GenerationProfile? value)
    {
        OnPropertyChanged(nameof(HasSelectedProfile));
    }

    private async Task RefreshWeightsAsync()
    {
        if (SelectedProfile == null)
            return;

        Weights.Clear();

        var weights = await _service.GetWeightsAsync(SelectedProfile.Id);

        if (weights != null)
        {
            foreach (var weight in weights)
                Weights.Add(weight);
        }

        RefreshDetailsState();
    }

    private void RefreshDetailsState()
    {
        OnPropertyChanged(nameof(HasRules));
        OnPropertyChanged(nameof(HasWeights));
    }
}
