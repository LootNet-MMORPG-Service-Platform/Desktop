using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Enums;
using desktop_app.Models.EnemyGeneration;
using desktop_app.Services.Dialogs.Generation.EnemyGeneration;

namespace desktop_app.Services.Dialogs.Generation;

public static class EnemyGenerationDialogs
{
    public static async Task<CreateStageProfileDialogResult?> ShowCreateStageProfileDialogAsync(Window owner)
    {
        return await StageProfileEnemyGenerationDialogs.ShowCreateStageProfileDialogAsync(owner);
    }

    public static async Task<CreateStageScenarioDialogResult?> ShowCreateStageScenarioDialogAsync(Window owner)
    {
        return await StageScenarioEnemyGenerationDialogs.ShowCreateStageScenarioDialogAsync(owner);
    }

    public static async Task<CreateScenarioSlotDialogResult?> ShowCreateScenarioSlotDialogAsync(
        Window owner,
        IEnumerable<EnemyClassProfile> classProfiles)
    {
        return await ScenarioSlotEnemyGenerationDialogs.ShowCreateScenarioSlotDialogAsync(owner, classProfiles);
    }

    public static async Task<CreateEnemyClassProfileDialogResult?> ShowCreateEnemyClassProfileDialogAsync(
        Window owner,
        IEnumerable<StageProfile> generationProfiles)
    {
        return await EnemyClassProfileEnemyGenerationDialogs.ShowCreateEnemyClassProfileDialogAsync(
            owner,
            generationProfiles);
    }

    internal static TextBox CreateTextBox(string watermark)
    {
        return new TextBox
        {
            Width = 230,
            Watermark = watermark,
            HorizontalAlignment = HorizontalAlignment.Center
        };
    }

    internal static TextBlock CreateLabel(string text)
    {
        return new TextBlock
        {
            Text = text,
            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray)
        };
    }

    internal static bool TryCreateStageProfileResult(
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

    internal static bool TryCreateScenarioResult(
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

    internal static bool TryCreateSlotResult(
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

    internal static bool TryCreateClassProfileResult(
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

    internal static bool TryParseAllowedColumns(string? value, out List<int> allowedColumns)
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

    internal static bool TryParseDouble(string? value, out double result)
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
