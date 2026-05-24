using System.Threading.Tasks;
using Avalonia.Controls;
using desktop_app.Models;
using desktop_app.Models.EnemyGeneration;
using desktop_app.Models.Generation;

namespace desktop_app.Services;

public static class DialogService
{
    public static Task<bool> ShowConfirmDialogAsync(
        Window owner,
        string message,
        string confirmText = "Confirm")
    {
        return CommonDialogService.ShowConfirmDialogAsync(owner, message, confirmText);
    }

    public static Task ShowErrorDialogAsync(Window owner, string message)
    {
        return CommonDialogService.ShowErrorDialogAsync(owner, message);
    }

    public static Task ShowReadonlyTextDialogAsync(Window owner, string title, string text)
    {
        return CommonDialogService.ShowReadonlyTextDialogAsync(owner, title, text);
    }

    public static Task<ChangePasswordDialogResult?> ShowChangePasswordDialogAsync(Window owner)
    {
        return CommonDialogService.ShowChangePasswordDialogAsync(owner);
    }

    public static Task<string?> ShowChangeRoleDialogAsync(Window owner, string username, string currentRole)
    {
        return UserDialogService.ShowChangeRoleDialogAsync(owner, username, currentRole);
    }

    public static Task ShowInventoryDialogAsync(Window owner, ItemCollectionDTO items)
    {
        return UserDialogService.ShowInventoryDialogAsync(owner, items);
    }

    public static Task ShowEquipmentDialogAsync(Window owner, EquipmentResponseDTO eq)
    {
        return UserDialogService.ShowEquipmentDialogAsync(owner, eq);
    }
    
    public static Task<string?> ShowCreateProfileDialogAsync(Window owner)
    {
        return GenerationDialogService.ShowCreateProfileDialogAsync(owner);
    }
    
    public static Task<CreateRuleDialogResult?> ShowCreateRuleDialogAsync(Window owner)
    {
        return GenerationDialogService.ShowCreateRuleDialogAsync(owner);
    }

    public static Task<CreateRuleDialogResult?> ShowEditRuleDialogAsync(Window owner, GenerationRule rule)
    {
        return GenerationDialogService.ShowEditRuleDialogAsync(owner, rule);
    }

    public static Task<CreateTypeWeightDialogResult?> ShowCreateTypeWeightDialogAsync(Window owner)
    {
        return GenerationDialogService.ShowCreateTypeWeightDialogAsync(owner);
    }

    public static Task<CreateTypeWeightDialogResult?> ShowEditTypeWeightDialogAsync(Window owner, TypeWeight weight)
    {
        return GenerationDialogService.ShowEditTypeWeightDialogAsync(owner, weight);
    }
    
    public static Task<CreateParameterDialogResult?> ShowCreateParameterDialogAsync(Window owner)
    {
        return GenerationDialogService.ShowCreateParameterDialogAsync(owner);
    }
    
    public static Task<CreateElementDialogResult?> ShowCreateElementDialogAsync(Window owner)
    {
        return GenerationDialogService.ShowCreateElementDialogAsync(owner);
    }

    public static Task<CreateStageProfileDialogResult?> ShowCreateStageProfileDialogAsync(Window owner)
    {
        return GenerationDialogService.ShowCreateStageProfileDialogAsync(owner);
    }

    public static Task<CreateStageScenarioDialogResult?> ShowCreateStageScenarioDialogAsync(Window owner)
    {
        return GenerationDialogService.ShowCreateStageScenarioDialogAsync(owner);
    }

    public static Task<CreateScenarioSlotDialogResult?> ShowCreateScenarioSlotDialogAsync(
        Window owner,
        System.Collections.Generic.IEnumerable<EnemyClassProfile> classProfiles)
    {
        return GenerationDialogService.ShowCreateScenarioSlotDialogAsync(owner, classProfiles);
    }
}
