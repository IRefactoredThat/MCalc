using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CalculatorApp.SettingsPage.Formatting;

public class Preview : INotifyPropertyChanged
{
    public string Text
    {
        get;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }
    } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class NumberPreview
{
    public ObservableCollection<Preview> Previews { get; } = [];

    public NumberPreview()
    {
        InitializeComponent();
        Previews.Add(new Preview() { Text = BuildDecimalPreview() });
        Previews.Add(new Preview() { Text = BuildENotationPreview() });
    }

    public static readonly BindableProperty FractionalPartLengthProperty =
        BindableProperty.Create(
            nameof(FractionalPartLength),
            typeof(int),
            typeof(NumberPreview),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnFractionalPartLengthChanged);

    public int FractionalPartLength
    {
        get => (int)GetValue(FractionalPartLengthProperty);
        set => SetValue(FractionalPartLengthProperty, value);
    }

    public static readonly BindableProperty MaxLengthProperty =
        BindableProperty.Create(
            nameof(MaxLength),
            typeof(int),
            typeof(NumberPreview),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnMaxLengthChanged);

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public static readonly BindableProperty ENotationExponentThresholdProperty =
        BindableProperty.Create(
            nameof(ENotationExponentThreshold),
            typeof(int),
            typeof(NumberPreview),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnENotationExponentThresholdChanged);

    public int ENotationExponentThreshold
    {
        get => (int)GetValue(ENotationExponentThresholdProperty);
        set => SetValue(ENotationExponentThresholdProperty, value);
    }

    public static readonly BindableProperty DecimalSeparatorProperty =
        BindableProperty.Create(
            nameof(DecimalSeparator),
            typeof(char),
            typeof(NumberPreview),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnDecimalSeparatorChanged);

    public char DecimalSeparator
    {
        get => (char)GetValue(DecimalSeparatorProperty);
        set => SetValue(DecimalSeparatorProperty, value);
    }

    public static readonly BindableProperty ThousandSeparatorProperty =
        BindableProperty.Create(
            nameof(ThousandSeparator),
            typeof(char),
            typeof(NumberPreview),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnThousandSeparatorChanged);

    public char ThousandSeparator
    {
        get => (char)GetValue(ThousandSeparatorProperty);
        set => SetValue(ThousandSeparatorProperty, value);
    }

    private static void OnFractionalPartLengthChanged(BindableObject bindable, object oldValue, object newValue) =>
        UpdateDecimalPreview(bindable);

    private static void OnMaxLengthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        UpdateDecimalPreview(bindable);
        UpdateENotationPreview(bindable);
    }

    private static void OnENotationExponentThresholdChanged(BindableObject bindable, object oldValue, object newValue) =>
        UpdateENotationPreview(bindable);

    private static void OnDecimalSeparatorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        UpdateDecimalPreview(bindable);
        UpdateENotationPreview(bindable);
    }

    private static void OnThousandSeparatorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        UpdateDecimalPreview(bindable);
        UpdateENotationPreview(bindable);
    }

    private static void UpdateDecimalPreview(BindableObject bindable)
    {
        if (bindable is not NumberPreview numberPreviews ||
            numberPreviews.Previews is [])
        {
            return;
        }

        numberPreviews.Previews[0].Text = numberPreviews.BuildDecimalPreview();
    }

    private static void UpdateENotationPreview(BindableObject bindable)
    {
        if (bindable is not NumberPreview numberPreviews ||
            numberPreviews.Previews is [])
        {
            return;
        }

        numberPreviews.Previews[1].Text = numberPreviews.BuildENotationPreview();
    }

    private string BuildENotationPreview()
    {
        if (MaxLength <= 0)
            return string.Empty;

        int exponent = Math.Max(0, ENotationExponentThreshold);

        if (exponent == 0)
        {
            return $"1{DecimalSeparator}{new string('1', MaxLength)}";
        }

        int decimalDigits = Math.Max(1, MaxLength - exponent);
        string mantissa = $"1{DecimalSeparator}{new string('1', decimalDigits)}";
        return $"{mantissa}E-{exponent}";
    }

    private string BuildDecimalPreview()
    {
        if (MaxLength <= 0)
        {
            return string.Empty;
        }

        var fractionalLength = Math.Clamp(
            FractionalPartLength,
            0,
            MaxLength - 1);

        var integerLength = MaxLength - fractionalLength;

        var thousandSeparatorCount = ThousandSeparator == '\0'
            ? 0
            : (integerLength - 1) / 3;

        var decimalSeparatorCount = fractionalLength > 0 ? 1 : 0;

        var resultLength =
            MaxLength +
            thousandSeparatorCount +
            decimalSeparatorCount;

        return string.Create(
            resultLength,
            (integerLength, fractionalLength, DecimalSeparator, ThousandSeparator),
            static (span, state) =>
            {
                var (integerLength, fractionalLength, decimalSeparator, thousandSeparator) = state;

                var outputIndex = 0;

                for (var i = 0; i < integerLength; i++)
                {
                    if (i > 0 &&
                        thousandSeparator != '\0' &&
                        (integerLength - i) % 3 == 0)
                    {
                        span[outputIndex++] = thousandSeparator;
                    }

                    span[outputIndex++] = (char)('1' + (i % 9));
                }

                if (fractionalLength > 0)
                {
                    span[outputIndex++] = decimalSeparator;

                    for (var i = 0; i < fractionalLength; i++)
                    {
                        span[outputIndex++] = (char)('1' + (i % 9));
                    }
                }
            });
    }
}
