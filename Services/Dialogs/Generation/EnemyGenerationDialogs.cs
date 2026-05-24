using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Enums;
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

    public static async Task<CreateStageScenarioDialogResult?> ShowCreateStageScenarioDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateStageScenarioDialogResult?>();

        var enemyCountBox = CreateTextBox("Enemy count");
        var weightBox = CreateTextBox("Weight");
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
                            Text = "Create scenario",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        CreateLabel("Enemy count"),
                        enemyCountBox,
                        CreateLabel("Weight"),
                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 400, 315);
        dialog.Title = "Create scenario";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!TryCreateScenarioResult(
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

    public static async Task<CreateScenarioSlotDialogResult?> ShowCreateScenarioSlotDialogAsync(
        Window owner,
        IEnumerable<EnemyClassProfile> classProfiles)
    {
        var tcs = new TaskCompletionSource<CreateScenarioSlotDialogResult?>();

        var positionBox = CreateTextBox("Position");
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
                    Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black)
                })
        };
        var weightBox = CreateTextBox("Weight");
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
                            Text = "Create slot",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        CreateLabel("Position"),
                        positionBox,
                        CreateLabel("Class profile"),
                        classProfileBox,
                        CreateLabel("Weight"),
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
            if (!TryCreateSlotResult(
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

    public static async Task<CreateEnemyClassProfileDialogResult?> ShowCreateEnemyClassProfileDialogAsync(
        Window owner,
        IEnumerable<StageProfile> generationProfiles)
    {
        var tcs = new TaskCompletionSource<CreateEnemyClassProfileDialogResult?>();

        var nameBox = CreateTextBox("Name");
        var classBox = new ComboBox
        {
            Width = 230,
            PlaceholderText = "Enemy class",
            ItemsSource = Enum.GetValues<EnemyClass>(),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        var allowedColumnsBox = CreateTextBox("0,1,2");
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
        var weightBox = CreateTextBox("Weight");
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

                        CreateLabel("Name"),
                        nameBox,
                        CreateLabel("Enemy class"),
                        classBox,
                        CreateLabel("Allowed columns"),
                        allowedColumnsBox,
                        CreateLabel("Generation profile"),
                        generationProfileBox,
                        CreateLabel("Weight"),
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
            if (!TryCreateClassProfileResult(
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

    private static bool TryCreateScenarioResult(
        string? enemyCountValue,
        string? weightValue,
        out CreateStageScenarioDialogResult? result,
        out string error)
    {
        result = null;
        error = "";

        if (!int.TryParse(enemyCountValue, NumberStyles.Integer, CultureInfo.CurrentCulture, out var enemyCount))
        {
            error = "Enter valid numeric values.";
            return false;
        }

        if (enemyCount is < 1 or > 20)
        {
            error = "Enemy count must be between 1 and 20.";
            return false;
        }

        if (!TryParseDouble(weightValue, out var weight))
        {
            error = "Enter valid numeric values.";
            return false;
        }

        if (weight is < 0.0001 or > 1000000)
        {
            error = "Weight must be between 0.0001 and 1000000.";
            return false;
        }

        result = new CreateStageScenarioDialogResult
        {
            EnemyCount = enemyCount,
            Weight = weight
        };

        return true;
    }

    private static bool TryCreateSlotResult(
        string? positionValue,
        EnemyClassProfile? classProfile,
        string? weightValue,
        out CreateScenarioSlotDialogResult? result,
        out string error)
    {
        result = null;
        error = "";

        if (!int.TryParse(positionValue, NumberStyles.Integer, CultureInfo.CurrentCulture, out var position))
        {
            error = "Enter valid numeric values.";
            return false;
        }

        if (position is < 0 or > 4)
        {
            error = "Position must be between 0 and 4.";
            return false;
        }

        if (classProfile == null || classProfile.Id == Guid.Empty)
        {
            error = "Class profile is required.";
            return false;
        }

        if (!TryParseDouble(weightValue, out var weight))
        {
            error = "Enter valid numeric values.";
            return false;
        }

        if (weight is < 0.0001 or > 1000000)
        {
            error = "Weight must be between 0.0001 and 1000000.";
            return false;
        }

        result = new CreateScenarioSlotDialogResult
        {
            Position = position,
            ClassProfileId = classProfile.Id,
            Weight = weight
        };

        return true;
    }

    private static bool TryCreateClassProfileResult(
        string? name,
        object? enemyClassValue,
        string? allowedColumnsValue,
        StageProfile? generationProfile,
        string? weightValue,
        out CreateEnemyClassProfileDialogResult? result,
        out string error)
    {
        result = null;
        error = "";

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Name is required.";
            return false;
        }

        if (enemyClassValue is not EnemyClass enemyClass)
        {
            error = "Enemy class is required.";
            return false;
        }

        if (generationProfile == null || generationProfile.Id == Guid.Empty)
        {
            error = "Generation profile is required.";
            return false;
        }

        if (!TryParseAllowedColumns(allowedColumnsValue, out var allowedColumns))
        {
            error = "Allowed columns must contain integers between 0 and 4.";
            return false;
        }

        if (!TryParseDouble(weightValue, out var weight))
        {
            error = "Enter valid numeric values.";
            return false;
        }

        if (weight is < 0.0001 or > 1000000)
        {
            error = "Weight must be between 0.0001 and 1000000.";
            return false;
        }

        result = new CreateEnemyClassProfileDialogResult
        {
            Name = name.Trim(),
            Class = enemyClass,
            AllowedColumns = allowedColumns,
            GenerationProfileId = generationProfile.Id,
            Weight = weight
        };

        return true;
    }

    private static bool TryParseAllowedColumns(string? value, out List<int> allowedColumns)
    {
        allowedColumns = new List<int>();

        if (string.IsNullOrWhiteSpace(value))
            return false;

        foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!int.TryParse(part, NumberStyles.Integer, CultureInfo.CurrentCulture, out var column))
                return false;

            if (column is < 0 or > 4)
                return false;

            allowedColumns.Add(column);
        }

        return allowedColumns.Count > 0;
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
