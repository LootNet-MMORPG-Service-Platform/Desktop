using System.Threading.Tasks;
using Avalonia.Controls;
using desktop_app.Models.EnemyGeneration;
using desktop_app.Models.Generation;
using desktop_app.Services.Dialogs.Generation;

namespace desktop_app.Services;

public static class GenerationDialogService
{
    public static async Task<string?> ShowCreateProfileDialogAsync(Window owner)
    {
        return await ProfileGenerationDialogs.ShowCreateProfileDialogAsync(owner);
    }

    public static async Task<CreateRuleDialogResult?> ShowCreateRuleDialogAsync(Window owner)
    {
        return await RuleGenerationDialogs.ShowCreateRuleDialogAsync(owner);
    }

    public static async Task<CreateRuleDialogResult?> ShowEditRuleDialogAsync(Window owner, GenerationRule rule)
    {
        return await RuleGenerationDialogs.ShowEditRuleDialogAsync(owner, rule);
    }

    public static async Task<CreateTypeWeightDialogResult?> ShowCreateTypeWeightDialogAsync(Window owner)
    {
        return await TypeWeightGenerationDialogs.ShowCreateTypeWeightDialogAsync(owner);
    }

    public static async Task<CreateTypeWeightDialogResult?> ShowEditTypeWeightDialogAsync(Window owner, TypeWeight weight)
    {
        return await TypeWeightGenerationDialogs.ShowEditTypeWeightDialogAsync(owner, weight);
    }

    public static async Task<CreateParameterDialogResult?> ShowCreateParameterDialogAsync(Window owner)
    {
        return await ParameterGenerationDialogs.ShowCreateParameterDialogAsync(owner);
    }

    public static async Task<CreateElementDialogResult?> ShowCreateElementDialogAsync(Window owner)
    {
        return await ParameterGenerationDialogs.ShowCreateElementDialogAsync(owner);
    }

    public static async Task<CreateStageProfileDialogResult?> ShowCreateStageProfileDialogAsync(Window owner)
    {
        return await EnemyGenerationDialogs.ShowCreateStageProfileDialogAsync(owner);
    }

    public static async Task<CreateStageProfileDialogResult?> ShowEditStageProfileDialogAsync(
        Window owner,
        StageProfile profile)
    {
        return await EnemyGenerationDialogs.ShowEditStageProfileDialogAsync(owner, profile);
    }

    public static async Task<CreateStageScenarioDialogResult?> ShowCreateStageScenarioDialogAsync(Window owner)
    {
        return await EnemyGenerationDialogs.ShowCreateStageScenarioDialogAsync(owner);
    }

    public static async Task<CreateStageScenarioDialogResult?> ShowEditStageScenarioDialogAsync(
        Window owner,
        StageScenario scenario)
    {
        return await EnemyGenerationDialogs.ShowEditStageScenarioDialogAsync(owner, scenario);
    }

    public static async Task<CreateScenarioSlotDialogResult?> ShowCreateScenarioSlotDialogAsync(
        Window owner,
        System.Collections.Generic.IEnumerable<EnemyClassProfile> classProfiles)
    {
        return await EnemyGenerationDialogs.ShowCreateScenarioSlotDialogAsync(owner, classProfiles);
    }

    public static async Task<CreateScenarioSlotDialogResult?> ShowEditScenarioSlotDialogAsync(
        Window owner,
        System.Collections.Generic.IEnumerable<EnemyClassProfile> classProfiles,
        ScenarioSlot slot)
    {
        return await EnemyGenerationDialogs.ShowEditScenarioSlotDialogAsync(owner, classProfiles, slot);
    }

    public static async Task<CreateEnemyClassProfileDialogResult?> ShowCreateEnemyClassProfileDialogAsync(
        Window owner,
        System.Collections.Generic.IEnumerable<StageProfile> generationProfiles)
    {
        return await EnemyGenerationDialogs.ShowCreateEnemyClassProfileDialogAsync(owner, generationProfiles);
    }

    public static async Task<CreateEnemyClassProfileDialogResult?> ShowEditEnemyClassProfileDialogAsync(
        Window owner,
        System.Collections.Generic.IEnumerable<StageProfile> generationProfiles,
        EnemyClassProfile classProfile)
    {
        return await EnemyGenerationDialogs.ShowEditEnemyClassProfileDialogAsync(
            owner,
            generationProfiles,
            classProfile);
    }
}
