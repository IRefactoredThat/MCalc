using System.Globalization;
using static System.Convert;

namespace CalculatorApp.SettingsPage.Types.StepSlider;

public class IntToDoubleConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var result = ToDouble(value);
        return result;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var result = (int)ToDouble(value);
        return result;
    }
}
