using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using desktop_app.Enums;
using desktop_app.Models.EnemyGeneration;
using desktop_app.Services;
using desktop_app.Services.Generation;

namespace desktop_app.ViewModels.Generation;

public partial class EnemyGenerationViewModel : ViewModelBase
{
    private EnemyGenerationAdminService _service;
    private readonly Dictionary<Guid, string> _classProfileNames = new();
    private readonly Dictionary<Guid, string> _stageProfileNames = new();

    public ObservableCollection<StageProfile> Profiles { get; } = new();
    public ObservableCollection<StageScenario> Scenarios { get; } = new();
    public ObservableCollection<ScenarioSlot> Slots { get; } = new();
    public ObservableCollection<EnemyClassProfile> ClassProfiles { get; } = new();

    public bool HasProfiles => Profiles.Count > 0;
    public bool HasScenarios => Scenarios.Count > 0;
    public bool HasSlots => Slots.Count > 0;
    public bool HasSelectedProfile => SelectedProfile != null;
    public bool HasSelectedScenario => SelectedScenario != null;
    public bool HasSelectedScenarioWithoutSlots => HasSelectedScenario && !HasSlots;
    public bool HasClassProfiles => ClassProfiles.Count > 0;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private StageProfile? _selectedProfile;

    [ObservableProperty]
    private StageScenario? _selectedScenario;

    public EnemyGenerationViewModel(EnemyGenerationAdminService service)
    {
        _service = service;
    }

    public void UpdateService(EnemyGenerationAdminService service)
    {
        _service = service;
    }

    public async Task CreateStageProfileAsync(
        string name,
        int stageIndex,
        double weight,
        double falloff,
        int threshold)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        await _service.CreateStageProfileAsync(
            name.Trim(),
            stageIndex,
            weight,
            falloff,
            threshold);

