using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models.EnemyGeneration;
using System.Globalization;

namespace desktop_app.Services.Dialogs.Generation.EnemyGeneration;

public static class StageProfileEnemyGenerationDialogs
{
    public static async Task<CreateStageProfileDialogResult?> ShowCreateStageProfileDialogAsync(Window owner)
    {
        return await ShowStageProfileDialogAsync(owner, "Create stage profile", "Create");
    }

    public static async Task<CreateStageProfileDialogResult?> ShowEditStageProfileDialogAsync(
        Window owner,
        StageProfile profile)
    {
        return await ShowStageProfileDialogAsync(owner, "Edit stage profile", "Save", profile);
    }

    private static async Task<CreateStageProfileDialogResult?> ShowStageProfileDialogAsync(
        Window owner,
        string title,
        string buttonText,
        StageProfile? profile = null)
    {
        var tcs = new TaskCompletionSource<CreateStageProfileDialogResult?>();

        var nameBox = EnemyGenerationDialogs.CreateTextBox("Name");
        var stageIndexBox = EnemyGenerationDialogs.CreateTextBox("Stage index");
        var weightBox = EnemyGenerationDialogs.CreateTextBox("Weight");
        var falloffBox = EnemyGenerationDialogs.CreateTextBox("Falloff");
        var thresholdBox = EnemyGenerationDialogs.CreateTextBox("Threshold");
        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 34
        };

        if (profile != null)
        {
            nameBox.Text = profile.Name;
            stageIndexBox.Text = profile.StageIndex.ToString(CultureInfo.CurrentCulture);
            weightBox.Text = profile.Weight.ToString(CultureInfo.CurrentCulture);
            falloffBox.Text = profile.Falloff.ToString(CultureInfo.CurrentCulture);
            thresholdBox.Text = profile.Threshold.ToString(CultureInfo.CurrentCulture);
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
                        EnemyGenerationDialogs.CreateLabel("Stage index"),
                        stageIndexBox,
                        EnemyGenerationDialogs.CreateLabel("Weight"),
                        weightBox,
                        EnemyGenerationDialogs.CreateLabel("Falloff"),
                        falloffBox,
                        EnemyGenerationDialogs.CreateLabel("Threshold"),
                        thresholdBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 400, 600);
        dialog.Title = title;

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!EnemyGenerationDialogs.TryCreateStageProfileResult(
                    nameBox.Text,
                    stageIndexBox.Text,
                    weightBox.Text,
                    falloffBox.Text,
                    thresholdBox.Text,
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
