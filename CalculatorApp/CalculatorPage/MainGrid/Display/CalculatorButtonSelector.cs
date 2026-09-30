using CalculatorApp.CalculatorPage.Buttons;

namespace CalculatorApp.CalculatorPage.MainGrid.Display;

public class CalculatorButtonSelector : DataTemplateSelector
{
    public required DataTemplate AngleModeButtonTemplate { get; init; }
    public required DataTemplate BackspaceButtonTemplate { get; init; }
    public required DataTemplate EqualButtonTemplate { get; init; }
    public required DataTemplate InverseButtonTemplate { get; init; }
    public required DataTemplate NumberButtonTemplate { get; init; }
    public required DataTemplate PrimaryButtonTemplate { get; init; }
    public required DataTemplate SecondaryButtonTemplate { get; init; }
    public required DataTemplate TrigButtonTemplate { get; init; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) =>
        item switch
        {
            AngleModeButtonViewModel => AngleModeButtonTemplate,
            BackspaceButtonViewModel => BackspaceButtonTemplate,
            EqualButtonViewModel => EqualButtonTemplate,
            InverseButtonViewModel => InverseButtonTemplate,
            NumberButtonViewModel => NumberButtonTemplate,
            PrimaryButtonViewModel => PrimaryButtonTemplate,
            SecondaryButtonViewModel => SecondaryButtonTemplate,
            AlternateButtonViewModel => TrigButtonTemplate,
            DecimalSeparatorButtonViewModel => NumberButtonTemplate,
            _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
        };
}
