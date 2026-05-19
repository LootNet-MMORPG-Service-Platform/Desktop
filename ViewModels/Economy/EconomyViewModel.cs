using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using desktop_app.Models.Economy;
using desktop_app.Services;
using desktop_app.Services.Economy;

namespace desktop_app.ViewModels.Economy;

public partial class EconomyViewModel : ViewModelBase
{
    private EconomyAdminService _service;

    public ObservableCollection<EconomyStatItem> StatItems { get; } = new();

    public bool HasSettings => Settings != null;
    public bool HasStats => Stats != null && StatItems.Count > 0;
    public bool HasProgressiveTaxBrackets => Settings?.ProgressiveTaxBrackets.Count > 0;

    [ObservableProperty]
    private EconomySettings? _settings;

    [ObservableProperty]
    private MarketStats? _stats;

    [ObservableProperty]
    private string _statusMessage = "Economy section ready.";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _dailyCurrencyRewardValue = "";

    [ObservableProperty]
    private string _dailyCurrencyRewardError = "";

    [ObservableProperty]
    private string _botBasePriceValue = "";

    [ObservableProperty]
    private string _botBasePriceError = "";

    [ObservableProperty]
    private string _botStatMultiplierValue = "";

    [ObservableProperty]
    private string _botStatMultiplierError = "";

    [ObservableProperty]
    private string _botElementMultiplierValue = "";

    [ObservableProperty]
    private string _botElementMultiplierError = "";

    [ObservableProperty]
    private bool _isPlayerToPlayerTaxEnabled;

    [ObservableProperty]
    private bool _isPlayerToBotTaxEnabled;

    public bool HasDailyCurrencyRewardError => !string.IsNullOrWhiteSpace(DailyCurrencyRewardError);
    public bool HasBotBasePriceError => !string.IsNullOrWhiteSpace(BotBasePriceError);
    public bool HasBotStatMultiplierError => !string.IsNullOrWhiteSpace(BotStatMultiplierError);
    public bool HasBotElementMultiplierError => !string.IsNullOrWhiteSpace(BotElementMultiplierError);

    public EconomyViewModel(EconomyAdminService service)
    {
        _service = service;
    }

    public void UpdateService(EconomyAdminService service)
    {
        _service = service;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunEconomyActionAsync(async () =>
        {
            IsLoading = true;
            StatusMessage = "Loading economy data...";

            Settings = await _service.GetEconomyAsync();
            Stats = await _service.GetStatsAsync();

            PopulateSettings();
            PopulateStats();

            StatusMessage = "Economy data loaded.";
        });
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Settings == null)
            return;

        if (!TryCreateUpdateRequest(out var request))
        {
            NotificationService.Instance.ShowError("Invalid economy value. Please check numeric fields.");
            return;
        }

