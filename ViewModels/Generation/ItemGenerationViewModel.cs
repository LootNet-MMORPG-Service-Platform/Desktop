using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using desktop_app.Models.Generation;
using desktop_app.Services;
using desktop_app.Services.Generation;
using desktop_app.Enums;

namespace desktop_app.ViewModels.Generation;

public partial class ItemGenerationViewModel : ViewModelBase
{
    private ItemGenerationAdminService _service;

    public ObservableCollection<GenerationProfile> Profiles { get; } = new();
    public ObservableCollection<GenerationRule> Rules { get; } = new();
    public ObservableCollection<TypeWeight> Weights { get; } = new();

    public bool HasRules => Rules.Count > 0;
    public bool HasWeights => Weights.Count > 0;
    public bool HasSelectedProfile => SelectedProfile != null;
    public bool IsProfileIncomplete =>
        HasSelectedProfile &&
        (!HasWeights ||
         !HasRules ||
         Rules.Any(rule => !rule.HasParameters || !rule.HasElements));

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private GenerationProfile? _selectedProfile;

    public ItemGenerationViewModel(ItemGenerationAdminService service)
    {
        _service = service;
    }

    public void UpdateService(ItemGenerationAdminService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task SelectProfile(GenerationProfile profile)
    {
        try
        {
            SelectedProfile = profile;

            Rules.Clear();
            Weights.Clear();
            RefreshDetailsState();

            var rules = await _service.GetRulesAsync(profile.Id);

            if (rules != null)
            {
                foreach (var r in rules)
                {
                    r.Parameters = await _service.GetParametersAsync(r.Id) ?? new List<GenerationParameter>();
                    r.Elements = await _service.GetElementsAsync(r.Id) ?? new List<GenerationElement>();
                    Rules.Add(r);
                }
            }

            var weights = await _service.GetWeightsAsync(profile.Id);

            if (weights != null)
            {
                foreach (var w in weights)
                    Weights.Add(w);
            }

            RefreshDetailsState();
        }
        catch (HttpRequestException)
        {
            StatusMessage = "Failed to load profile details.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running and verify your internet connection.");
        }
        catch (Exception)
        {
            StatusMessage = "Failed to load profile details.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }

    [RelayCommand]
    public async Task LoadProfilesAsync()
    {
        try
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
        catch (HttpRequestException)
        {
            StatusMessage = "Failed to load profiles.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running and verify your internet connection.");
        }
        catch (Exception)
        {
            StatusMessage = "Failed to load profiles.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
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

    public async Task UpdateSelectedProfileAsync(string name)
    {
        if (SelectedProfile == null || string.IsNullOrWhiteSpace(name))
            return;

        var profileId = SelectedProfile.Id;

        await _service.UpdateProfileAsync(profileId, name.Trim());

        await LoadProfilesAsync();

        SelectedProfile = Profiles.FirstOrDefault(profile => profile.Id == profileId);
        RefreshDetailsState();
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

    public async Task UpdateTypeWeightAsync(
        TypeWeight typeWeight,
        ItemCategory category,
        WeaponType? weaponType,
        ArmorType? armorType,
        double weight)
    {
        await _service.UpdateTypeWeightAsync(
            typeWeight.Id,
            category,
            weaponType,
            armorType,
            weight);

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

    public async Task UpdateRuleAsync(
        GenerationRule rule,
        ItemCategory category,
        WeaponType? weaponType,
        ArmorType? armorType,
        bool isFallback)
    {
        await _service.UpdateRuleAsync(
            rule.Id,
            category,
            weaponType,
            armorType,
            isFallback);

        await RefreshRulesAsync();
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

        await RefreshParametersAsync(rule);
    }

    public async Task DeleteParameterAsync(GenerationParameter parameter)
    {
        var rule = Rules.FirstOrDefault(r => r.Parameters.Any(p => p.Id == parameter.Id));

        await _service.DeleteParameterAsync(parameter.Id);

        if (rule != null)
        {
            await RefreshParametersAsync(rule);
        }
    }

    public async Task UpdateParameterAsync(
        GenerationParameter parameter,
        ItemParameter itemParameter,
        List<CreateSegmentInput> segments)
    {
        var rule = Rules.FirstOrDefault(r => r.Parameters.Any(p => p.Id == parameter.Id));

        await _service.UpdateParameterAsync(
            parameter.Id,
            itemParameter,
            segments);

        if (rule != null)
        {
            await RefreshParametersAsync(rule);
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

        await RefreshElementsAsync(rule);
    }

    public async Task DeleteElementAsync(GenerationElement element)
    {
        var rule = Rules.FirstOrDefault(r => r.Elements.Any(e => e.Id == element.Id));

        await _service.DeleteElementAsync(element.Id);

        if (rule != null)
        {
            await RefreshElementsAsync(rule);
        }
    }

    public async Task UpdateElementAsync(
        GenerationElement element,
        ItemElementType elementType,
        List<CreateSegmentInput> segments)
    {
        var rule = Rules.FirstOrDefault(r => r.Elements.Any(e => e.Id == element.Id));

        await _service.UpdateElementAsync(
            element.Id,
            elementType,
            segments);

        if (rule != null)
        {
            await RefreshElementsAsync(rule);
        }
    }

    public void ClearSelection()
    {
        Rules.Clear();
        Weights.Clear();
        SelectedProfile = null;
        RefreshDetailsState();
    }

    public void ResetSessionState()
    {
        Profiles.Clear();
        Rules.Clear();
        Weights.Clear();
        SelectedProfile = null;
        StatusMessage = "";
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

    private async Task RefreshRulesAsync()
    {
        if (SelectedProfile == null)
            return;

        Rules.Clear();

        var rules = await _service.GetRulesAsync(SelectedProfile.Id);

        if (rules != null)
        {
            foreach (var rule in rules)
            {
                rule.Parameters = await _service.GetParametersAsync(rule.Id) ?? new List<GenerationParameter>();
                rule.Elements = await _service.GetElementsAsync(rule.Id) ?? new List<GenerationElement>();
                Rules.Add(rule);
            }
        }

        RefreshDetailsState();
    }

    private async Task RefreshParametersAsync(GenerationRule rule)
    {
        var parameters = await _service.GetParametersAsync(rule.Id);

        var ruleIndex = Rules.IndexOf(rule);

        if (ruleIndex >= 0)
        {
            Rules[ruleIndex] = new GenerationRule
            {
                Id = rule.Id,
                Category = rule.Category,
                WeaponType = rule.WeaponType,
                ArmorType = rule.ArmorType,
                IsFallback = rule.IsFallback,
                Parameters = parameters ?? new List<GenerationParameter>(),
                Elements = rule.Elements
            };
        }

        RefreshDetailsState();
    }

    private async Task RefreshElementsAsync(GenerationRule rule)
    {
        var elements = await _service.GetElementsAsync(rule.Id);

        var ruleIndex = Rules.IndexOf(rule);

        if (ruleIndex >= 0)
        {
            Rules[ruleIndex] = new GenerationRule
            {
                Id = rule.Id,
                Category = rule.Category,
                WeaponType = rule.WeaponType,
                ArmorType = rule.ArmorType,
                IsFallback = rule.IsFallback,
                Parameters = rule.Parameters,
                Elements = elements ?? new List<GenerationElement>()
            };
        }

        RefreshDetailsState();
    }

    private void RefreshDetailsState()
    {
        OnPropertyChanged(nameof(HasRules));
        OnPropertyChanged(nameof(HasWeights));
        OnPropertyChanged(nameof(IsProfileIncomplete));
    }
}
