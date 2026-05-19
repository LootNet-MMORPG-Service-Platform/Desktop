using System;
using System.Globalization;
using Avalonia.Data.Converters;
using desktop_app.Models.Economy;

namespace desktop_app.Converters;

public class TaxBracketDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not MarketTaxBracket bracket)
            return "";

        var from = FormatDecimal(bracket.From, culture);
        var rate = FormatDecimal(bracket.Rate * 100, culture);

        if (bracket.To == null)
            return $"{from}+ → {rate}%";

        var to = FormatDecimal(bracket.To.Value, culture);
        return $"{from} - {to} → {rate}%";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static string FormatDecimal(decimal value, CultureInfo culture)
    {
        return value.ToString("0.##", culture);
    }
}