        await LoadProfilesAsync();
    }

    public async Task DeleteStageProfileAsync(StageProfile profile)
    {
        await _service.DeleteStageProfileAsync(profile.Id);

        if (SelectedProfile?.Id == profile.Id)
            ClearSelection();

        await LoadProfilesAsync();
    }

    public async Task UpdateStageProfileAsync(
        StageProfile profile,
        string name,
        int stageIndex,
        double weight,
        double falloff,
        int threshold)
    {
        await _service.UpdateStageProfileAsync(
            profile.Id,
            name.Trim(),
            stageIndex,
            weight,
            falloff,
            threshold);

        await LoadProfilesAsync();
    }

    public async Task CreateStageScenarioAsync(int enemyCount, double weight)
    {
        if (SelectedProfile == null)
            return;

        await _service.CreateStageScenarioAsync(
            SelectedProfile.Id,
            enemyCount,
            weight);

        await RefreshScenariosAsync();
    }

    public async Task DeleteStageScenarioAsync(StageScenario scenario)
    {
        await _service.DeleteStageScenarioAsync(scenario.Id);

        if (SelectedScenario?.Id == scenario.Id)
        {
            SelectedScenario = null;
            Slots.Clear();
        }

        await RefreshScenariosAsync();
    }

    public async Task UpdateStageScenarioAsync(StageScenario scenario, int enemyCount, double weight)
    {
        await _service.UpdateStageScenarioAsync(
            scenario.Id,
            enemyCount,
            weight);

        var wasSelectedScenario = SelectedScenario?.Id == scenario.Id;

        await RefreshScenariosAsync();

        if (wasSelectedScenario)
            SelectedScenario = Scenarios.FirstOrDefault(x => x.Id == scenario.Id);
    }

    public async Task CreateScenarioSlotAsync(int position, Guid classProfileId, double weight)
    {
        if (SelectedScenario == null)
            return;

        await _service.CreateScenarioSlotAsync(
            SelectedScenario.Id,
            position,
            classProfileId,
            weight);

        await RefreshSlotsAsync();
    }

    public async Task DeleteScenarioSlotAsync(ScenarioSlot slot)
    {
        await _service.DeleteScenarioSlotAsync(slot.Id);
        await RefreshSlotsAsync();
    }

    public async Task UpdateScenarioSlotAsync(
        ScenarioSlot slot,
        int position,
        Guid classProfileId,
        double weight)
    {
        await _service.UpdateScenarioSlotAsync(
            slot.Id,
            position,
            classProfileId,
            weight);

        await RefreshSlotsAsync();
    }

    public async Task CreateEnemyClassProfileAsync(
        string name,
        EnemyClass enemyClass,
        List<int> allowedColumns,
        Guid generationProfileId,
        double weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        await _service.CreateEnemyClassProfileAsync(
            name.Trim(),
            enemyClass,
            allowedColumns,
            generationProfileId,
            weight);

        await RefreshClassProfilesAsync();
    }

    public async Task DeleteEnemyClassProfileAsync(EnemyClassProfile classProfile)
    {
        await _service.DeleteEnemyClassProfileAsync(classProfile.Id);
        await RefreshClassProfilesAsync();
    }

    public async Task UpdateEnemyClassProfileAsync(
        EnemyClassProfile classProfile,
        string name,
        EnemyClass enemyClass,
        List<int> allowedColumns,
        Guid generationProfileId,
        double weight)
    {
        await _service.UpdateEnemyClassProfileAsync(
            classProfile.Id,
            name.Trim(),
            enemyClass,
            allowedColumns,
            generationProfileId,
            weight);

        await RefreshClassProfilesAsync();
    }

    [RelayCommand]
    public async Task LoadProfilesAsync()
    {
        try
        {
            StatusMessage = "Loading enemy generation profiles...";

            Profiles.Clear();
            ClassProfiles.Clear();
            ClearSelection();
            RefreshDetailsState();

            var profiles = await _service.GetStageProfilesAsync();
            var classProfiles = await _service.GetEnemyClassProfilesAsync();

            if (profiles != null)
            {
                foreach (var profile in profiles)
                    Profiles.Add(profile);
            }

            RefreshStageProfileNameCache();

            if (classProfiles != null)
            {
                foreach (var classProfile in classProfiles)
                {
                    ApplyGenerationProfileDisplay(classProfile);
                    ClassProfiles.Add(classProfile);
                }
            }

            RefreshClassProfileNameCache();

            StatusMessage = $"Loaded {Profiles.Count} stage profiles and {ClassProfiles.Count} class profiles.";
            RefreshDetailsState();
        }
        catch (HttpRequestException)
        {
            StatusMessage = "Failed to load enemy generation profiles.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
        }
        catch (Exception)
        {
            StatusMessage = "Failed to load enemy generation profiles.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }

    [RelayCommand]
    private async Task SelectProfile(StageProfile profile)
    {
        try
        {
            SelectedProfile = profile;
            SelectedScenario = null;

            Scenarios.Clear();
            Slots.Clear();
            RefreshDetailsState();

            await RefreshScenariosAsync();
        }
        catch (HttpRequestException)
        {
            StatusMessage = "Failed to load stage profile details.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
        }
        catch (Exception)
        {
            StatusMessage = "Failed to load stage profile details.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }

    [RelayCommand]
    private async Task SelectScenario(StageScenario scenario)
    {
        try
        {
            SelectedScenario = scenario;

            Slots.Clear();
            RefreshDetailsState();

            var slots = await _service.GetScenarioSlotsAsync(scenario.Id);

            if (slots != null)
            {
                foreach (var slot in slots)
                {
                    ApplyClassProfileDisplay(slot);
                    Slots.Add(slot);
                }
            }

            RefreshDetailsState();
        }
        catch (HttpRequestException)
        {
            StatusMessage = "Failed to load scenario slots.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
        }
        catch (Exception)
        {
            StatusMessage = "Failed to load scenario slots.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }

    [RelayCommand]
    public void ClearSelection()
    {
        Scenarios.Clear();
        Slots.Clear();
        SelectedProfile = null;
        SelectedScenario = null;
        RefreshDetailsState();
    }

    public void ResetSessionState()
    {
        Profiles.Clear();
        Scenarios.Clear();
        Slots.Clear();
        ClassProfiles.Clear();
        _classProfileNames.Clear();
        _stageProfileNames.Clear();
        SelectedProfile = null;
        SelectedScenario = null;
        StatusMessage = "";
        RefreshDetailsState();
    }

    private void RefreshStageProfileNameCache()
    {
        _stageProfileNames.Clear();

        foreach (var profile in Profiles)
            _stageProfileNames[profile.Id] = profile.Name;

        foreach (var classProfile in ClassProfiles)
            ApplyGenerationProfileDisplay(classProfile);
    }

    private void RefreshClassProfileNameCache()
    {
        _classProfileNames.Clear();

        foreach (var classProfile in ClassProfiles)
            _classProfileNames[classProfile.Id] = classProfile.Name;

        foreach (var slot in Slots)
            ApplyClassProfileDisplay(slot);
    }

    private void ApplyGenerationProfileDisplay(EnemyClassProfile classProfile)
    {
        classProfile.GenerationProfileName = _stageProfileNames.TryGetValue(
            classProfile.GenerationProfileId,
            out var name)
            ? name
            : "";
    }

    private void ApplyClassProfileDisplay(ScenarioSlot slot)
    {
        slot.ClassProfileName = _classProfileNames.TryGetValue(
            slot.ClassProfileId,
            out var name)
            ? name
            : "";
    }

    private async Task RefreshScenariosAsync()
    {
        if (SelectedProfile == null)
            return;

        Scenarios.Clear();

        var scenarios = await _service.GetStageScenariosAsync(SelectedProfile.Id);

        if (scenarios != null)
        {
            foreach (var scenario in scenarios)
                Scenarios.Add(scenario);
        }

        RefreshDetailsState();
    }

    public async Task RefreshSlotsAsync()
    {
        if (SelectedScenario == null)
            return;

        Slots.Clear();

        var slots = await _service.GetScenarioSlotsAsync(SelectedScenario.Id);

        if (slots != null)
        {
            foreach (var slot in slots)
            {
                ApplyClassProfileDisplay(slot);
                Slots.Add(slot);
            }
        }

        RefreshDetailsState();
    }

    public async Task RefreshClassProfilesAsync()
    {
        ClassProfiles.Clear();

        var classProfiles = await _service.GetEnemyClassProfilesAsync();

        if (classProfiles != null)
        {
            foreach (var classProfile in classProfiles)
            {
                ApplyGenerationProfileDisplay(classProfile);
                ClassProfiles.Add(classProfile);
            }
        }

        RefreshClassProfileNameCache();
        RefreshDetailsState();
    }

    partial void OnSelectedProfileChanged(StageProfile? value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasSelectedProfile));
    }

    partial void OnSelectedScenarioChanged(StageScenario? value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasSelectedScenario));
    }

    private void RefreshDetailsState()
    {
        OnPropertyChanged(nameof(HasProfiles));
        OnPropertyChanged(nameof(HasScenarios));
        OnPropertyChanged(nameof(HasSlots));
        OnPropertyChanged(nameof(HasSelectedProfile));
        OnPropertyChanged(nameof(HasSelectedScenario));
        OnPropertyChanged(nameof(HasSelectedScenarioWithoutSlots));
        OnPropertyChanged(nameof(HasClassProfiles));
    }
}
