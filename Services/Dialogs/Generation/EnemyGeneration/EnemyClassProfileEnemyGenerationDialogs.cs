using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        return await ShowEnemyClassProfileDialogAsync(owner, generationProfiles, "Create class profile", "Create");
    }

    public static async Task<CreateEnemyClassProfileDialogResult?> ShowEditEnemyClassProfileDialogAsync(
        Window owner,
        IEnumerable<StageProfile> generationProfiles,
        EnemyClassProfile classProfile)
    {
        return await ShowEnemyClassProfileDialogAsync(
            owner,
            generationProfiles,
            "Edit class profile",
            "Save",
            classProfile);
    }

    private static async Task<CreateEnemyClassProfileDialogResult?> ShowEnemyClassProfileDialogAsync(
        Window owner,
        IEnumerable<StageProfile> generationProfiles,
        string title,
        string buttonText,
        EnemyClassProfile? classProfile = null)
    {
        var tcs = new TaskCompletionSource<CreateEnemyClassProfileDialogResult?>();
        var generationProfilesList = generationProfiles.ToList();

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
            ItemsSource = generationProfilesList,
            HorizontalAlignment = HorizontalAlignment.Center,
            ItemTemplate = new FuncDataTemplate<StageProfile>((profile, _) =>
                new TextBlock
                {
                    Text = profile?.Name ?? "",
                    Foreground = Brushes.Black
                })
        };
        var weightBox = EnemyGenerationDialogs.CreateTextBox("Weight");
        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 34
        };

        if (classProfile != null)
        {
            nameBox.Text = classProfile.Name;
            classBox.SelectedItem = classProfile.Class;
            allowedColumnsBox.Text = string.Join(",", classProfile.AllowedColumns);
            generationProfileBox.SelectedItem = generationProfilesList.FirstOrDefault(
                x => x.Id == classProfile.GenerationProfileId);
            weightBox.Text = classProfile.Weight.ToString(CultureInfo.CurrentCulture);
        }

        var createButton = GenerationDialogUiFactory.CreateDialogButton(buttonText, "detailsBtn");
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
                            Text = title,
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
        dialog.Title = title;

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
