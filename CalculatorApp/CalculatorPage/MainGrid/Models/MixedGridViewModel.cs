using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.Layout;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.MainGrid.Models;

public class MixedGridViewModel(FormattingSettings formattingSettings) : IGridViewModel
{
    public int UnitHeight => 8;
    public int UnitWidth => 4;

    public IReadOnlyDictionary<CellRange, GridViewModelPart> Parts { get; } =
        new Dictionary<CellRange, GridViewModelPart>()
        {
            [new CellRange(0, 0, 1, 4)] = new(SubgridLayoutMode.Grid, columnSpanExtension: 1)
            {
                new SecondaryButtonViewModel(token: ExclamationMarkToken.Instance),
                new SecondaryButtonViewModel(token: CaretToken.Instance),
                new SecondaryButtonViewModel(token: SqrtToken.Instance),
                new SecondaryButtonViewModel(token: PiToken.Instance),
                new SecondaryButtonViewModel(token: EToken.Instance)
            },
            [new CellRange(1, 0, 1, 1)] = new(SubgridLayoutMode.Grid)
            {
                new AngleModeButtonViewModel()
            },
            [new CellRange(2, 0, 1, 1)] = new(SubgridLayoutMode.Grid)
            {
                new InverseButtonViewModel()
            },
            [new CellRange(1, 1, 2, 3)] = new(SubgridLayoutMode.Carousel)
            {
                new AlternateButtonViewModel(token: SinToken.Instance,
                    alternateToken: AsinToken.Instance, LayoutItemCategory.Trigonometric),

                new AlternateButtonViewModel(token: CosToken.Instance,
                    alternateToken: AcosToken.Instance, LayoutItemCategory.Trigonometric),

                new AlternateButtonViewModel(token: SecToken.Instance,
                    alternateToken: AsecToken.Instance, LayoutItemCategory.Trigonometric),


                new AlternateButtonViewModel(token: TanToken.Instance,
                    alternateToken: AtanToken.Instance, LayoutItemCategory.Trigonometric),

                new AlternateButtonViewModel(token: CotToken.Instance,
                    alternateToken: AcotToken.Instance, LayoutItemCategory.Trigonometric),

                new AlternateButtonViewModel(token: CscToken.Instance,
                    alternateToken: AcscToken.Instance, LayoutItemCategory.Trigonometric),


                new AlternateButtonViewModel(token: SinhToken.Instance,
                    alternateToken: AsinhToken.Instance, LayoutItemCategory.Hyperbolic),

                new AlternateButtonViewModel(token: CoshToken.Instance,
                    alternateToken: AcoshToken.Instance, LayoutItemCategory.Hyperbolic),

                new AlternateButtonViewModel(token: SechToken.Instance,
                    alternateToken: AsechToken.Instance, LayoutItemCategory.Hyperbolic),


                new AlternateButtonViewModel(token: TanhToken.Instance,
                    alternateToken: AtanhToken.Instance, LayoutItemCategory.Hyperbolic),

                new AlternateButtonViewModel(token: CothToken.Instance,
                    alternateToken: AcothToken.Instance, LayoutItemCategory.Hyperbolic),

                new AlternateButtonViewModel(token: CschToken.Instance,
                    alternateToken: AcschToken.Instance, LayoutItemCategory.Hyperbolic)
            },
            [new CellRange(3, 0, 5, 4)] = new(SubgridLayoutMode.Grid)
            {
                new PrimaryButtonViewModel(OpenBracketToken.Instance),
                new PrimaryButtonViewModel(PercentToken.Instance),
                new PrimaryButtonViewModel(ClosedBracketToken.Instance),
                new PrimaryButtonViewModel(PlusToken.Instance),

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
                new PrimaryButtonViewModel(DivToken.Instance),

                formattingSettings.DecimalSeparatorButtonViewModel,
                new NumberButtonViewModel(NumberToken.Zero),
                new BackspaceButtonViewModel(),
                new EqualButtonViewModel()
            }
        };
}
