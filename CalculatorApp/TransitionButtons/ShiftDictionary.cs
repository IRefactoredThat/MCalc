namespace CalculatorApp.TransitionButtons;

internal class ButtonTextTransitionBuilder<TTransition> where TTransition : ITextTransition
{
    private readonly Dictionary<Button, TTransition> _builder = [];

    public ButtonTextTransitionBuilder<TTransition> Add(Button button, TTransition transition)
    {
        _builder[button] = transition;
        return this;
    }

    public Dictionary<Button, TTransition> Build() => _builder;
}