using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models.EnemyGeneration;
using System.Globalization;

namespace desktop_app.Services.Dialogs.Generation.EnemyGeneration;

public static class StageScenarioEnemyGenerationDialogs
{
    public static async Task<CreateStageScenarioDialogResult?> ShowCreateStageScenarioDialogAsync(Window owner)
    {
        return await ShowStageScenarioDialogAsync(owner, "Create scenario", "Create");
    }

    public static async Task<CreateStageScenarioDialogResult?> ShowEditStageScenarioDialogAsync(
        Window owner,
        StageScenario scenario)
    {
        return await ShowStageScenarioDialogAsync(owner, "Edit scenario", "Save", scenario);
    }

    private static async Task<CreateStageScenarioDialogResult?> ShowStageScenarioDialogAsync(
        Window owner,
        string title,
        string buttonText,
        StageScenario? scenario = null)
    {
        var tcs = new TaskCompletionSource<CreateStageScenarioDialogResult?>();

        var enemyCountBox = EnemyGenerationDialogs.CreateTextBox("Enemy count");
        var weightBox = EnemyGenerationDialogs.CreateTextBox("Weight");
        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 34
        };

        if (scenario != null)
        {
            enemyCountBox.Text = scenario.EnemyCount.ToString(CultureInfo.CurrentCulture);
            weightBox.Text = scenario.Weight.ToString(CultureInfo.CurrentCulture);
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

                        EnemyGenerationDialogs.CreateLabel("Enemy count"),
                        enemyCountBox,
                        EnemyGenerationDialogs.CreateLabel("Weight"),
                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 400, 360);
        dialog.Title = title;

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!EnemyGenerationDialogs.TryCreateScenarioResult(
                    enemyCountBox.Text,
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
