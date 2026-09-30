using System.Globalization;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.Resources.Fonts;

namespace CalculatorApp.SettingsPage.CustomLayout.LayoutBuilder;

public class LayoutModeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SubgridLayoutMode layoutMode)
        {
            return null;
        }

        return layoutMode switch
        {
            SubgridLayoutMode.Grid => MaterialSymbols.GridLayout,
            SubgridLayoutMode.Carousel => MaterialSymbols.CarouselLayout,
            _ => throw new ArgumentOutOfRangeException(nameof(layoutMode), layoutMode, null)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string symbol)
        {
            return null;
        }

        return symbol switch
        {
            MaterialSymbols.GridLayout => SubgridLayoutMode.Grid,
            MaterialSymbols.CarouselLayout => SubgridLayoutMode.Carousel,
            _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
        };
    }
}
