using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.MainGrid.Models;

public class SimpleGridViewModel(FormattingSettings formattingSettings) : IGridViewModel
{
    private Dictionary<CellRange, GridViewModelPart> _parts =
        new()
        {
            [new CellRange(0, 0, 1, 4)] = new(SubgridLayoutMode.Grid)
            {
                new PrimaryButtonViewModel(OpenBracketToken.Instance),
                new PrimaryButtonViewModel(PercentToken.Instance),
                new PrimaryButtonViewModel(ClosedBracketToken.Instance),
                new PrimaryButtonViewModel(DivToken.Instance),
            },
            [new CellRange(1, 0, 4, 4)] = new(SubgridLayoutMode.Grid)
            {
                new NumberButtonViewModel(NumberToken.Seven),
                new NumberButtonViewModel(NumberToken.Eight),
                new NumberButtonViewModel(NumberToken.Nine),
                new PrimaryButtonViewModel(TimesToken.Instance),

                new NumberButtonViewModel(NumberToken.Four),
                new NumberButtonViewModel(NumberToken.Five),
                new NumberButtonViewModel(NumberToken.Six),
                new PrimaryButtonViewModel(MinusToken.Instance),

                new NumberButtonViewModel(NumberToken.One),
                new NumberButtonViewModel(NumberToken.Two),
                new NumberButtonViewModel(NumberToken.Three),
                new PrimaryButtonViewModel(PlusToken.Instance),

                formattingSettings.DecimalSeparatorButtonViewModel,
                new NumberButtonViewModel(NumberToken.Zero),
                new BackspaceButtonViewModel(),
                new EqualButtonViewModel()
            }
        };

    public event EventHandler<PartChangedEventArgs>? PartChanged;

    public int UnitWidth => 4;
    public int UnitHeight => 5;

    public IReadOnlyDictionary<CellRange, GridViewModelPart> Parts => _parts;

    public void Update(bool includeClearButton)
    {
        var (affectedCell, affectedPart) = _parts.First();
        var hasClearButton = affectedPart.FirstOrDefault() is ClearButtonViewModel;

        if (includeClearButton && !hasClearButton)
        {
            affectedPart.ColumnSpanExtension++;
            affectedPart.Insert(0, new ClearButtonViewModel());
        }

        if (!includeClearButton && hasClearButton)
        {
            affectedPart.ColumnSpanExtension--;
            affectedPart.RemoveAt(0);
        }

        PartChanged?.Invoke(this, new PartChangedEventArgs(
            affectedCell, PartChangedAction.Update, affectedPart));
    }
}
