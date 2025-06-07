using static Calculator.ExpressionPartsMapping.ExpressionPartSymbols;

namespace CalculatorApp.TransitionButtons;

internal class ShiftButtons
{
    private readonly Dictionary<Button, AlternateTextTransition> _hypSwitchButton;
    private readonly Dictionary<Button, AlternateTextTransition> _menuSwitchButton;
    private readonly Dictionary<Button, AlternateTextTransition> _menuTransitions;
    private readonly Dictionary<Button, TrigonometricFunctionTransition>
        _trigonometricTransitions;

    public ShiftButtons(Grid grid)
    {
        var textToButtons = grid.Children
            .OfType<Button>()
            .ToDictionary(button => button.Text, button => button);

        _hypSwitchButton = new ButtonTextTransitionBuilder<AlternateTextTransition>()
            .Add(textToButtons["CIR"], new("CIR", "HYP"))
            .Build();

        _menuSwitchButton = new ButtonTextTransitionBuilder<AlternateTextTransition>()
            .Add(textToButtons["1/2"], new("1/2", "2/2"))
            .Build();

        _trigonometricTransitions = new ButtonTextTransitionBuilder
                <TrigonometricFunctionTransition>()
            .Add(textToButtons[Sin], new())
            .Add(textToButtons[Cos], new())
            .Add(textToButtons[Tan], new())
            .Add(textToButtons[Cot], new())
            .Add(textToButtons[Csc], new())
            .Add(textToButtons[Sec], new())
            .Build();

        _menuTransitions = new ButtonTextTransitionBuilder<AlternateTextTransition>()
            .Add(textToButtons[Sqrt], new(Sqrt, $"{Caret}2"))
            .Add(textToButtons[Cbrt], new(Cbrt, $"{Caret}3"))
            .Add(textToButtons[Log], new(Log, $"10{Caret}"))
            .Add(textToButtons[Ln], new(Ln, $"{E}{Caret}"))
            .Add(textToButtons[Caret], new(Caret, $"{Caret}{Open}1{Slash}"))
            .Build();
    }

    public void UpdateButtons(bool swapToHypFunctions)
    {
        if(swapToHypFunctions)
        {
            foreach (var button in _trigonometricTransitions.Keys)
            {
                button.Text = TrigonometricFunctionTransition.UpdateValueToHyp(button.Text);
            }
            foreach(var (button, transition) in _hypSwitchButton)
            {
                button.Text = transition.UpdateValue(button.Text);
            }
        }
        else
        {
            foreach(var button in _trigonometricTransitions.Keys)
            {
                button.Text = TrigonometricFunctionTransition.UpdateValueToCir(button.Text);
            }
            foreach(var (button, transition) in _menuTransitions)
            {
                button.Text = transition.UpdateValue(button.Text);
            }
            foreach (var (button, transition) in _menuSwitchButton)
            {
                button.Text = transition.UpdateValue(button.Text);
            }
        }
    }
}