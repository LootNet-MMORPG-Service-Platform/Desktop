using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models.EnemyGeneration;

namespace desktop_app.Services.Dialogs.Generation.EnemyGeneration;

public static class ScenarioSlotEnemyGenerationDialogs
{
    public static async Task<CreateScenarioSlotDialogResult?> ShowCreateScenarioSlotDialogAsync(
        Window owner,
        IEnumerable<EnemyClassProfile> classProfiles)
    {
        var tcs = new TaskCompletionSource<CreateScenarioSlotDialogResult?>();

        var positionBox = EnemyGenerationDialogs.CreateTextBox("Position");
        var classProfileBox = new ComboBox
        {
            Width = 230,
            PlaceholderText = "Class profile",
            ItemsSource = classProfiles,
            HorizontalAlignment = HorizontalAlignment.Center,
            ItemTemplate = new FuncDataTemplate<EnemyClassProfile>((profile, _) =>
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
                            Text = "Create slot",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        EnemyGenerationDialogs.CreateLabel("Position"),
                        positionBox,
                        EnemyGenerationDialogs.CreateLabel("Class profile"),
                        classProfileBox,
                        EnemyGenerationDialogs.CreateLabel("Weight"),
                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 400, 400);
        dialog.Title = "Create slot";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!EnemyGenerationDialogs.TryCreateSlotResult(
                    positionBox.Text,
                    classProfileBox.SelectedItem as EnemyClassProfile,
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
