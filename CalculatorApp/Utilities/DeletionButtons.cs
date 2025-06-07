using CalculatorApp.UIComponents;

namespace CalculatorApp.Utilities;

internal class DeletionButtons
{
    private readonly Button _clear, _backspace;

    public DeletionButtons(Button backspace, Button clear)
    {
        (_backspace, _clear) = (backspace, clear);

        _backspace.IsEnabled = false;
        _clear.IsEnabled = false;

        CalculatorInputField.CursorPositionChanged += BackspaceEnabled;
        CalculatorInputField.TextLengthChanged += ClearEnabled; 
    }

    private void BackspaceEnabled(int cursorPosition) =>
        _backspace.IsEnabled = cursorPosition != 0;

    private void ClearEnabled(int textLength) =>
        _clear.IsEnabled = textLength != 0;
}