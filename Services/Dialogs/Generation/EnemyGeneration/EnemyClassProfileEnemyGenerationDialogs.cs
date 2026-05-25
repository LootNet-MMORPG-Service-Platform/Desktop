using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Enums;
using desktop_app.Models.EnemyGeneration;

namespace desktop_app.Services.Dialogs.Generation.EnemyGeneration;

public static class EnemyClassProfileEnemyGenerationDialogs
{
    public static async Task<CreateEnemyClassProfileDialogResult?> ShowCreateEnemyClassProfileDialogAsync(
        Window owner,
        IEnumerable<StageProfile> generationProfiles)
    {
        var tcs = new TaskCompletionSource<CreateEnemyClassProfileDialogResult?>();

        var nameBox = EnemyGenerationDialogs.CreateTextBox("Name");
        var classBox = new ComboBox
        {
            Width = 230,
            PlaceholderText = "Enemy class",
            ItemsSource = Enum.GetValues<EnemyClass>(),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        var allowedColumnsBox = EnemyGenerationDialogs.CreateTextBox("0,1,2");
        var generationProfileBox = new ComboBox
        {
            Width = 230,
            PlaceholderText = "Generation profile",
            ItemsSource = generationProfiles,
            HorizontalAlignment = HorizontalAlignment.Center,
            ItemTemplate = new FuncDataTemplate<StageProfile>((profile, _) =>
                new TextBlock
                {
                    Text = profile?.Name ?? "",
                    Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black)
                })
        };
        var weightBox = EnemyGenerationDialogs.CreateTextBox("Weight");
        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 25
        };

        var createButton = GenerationDialogUiFactory.CreateDialogButton("Create", "detailsBtn");
        var cancelButton = GenerationDialogUiFactory.CreateDialogButton("Cancel", "dialogCancelBtn");

        var content = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new StackPanel
                {
                    Spacing = 10,
                    Width = 260,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Create class profile",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        EnemyGenerationDialogs.CreateLabel("Name"),
                        nameBox,
                        EnemyGenerationDialogs.CreateLabel("Enemy class"),
                        classBox,
                        EnemyGenerationDialogs.CreateLabel("Allowed columns"),
                        allowedColumnsBox,
                        EnemyGenerationDialogs.CreateLabel("Generation profile"),
                        generationProfileBox,
                        EnemyGenerationDialogs.CreateLabel("Weight"),
                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 400, 575);
        dialog.Title = "Create class profile";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!EnemyGenerationDialogs.TryCreateClassProfileResult(
                    nameBox.Text,
                    classBox.SelectedItem,
                    allowedColumnsBox.Text,
                    generationProfileBox.SelectedItem as StageProfile,
                    weightBox.Text,
                    out var result,
                    out var error))
            {
                errorText.Text = error;
                return;
            }

            tcs.TrySetResult(result);
            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }
}
