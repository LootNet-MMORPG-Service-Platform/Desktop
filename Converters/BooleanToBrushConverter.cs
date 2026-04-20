using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace desktop_app.Converters;

public class BooleanToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b
                ? new SolidColorBrush(Color.Parse("#16A34A"))
                : new SolidColorBrush(Color.Parse("#DC2626"));
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}