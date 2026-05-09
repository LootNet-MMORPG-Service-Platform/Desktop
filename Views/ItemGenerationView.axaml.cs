using Avalonia.Controls;
using Avalonia.VisualTree;
using desktop_app.Services;
using desktop_app.ViewModels.Generation;
using desktop_app.Models.Generation;

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

        await vm.DeleteSelectedProfileAsync();
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

        await vm.CreateProfileAsync(name);
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

        await vm.CreateRuleAsync(
            result.Category,
            result.WeaponType,
            result.ArmorType,
            result.IsFallback);
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

        await vm.CreateTypeWeightAsync(
            result.Category,
            result.WeaponType,
            result.ArmorType,
            result.Weight);
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

        await vm.DeleteTypeWeightAsync(weight);
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

        await vm.UpdateTypeWeightAsync(
            weight,
            result.Category,
            result.WeaponType,
            result.ArmorType,
            result.Weight);
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

        await vm.DeleteRuleAsync(rule);
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

        await vm.CreateParameterAsync(
            rule,
            result.Parameter,
            result.Segments);
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

        await vm.DeleteParameterAsync(parameter);
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

        await vm.CreateElementAsync(
            rule,
            result.ElementType,
            result.Segments);
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

        await vm.DeleteElementAsync(element);
    }
}
