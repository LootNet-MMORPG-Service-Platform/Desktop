using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace desktop_app.Converters;

public class BooleanToStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? "Blocked" : "Active";

        return "Unknown";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}