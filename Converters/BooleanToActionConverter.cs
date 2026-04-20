using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace desktop_app.Converters;

public class BooleanToActionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? "Unblock" : "Block";

        return "Action";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}