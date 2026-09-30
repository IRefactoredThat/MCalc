namespace CalculatorApp.SharedElements.StepSlider;

public class StepSlider : Slider
{
    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(
            nameof(Step),
            typeof(double),
            typeof(StepSlider),
            1d);

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }
}
