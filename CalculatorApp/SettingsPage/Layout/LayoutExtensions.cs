using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.SettingsPage.Layout;

public static class LayoutExtensions
{
    public static IBaseButtonViewModel[] Create(
        FormattingSettings formattingSettings) =>
        [
            formattingSettings.DecimalSeparatorButtonViewModel,
            new NumberButtonViewModel(NumberToken.Zero),
            new NumberButtonViewModel(NumberToken.One),
            new NumberButtonViewModel(NumberToken.Two),
            new NumberButtonViewModel(NumberToken.Three),
            new NumberButtonViewModel(NumberToken.Four),
            new NumberButtonViewModel(NumberToken.Five),
            new NumberButtonViewModel(NumberToken.Six),
            new NumberButtonViewModel(NumberToken.Seven),
            new NumberButtonViewModel(NumberToken.Eight),
            new NumberButtonViewModel(NumberToken.Nine),

            new PrimaryButtonViewModel(PlusToken.Instance),
            new PrimaryButtonViewModel(MinusToken.Instance),
            new PrimaryButtonViewModel(TimesToken.Instance),
            new PrimaryButtonViewModel(DivToken.Instance),
            new PrimaryButtonViewModel(CaretToken.Instance),
            new PrimaryButtonViewModel(PercentToken.Instance),
            new PrimaryButtonViewModel(ExclamationMarkToken.Instance),
            new PrimaryButtonViewModel(OpenBracketToken.Instance),
            new PrimaryButtonViewModel(ClosedBracketToken.Instance),

            new SecondaryButtonViewModel(SqrtToken.Instance),
            new SecondaryButtonViewModel(CbrtToken.Instance),
            new SecondaryButtonViewModel(LogToken.Instance),
            new SecondaryButtonViewModel(LnToken.Instance),
            new SecondaryButtonViewModel(EToken.Instance),
            new SecondaryButtonViewModel(PiToken.Instance),

            new AlternateButtonViewModel(
                SinToken.Instance, AsinToken.Instance, LayoutItemCategory.Trigonometric),
            new AlternateButtonViewModel(
                CosToken.Instance, AcosToken.Instance, LayoutItemCategory.Trigonometric),
            new AlternateButtonViewModel(
                TanToken.Instance, AtanToken.Instance, LayoutItemCategory.Trigonometric),
            new AlternateButtonViewModel(
                CotToken.Instance, AcotToken.Instance, LayoutItemCategory.Trigonometric),
            new AlternateButtonViewModel(
                SecToken.Instance, AsecToken.Instance, LayoutItemCategory.Trigonometric),
            new AlternateButtonViewModel(
                CscToken.Instance, AcscToken.Instance, LayoutItemCategory.Trigonometric),

            new AlternateButtonViewModel(
                SinhToken.Instance, AsinhToken.Instance, LayoutItemCategory.Hyperbolic),
            new AlternateButtonViewModel(
                CoshToken.Instance, AcoshToken.Instance, LayoutItemCategory.Hyperbolic),
            new AlternateButtonViewModel(
                TanhToken.Instance, AtanhToken.Instance, LayoutItemCategory.Hyperbolic),
            new AlternateButtonViewModel(
                CothToken.Instance, AcothToken.Instance, LayoutItemCategory.Hyperbolic),
            new AlternateButtonViewModel(
                SechToken.Instance, AsechToken.Instance, LayoutItemCategory.Hyperbolic),
            new AlternateButtonViewModel(
                CschToken.Instance, AcschToken.Instance, LayoutItemCategory.Hyperbolic),

            new AngleModeButtonViewModel(),
            new BackspaceButtonViewModel(),
            new EqualButtonViewModel(),
            new InverseButtonViewModel()
        ];
}
