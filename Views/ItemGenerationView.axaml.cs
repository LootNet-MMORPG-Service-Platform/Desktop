using Avalonia.Controls;
using Avalonia.VisualTree;
using desktop_app.Services;
using desktop_app.ViewModels.Generation;
using desktop_app.Models.Generation;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace desktop_app.Views;

public partial class ItemGenerationView : UserControl
{
    public ItemGenerationView()
    {
        InitializeComponent();
    }

    private async void DeleteProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete profile '{vm.SelectedProfile?.Name}'?",
            "Delete");

        if (!confirmed)
            return;

        await RunGenerationActionAsync(
            () => vm.DeleteSelectedProfileAsync(),
            "Profile deleted.");
    }
    
    private async void CreateProfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var name = await DialogService.ShowCreateProfileDialogAsync(owner);

        if (string.IsNullOrWhiteSpace(name))
            return;

        await RunGenerationActionAsync(
            () => vm.CreateProfileAsync(name),
            "Profile created.");
    }
    
    private async void AddRule_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateRuleDialogAsync(owner);

        if (result == null)
            return;

        await RunGenerationActionAsync(
            () => vm.CreateRuleAsync(
                result.Category,
                result.WeaponType,
                result.ArmorType,
                result.IsFallback),
            "Rule created.");
    }

    private async void AddTypeWeight_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateTypeWeightDialogAsync(owner);

        if (result == null)
            return;

        await RunGenerationActionAsync(
            () => vm.CreateTypeWeightAsync(
                result.Category,
                result.WeaponType,
                result.ArmorType,
                result.Weight),
            "Type weight created.");
    }

    private async void DeleteTypeWeight_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: TypeWeight weight })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete type weight '{weight.Category} {weight.WeaponType}{weight.ArmorType}'?",
            "Delete");

        if (!confirmed)
            return;

        await RunGenerationActionAsync(
            () => vm.DeleteTypeWeightAsync(weight),
            "Type weight deleted.");
    }

    private async void EditTypeWeight_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: TypeWeight weight })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowEditTypeWeightDialogAsync(owner, weight);

        if (result == null)
            return;

        await RunGenerationActionAsync(
            () => vm.UpdateTypeWeightAsync(
                weight,
                result.Category,
                result.WeaponType,
                result.ArmorType,
                result.Weight),
            "Type weight updated.");
    }
    
    private async void DeleteRule_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: GenerationRule rule })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete rule '{rule.Category} {rule.WeaponType}{rule.ArmorType}'?",
            "Delete");

        if (!confirmed)
            return;

        await RunGenerationActionAsync(
            () => vm.DeleteRuleAsync(rule),
            "Rule deleted.");
    }

    private async void EditRule_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: GenerationRule rule })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowEditRuleDialogAsync(owner, rule);

        if (result == null)
            return;

        await RunGenerationActionAsync(
            () => vm.UpdateRuleAsync(
                rule,
                result.Category,
                result.WeaponType,
                result.ArmorType,
                result.IsFallback),
            "Rule updated.");
    }
    
    private async void AddParameter_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: GenerationRule rule })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateParameterDialogAsync(owner);

        if (result == null)
            return;

        await RunGenerationActionAsync(
            () => vm.CreateParameterAsync(
                rule,
                result.Parameter,
                result.Segments),
            "Parameter created.");
    }

    private async void DeleteParameter_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: GenerationParameter parameter })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete parameter '{parameter.Parameter}'?",
            "Delete");

        if (!confirmed)
            return;

        await RunGenerationActionAsync(
            () => vm.DeleteParameterAsync(parameter),
            "Parameter deleted.");
    }
    
    private async void AddElement_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: GenerationRule rule })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var result = await DialogService.ShowCreateElementDialogAsync(owner);

        if (result == null)
            return;

        await RunGenerationActionAsync(
            () => vm.CreateElementAsync(
                rule,
                result.ElementType,
                result.Segments),
            "Element created.");
    }

    private async void DeleteElement_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ItemGenerationViewModel vm)
            return;

        if (sender is not Button { DataContext: GenerationElement element })
            return;

        var owner = this.GetVisualRoot() as Window;

        if (owner == null)
            return;

        var confirmed = await DialogService.ShowConfirmDialogAsync(
            owner,
            $"Are you sure you want to delete element '{element.ElementType}'?",
            "Delete");

        if (!confirmed)
            return;

        await RunGenerationActionAsync(
            () => vm.DeleteElementAsync(element),
            "Element deleted.");
    }

    private static async Task RunGenerationActionAsync(Func<Task> action, string? successMessage = null)
    {
        try
        {
            await action();

            if (!string.IsNullOrWhiteSpace(successMessage))
                NotificationService.Instance.ShowSuccess(successMessage);
        }
        catch (HttpRequestException)
        {
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
        }
        catch (Exception)
        {
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
    }
}
