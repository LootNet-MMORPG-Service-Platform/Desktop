using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Enums;
using desktop_app.Models.Generation;

namespace desktop_app.Services.Dialogs.Generation;

public static class ParameterGenerationDialogs
{
    public static async Task<CreateParameterDialogResult?> ShowCreateParameterDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateParameterDialogResult?>();

        var parameters = Enum.GetValues<ItemParameter>();

        var parameterBox = new ComboBox
        {
            ItemsSource = parameters,
            SelectedItem = ItemParameter.CutDamage,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var minBox = new TextBox
        {
            Watermark = "Min",
            Width = 220
        };

        var maxBox = new TextBox
        {
            Watermark = "Max",
            Width = 220
        };

        var weightBox = new TextBox
        {
            Watermark = "Weight",
            Width = 220
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
                    Spacing = 12,
                    Width = 240,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Create parameter",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        parameterBox,
                        minBox,
                        maxBox,
                        weightBox,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 360, 340);
        dialog.Title = "Create parameter";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!double.TryParse(minBox.Text, out var min))
                return;

            if (!double.TryParse(maxBox.Text, out var max))
                return;

            if (!int.TryParse(weightBox.Text, out var weight))
                return;

            tcs.TrySetResult(new CreateParameterDialogResult
            {
                Parameter = (ItemParameter)parameterBox.SelectedItem!,
                Segments = new List<CreateSegmentInput>
                {
                    new()
                    {
                        Min = min,
                        Max = max,
                        Weight = weight
                    }
                }
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

    public static async Task<CreateElementDialogResult?> ShowCreateElementDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateElementDialogResult?>();

        var elements = Enum.GetValues<ItemElementType>();

        var elementBox = new ComboBox
        {
            ItemsSource = elements,
            SelectedItem = ItemElementType.Fire,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var minBox = new TextBox
        {
            Watermark = "Min",
            Width = 220
        };

        var maxBox = new TextBox
        {
            Watermark = "Max",
            Width = 220
        };

        var weightBox = new TextBox
        {
            Watermark = "Weight",
            Width = 220
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
                    Spacing = 12,
                    Width = 240,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Create element",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        elementBox,
                        minBox,
                        maxBox,
                        weightBox,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 360, 340);
        dialog.Title = "Create element";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!double.TryParse(minBox.Text, out var min))
                return;

            if (!double.TryParse(maxBox.Text, out var max))
                return;

            if (!int.TryParse(weightBox.Text, out var weight))
                return;

            tcs.TrySetResult(new CreateElementDialogResult
            {
                ElementType = (ItemElementType)elementBox.SelectedItem!,
                Segments = new List<CreateSegmentInput>
                {
                    new()
                    {
                        Min = min,
                        Max = max,
                        Weight = weight
                    }
                }
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }
}
