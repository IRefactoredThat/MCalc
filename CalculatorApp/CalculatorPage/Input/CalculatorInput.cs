namespace CalculatorApp.CalculatorPage.Input;

public class CalculatorInput : Entry
{
    public static readonly BindableProperty CursorColorProperty =
        BindableProperty.Create(
            nameof(CursorColor),
            typeof(Color),
            typeof(CalculatorInput),
            Colors.Black);

    public Color CursorColor
    {
        get => (Color)GetValue(CursorColorProperty);
        set => SetValue(CursorColorProperty, value);
    }

    public static readonly BindableProperty NormalizePositionProperty =
        BindableProperty.Create(
            nameof(NormalizePosition),
            typeof(Func<int, int>),
            typeof(CalculatorInput));

    public Func<int, int> NormalizePosition
    {
        get => (Func<int, int>)GetValue(NormalizePositionProperty);
        set => SetValue(NormalizePositionProperty, value);
    }
}