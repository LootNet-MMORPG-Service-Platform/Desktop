using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models.EnemyGeneration;

namespace desktop_app.Services.Dialogs.Generation;

public static class EnemyGenerationDialogs
{
    public static async Task<CreateStageProfileDialogResult?> ShowCreateStageProfileDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateStageProfileDialogResult?>();

        var nameBox = CreateTextBox("Name");
        var stageIndexBox = CreateTextBox("Stage index");
        var weightBox = CreateTextBox("Weight");
        var falloffBox = CreateTextBox("Falloff");
        var thresholdBox = CreateTextBox("Threshold");
        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            Height = 20
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
                            Text = "Create stage profile",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        CreateLabel("Name"),
                        nameBox,
                        CreateLabel("Stage index"),
                        stageIndexBox,
                        CreateLabel("Weight"),
                        weightBox,
                        CreateLabel("Falloff"),
                        falloffBox,
                        CreateLabel("Threshold"),
                        thresholdBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 400, 600);
        dialog.Title = "Create stage profile";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!TryCreateResult(
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

    private static TextBox CreateTextBox(string watermark)
    {
        return new TextBox
        {
            Width = 230,
            Watermark = watermark,
            HorizontalAlignment = HorizontalAlignment.Center
        };
    }

    private static TextBlock CreateLabel(string text)
    {
        return new TextBlock
        {
            Text = text,
            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray)
        };
    }

    private static bool TryCreateResult(
        string? name,
        string? stageIndexValue,
        string? weightValue,
        string? falloffValue,
        string? thresholdValue,
        out CreateStageProfileDialogResult? result,
        out string error)
    {
        result = null;
        error = "";

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Name is required.";
            return false;
        }

        if (!int.TryParse(stageIndexValue, NumberStyles.Integer, CultureInfo.CurrentCulture, out var stageIndex) ||
            !TryParseDouble(weightValue, out var weight) ||
            !TryParseDouble(falloffValue, out var falloff) ||
            !int.TryParse(thresholdValue, NumberStyles.Integer, CultureInfo.CurrentCulture, out var threshold))
        {
            error = "Enter valid numeric values.";
            return false;
        }

        result = new CreateStageProfileDialogResult
        {
            Name = name.Trim(),
            StageIndex = stageIndex,
            Weight = weight,
            Falloff = falloff,
            Threshold = threshold
        };

        return true;
    }

    private static bool TryParseDouble(string? value, out double result)
    {
        return double.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result)
            || double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result)
            || double.TryParse(
                value?.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out result);
    }
}