        await RunEconomyActionAsync(async () =>
        {
            IsLoading = true;

            await _service.UpdateEconomyAsync(request);
            NotificationService.Instance.ShowSuccess("Economy settings updated.");

            await LoadAsync();
        });
    }

    private void PopulateSettings()
    {
        ClearValidationErrors();

        if (Settings == null)
        {
            RefreshState();
            return;
        }

        DailyCurrencyRewardValue = FormatDecimal(Settings.DailyCurrencyReward);
        BotBasePriceValue = FormatDecimal(Settings.BotBasePrice);
        BotStatMultiplierValue = FormatDecimal(Settings.BotStatMultiplier);
        BotElementMultiplierValue = FormatDecimal(Settings.BotElementMultiplier);
        IsPlayerToPlayerTaxEnabled = Settings.IsPlayerToPlayerTaxEnabled;
        IsPlayerToBotTaxEnabled = Settings.IsPlayerToBotTaxEnabled;

        RefreshState();
    }

    private void PopulateStats()
    {
        StatItems.Clear();

        if (Stats == null)
        {
            RefreshState();
            return;
        }

        foreach (var property in Stats.Properties)
        {
            StatItems.Add(new EconomyStatItem(
                CreateLabel(property.Key),
                FormatJsonValue(property.Value)));
        }

        RefreshState();
    }

    private bool TryCreateUpdateRequest(out UpdateEconomySettingsRequest request)
    {
        request = new UpdateEconomySettingsRequest();
        ClearValidationErrors();

        var isValid = true;

        if (!TryParseNonNegativeDecimal(DailyCurrencyRewardValue, out var dailyCurrencyReward))
        {
            DailyCurrencyRewardError = GetNumberError(DailyCurrencyRewardValue);
            isValid = false;
        }

        if (!TryParseNonNegativeDecimal(BotBasePriceValue, out var botBasePrice))
        {
            BotBasePriceError = GetNumberError(BotBasePriceValue);
            isValid = false;
        }

        if (!TryParseNonNegativeDecimal(BotStatMultiplierValue, out var botStatMultiplier))
        {
            BotStatMultiplierError = GetNumberError(BotStatMultiplierValue);
            isValid = false;
        }

        if (!TryParseNonNegativeDecimal(BotElementMultiplierValue, out var botElementMultiplier))
        {
            BotElementMultiplierError = GetNumberError(BotElementMultiplierValue);
            isValid = false;
        }

        if (!isValid || Settings == null)
            return false;

        request = new UpdateEconomySettingsRequest
        {
            DailyCurrencyReward = dailyCurrencyReward,
            BotBasePrice = botBasePrice,
            BotStatMultiplier = botStatMultiplier,
            BotElementMultiplier = botElementMultiplier,
            IsPlayerToPlayerTaxEnabled = IsPlayerToPlayerTaxEnabled,
            IsPlayerToBotTaxEnabled = IsPlayerToBotTaxEnabled,
            ProgressiveTaxBrackets = Settings.ProgressiveTaxBrackets
                .OrderBy(x => x.From)
                .ThenBy(x => x.To ?? decimal.MaxValue)
                .ThenBy(x => x.Rate)
                .GroupBy(x => new { x.From, x.To, x.Rate })
                .Select(x => x.First())
                .ToList()
        };

        return true;
    }

    private async Task RunEconomyActionAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == null)
        {
            StatusMessage = "Failed to load economy data.";
            NotificationService.Instance.ShowError("API unavailable. Check if the server is running.");
        }
        catch (HttpRequestException)
        {
            StatusMessage = "Operation failed.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
        catch (Exception)
        {
            StatusMessage = "Operation failed.";
            NotificationService.Instance.ShowError("Operation failed. Please try again.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearValidationErrors()
    {
        DailyCurrencyRewardError = "";
        BotBasePriceError = "";
        BotStatMultiplierError = "";
        BotElementMultiplierError = "";
    }

    public void ResetSessionState()
    {
        Settings = null;
        Stats = null;
        StatItems.Clear();
        DailyCurrencyRewardValue = "";
        BotBasePriceValue = "";
        BotStatMultiplierValue = "";
        BotElementMultiplierValue = "";
        IsPlayerToPlayerTaxEnabled = false;
        IsPlayerToBotTaxEnabled = false;
        IsLoading = false;
        StatusMessage = "Economy section ready.";
        ClearValidationErrors();
        RefreshState();
    }

    private void RefreshState()
    {
        OnPropertyChanged(nameof(HasSettings));
        OnPropertyChanged(nameof(HasStats));
        OnPropertyChanged(nameof(HasProgressiveTaxBrackets));
    }

    partial void OnSettingsChanged(EconomySettings? value)
    {
        _ = value;
        RefreshState();
    }

    partial void OnStatsChanged(MarketStats? value)
    {
        _ = value;
        RefreshState();
    }

    partial void OnDailyCurrencyRewardValueChanged(string value)
    {
        _ = value;
        DailyCurrencyRewardError = "";
    }

    partial void OnDailyCurrencyRewardErrorChanged(string value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasDailyCurrencyRewardError));
    }

    partial void OnBotBasePriceValueChanged(string value)
    {
        _ = value;
        BotBasePriceError = "";
    }

    partial void OnBotBasePriceErrorChanged(string value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasBotBasePriceError));
    }

    partial void OnBotStatMultiplierValueChanged(string value)
    {
        _ = value;
        BotStatMultiplierError = "";
    }

    partial void OnBotStatMultiplierErrorChanged(string value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasBotStatMultiplierError));
    }

    partial void OnBotElementMultiplierValueChanged(string value)
    {
        _ = value;
        BotElementMultiplierError = "";
    }

    partial void OnBotElementMultiplierErrorChanged(string value)
    {
        _ = value;
        OnPropertyChanged(nameof(HasBotElementMultiplierError));
    }

    private static bool TryParseNonNegativeDecimal(string value, out decimal result)
    {
        if (!TryParseFlexibleDecimal(value, out result))
            return false;

        return result >= 0;
    }

    private static bool TryParseFlexibleDecimal(string value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result)
            || decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result)
            || decimal.TryParse(
                value.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out result);
    }

    private static string GetNumberError(string value)
    {
        return TryParseFlexibleDecimal(value, out var number) && number < 0
            ? "Value cannot be negative."
            : "Enter a valid number.";
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString(CultureInfo.CurrentCulture);
    }

    private static string FormatJsonValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? "",
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "",
            _ => value.GetRawText()
        };
    }

    private static string CreateLabel(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return key;

        var builder = new StringBuilder();

        for (var i = 0; i < key.Length; i++)
        {
            var current = key[i];

            if (ShouldInsertLabelSpace(key, i))
                builder.Append(' ');

            builder.Append(i == 0 ? char.ToUpperInvariant(current) : current);
        }

        return builder.ToString();
    }

    private static bool ShouldInsertLabelSpace(string key, int index)
    {
        if (index <= 0)
            return false;

        var current = key[index];
        var previous = key[index - 1];
        var next = index + 1 < key.Length ? key[index + 1] : '\0';

        if (!char.IsUpper(current) || char.IsWhiteSpace(previous))
            return false;

        if (char.IsUpper(previous))
            return next != '\0' && char.IsLower(next);

        return char.IsLower(previous);
    }
}

public class EconomyStatItem
{
    public string Label { get; }
    public string Value { get; }

    public EconomyStatItem(string label, string value)
    {
        Label = label;
        Value = value;
    }
}
