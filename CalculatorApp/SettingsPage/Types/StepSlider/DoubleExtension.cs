namespace CalculatorApp.SettingsPage.Types.StepSlider;

[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Value))]

public class DoubleExtension : IMarkupExtension<double>
{
    public int Value { get; set; }

    public double ProvideValue(IServiceProvider serviceProvider)
        => Convert.ToDouble(Value);

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        => ProvideValue(serviceProvider);
}