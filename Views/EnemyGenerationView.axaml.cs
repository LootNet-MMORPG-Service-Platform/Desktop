using Avalonia.Controls;
using Avalonia.VisualTree;
using desktop_app.Models.EnemyGeneration;
using desktop_app.Services;
using desktop_app.ViewModels.Generation;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace desktop_app.Views;

public partial class EnemyGenerationView : UserControl
{
    private bool _isActionRunning;

    public EnemyGenerationView()
    {
        InitializeComponent();
    }

    private async void CreateStageProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateStageProfileDialogAsync(owner);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.CreateStageProfileAsync(
                result.Name,
                result.StageIndex,
                result.Weight,
                result.Falloff,
                result.Threshold),
            "Stage profile created.");
    }

    private async void DeleteStageProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: StageProfile profile })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete stage profile '{profile.Name}'?",
            "Delete");

        if (!confirmed)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.DeleteStageProfileAsync(profile),
            "Stage profile deleted.");
    }

    private async void EditStageProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: StageProfile profile })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowEditStageProfileDialogAsync(owner, profile);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.UpdateStageProfileAsync(
                profile,
                result.Name,
                result.StageIndex,
                result.Weight,
                result.Falloff,
                result.Threshold),
            "Stage profile updated.");
    }

    private async void CreateStageScenario_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateStageScenarioDialogAsync(owner);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.CreateStageScenarioAsync(
                result.EnemyCount,
                result.Weight),
            "Scenario created.");
    }

    private async void DeleteStageScenario_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: StageScenario scenario })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete scenario with {scenario.EnemyCount} enemies?",
            "Delete");

        if (!confirmed)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.DeleteStageScenarioAsync(scenario),
            "Scenario deleted.");
    }

    private async void EditStageScenario_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: StageScenario scenario })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowEditStageScenarioDialogAsync(owner, scenario);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.UpdateStageScenarioAsync(
                scenario,
                result.EnemyCount,
                result.Weight),
            "Scenario updated.");
    }

    private async void CreateScenarioSlot_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateScenarioSlotDialogAsync(owner, vm.ClassProfiles);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.CreateScenarioSlotAsync(
                result.Position,
                result.ClassProfileId,
                result.Weight),
            "Slot created.");
    }

    private async void DeleteScenarioSlot_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: ScenarioSlot slot })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete slot at position {slot.Position}?",
            "Delete");

        if (!confirmed)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.DeleteScenarioSlotAsync(slot),
            "Slot deleted.");
    }

    private async void EditScenarioSlot_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: ScenarioSlot slot })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowEditScenarioSlotDialogAsync(owner, vm.ClassProfiles, slot);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.UpdateScenarioSlotAsync(
                slot,
                result.Position,
                result.ClassProfileId,
                result.Weight),
            "Slot updated.");
    }

    private async void CreateEnemyClassProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateEnemyClassProfileDialogAsync(owner, vm.Profiles);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.CreateEnemyClassProfileAsync(
                result.Name,
                result.Class,
                result.AllowedColumns,
                result.GenerationProfileId,
                result.Weight),
            "Class profile created.");
    }

    private async void DeleteEnemyClassProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: EnemyClassProfile classProfile })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete class profile '{classProfile.Name}'?",
            "Delete");

        if (!confirmed)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.DeleteEnemyClassProfileAsync(classProfile),
            "Class profile deleted.");
    }

    private async void EditEnemyClassProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EnemyGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: EnemyClassProfile classProfile })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowEditEnemyClassProfileDialogAsync(owner, vm.Profiles, classProfile);

        if (result == null)
            return;

        await RunEnemyGenerationActionAsync(
            () => vm.UpdateEnemyClassProfileAsync(
                classProfile,
                result.Name,
                result.Class,
                result.AllowedColumns,
                result.GenerationProfileId,
                result.Weight),
            "Class profile updated.");
    }

    private async Task RunEnemyGenerationActionAsync(Func<Task> action, string? successMessage = null)
    {
        if (_isActionRunning)
            return;

        try
        {
            _isActionRunning = true;

            await action();

            if (!string.IsNullOrWhiteSpace(successMessage))
                NotificationService.Instance.ShowSuccess(successMessage);
        }
        catch (HttpRequestException)
        {
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running and verify your internet connection.");
        }
        catch (Exception)
        {
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
        finally
        {
            _isActionRunning = false;
        }
    }
}
