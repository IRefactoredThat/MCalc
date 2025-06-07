using System.ComponentModel;

namespace CalculatorApp.UIComponents;

public partial class CalculatorButton : Button
{
    private const double FontSizeFactor = 0.4d;

    private readonly Color _startingColor = Colors.White;
    private readonly Color _disabledColor = Colors.DimGray;

    public delegate void FontSizeDelegate(double updatedSize);
    public static event FontSizeDelegate? FontSizeChanged;

    public delegate void ButtonSizeDelegate(double width, double height);
    public static event ButtonSizeDelegate? ButtonSizeChanged;

    public CalculatorButton()
    {
        InitializeComponent();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsEnabled))
        {
            TextColor = IsEnabled ? _startingColor : _disabledColor;
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        button.CornerRadius = (int)(Math.Min(button.Width, button.Height) / 2);
        ButtonSizeChanged?.Invoke(button.Width, button.Height);

        var minSize = Math.Min(button.Width, button.Height);
        var newFontSize = minSize * FontSizeFactor;
        button.FontSize = newFontSize;
        FontSizeChanged?.Invoke(newFontSize);
    }
}