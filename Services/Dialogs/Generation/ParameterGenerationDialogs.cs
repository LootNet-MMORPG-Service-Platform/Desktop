using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        return await ShowParameterDialogAsync(owner, "Create parameter", "Create");
    }

    public static async Task<CreateParameterDialogResult?> ShowEditParameterDialogAsync(
        Window owner,
        GenerationParameter parameter)
    {
        return await ShowParameterDialogAsync(owner, "Edit parameter", "Save", parameter);
    }

    private static async Task<CreateParameterDialogResult?> ShowParameterDialogAsync(
        Window owner,
        string title,
        string buttonText,
        GenerationParameter? parameter = null)
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

        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 34
        };

        if (parameter != null)
        {
            parameterBox.SelectedItem = parameter.Parameter;
            ApplySegmentValues(parameter.Segments.FirstOrDefault(), minBox, maxBox, weightBox);
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
                    Spacing = 12,
                    Width = 240,
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

                        parameterBox,
                        minBox,
                        maxBox,
                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 360, 400);
        dialog.Title = title;

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (parameterBox.SelectedItem == null)
            {
                errorText.Text = "Parameter is required.";
                return;
            }

            if (!TryCreateSegment(
                    minBox.Text,
                    maxBox.Text,
                    weightBox.Text,
                    out var segment,
                    out var error))
            {
                errorText.Text = error;
                return;
            }

            tcs.TrySetResult(new CreateParameterDialogResult
            {
                Parameter = (ItemParameter)parameterBox.SelectedItem!,
                Segments = new List<CreateSegmentInput>
                {
                    segment
                }
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

    public static async Task<CreateElementDialogResult?> ShowCreateElementDialogAsync(Window owner)
    {
        return await ShowElementDialogAsync(owner, "Create element", "Create");
    }

    public static async Task<CreateElementDialogResult?> ShowEditElementDialogAsync(
        Window owner,
        GenerationElement element)
    {
        return await ShowElementDialogAsync(owner, "Edit element", "Save", element);
    }

    private static async Task<CreateElementDialogResult?> ShowElementDialogAsync(
        Window owner,
        string title,
        string buttonText,
        GenerationElement? element = null)
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

        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 34
        };

        if (element != null)
        {
            elementBox.SelectedItem = element.ElementType;
            ApplySegmentValues(element.Segments.FirstOrDefault(), minBox, maxBox, weightBox);
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
                    Spacing = 12,
                    Width = 240,
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

                        elementBox,
                        minBox,
                        maxBox,
                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 360, 400);
        dialog.Title = title;

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (elementBox.SelectedItem == null)
            {
                errorText.Text = "Element type is required.";
                return;
            }

            if (!TryCreateSegment(
                    minBox.Text,
                    maxBox.Text,
                    weightBox.Text,
                    out var segment,
                    out var error))
            {
                errorText.Text = error;
                return;
            }

            tcs.TrySetResult(new CreateElementDialogResult
            {
                ElementType = (ItemElementType)elementBox.SelectedItem!,
                Segments = new List<CreateSegmentInput>
                {
                    segment
                }
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

    private static void ApplySegmentValues(
        Segment? segment,
        TextBox minBox,
        TextBox maxBox,
        TextBox weightBox)
    {
        if (segment == null)
            return;

        minBox.Text = segment.Min.ToString(CultureInfo.CurrentCulture);
        maxBox.Text = segment.Max.ToString(CultureInfo.CurrentCulture);
        weightBox.Text = segment.Weight.ToString(CultureInfo.CurrentCulture);
    }

    private static bool TryCreateSegment(
        string? minValue,
        string? maxValue,
        string? weightValue,
        out CreateSegmentInput segment,
        out string error)
    {
        segment = new CreateSegmentInput();
        error = "";

        if (string.IsNullOrWhiteSpace(minValue))
        {
            error = "Min is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(maxValue))
        {
            error = "Max is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(weightValue))
        {
            error = "Weight is required.";
            return false;
        }

        if (!TryParseDouble(minValue, out var min) ||
            !TryParseDouble(maxValue, out var max) ||
            !int.TryParse(weightValue, NumberStyles.Integer, CultureInfo.CurrentCulture, out var weight))
        {
            error = "Enter valid numeric values.";
            return false;
        }

        if (min is < -1000000 or > 1000000 ||
            max is < -1000000 or > 1000000)
        {
            error = "Min and Max must be between -1000000 and 1000000.";
            return false;
        }

        if (weight is < 1 or > 1000000)
        {
            error = "Weight must be between 1 and 1000000.";
            return false;
        }

        if (min > max)
        {
            error = "Min cannot be greater than Max.";
            return false;
        }

        segment = new CreateSegmentInput
        {
            Min = min,
            Max = max,
            Weight = weight
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
