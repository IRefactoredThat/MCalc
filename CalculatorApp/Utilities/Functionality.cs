using CommunityToolkit.Maui.Alerts;
using Essentials.ResultType;
using Essentials.ErrorType;
using Essentials.Calculator;
using Calculator.ExpressionSolving;
using CalculatorApp.History;
using CalculatorApp.TextFormatting;
using CalculatorApp.TransitionButtons;
using CalculatorApp.Animations;

namespace CalculatorApp.Utilities;

class Functionality(Grid grid, Editor inputField, Label outputField, 
    Button angleModeButton)
{
    private readonly ShiftButtons _shiftButton = new(grid);
    private readonly AngleModeButton _angleModeButton = new(angleModeButton);
    private readonly Editor _inputField = inputField;
    private readonly Label _outputField = outputField;

    private Result<string> lastResult = string.Empty;

    public string Evaluate(string inputText)
    {
        var formattedResult = ExpressionEvaluatingExtensions.EvaluateAndRound
            (inputText, _angleModeButton.Mode).Map
            (value => value.ToFormattedOutput().ToResult());

        formattedResult.Switch(
            operand => _outputField.Text = operand,
            error => _outputField.Text = string.Empty);

        lastResult = formattedResult;
        return inputText;
    }

    public async Task EqualButton(bool canWriteResult) =>
        await lastResult.SwitchAsync(
            async operand =>
            {
                if (!canWriteResult)
                {
                    return;
                }

                LastResult.Set(operand);

                var expressionInfo = new ExpressionInfo()
                {
                    Expression = _inputField.Text,
                    Result = operand,
                    Mode = _angleModeButton.Mode.ToString()
                };
                SavedExpressions.Add(expressionInfo);

                await TextTransferAnimation.Animate(_outputField, _inputField);
            },
            async error =>
            {
                var toast = Toast.Make(error.Message);
                await toast.Show();
            });

    public Task<string> BackspaceButton(string inputText)
    {
        var result = inputText.RemoveBeforeCursor(_inputField.CursorPosition);
        return Task.FromResult(result.NewText);
    }

    public async Task<string> AppendText(string toAppend)
    {
        var newText = _inputField.Text
                        .InsertBeforeCursor(_inputField.CursorPosition, toAppend);

        if(toAppend != string.Empty && newText == _inputField.Text)
        {
            var toast = Toast.Make(new TooLongNumberInExpression().Message);
            await toast.Show();
        }

        return newText;
    }

    public static Task<string> ClearButton() => Task.FromResult(string.Empty);

    public Task<string> AngleModeButton(string inputText)
    {
        _angleModeButton.SwitchMode();
        return Task.FromResult(inputText);
    }

    public async Task<string> PreviousAnswerButton() =>
        await AppendText(LastResult.Get());

    public Task<string> ShiftButton(string inputText, bool swapToHYPFunctions)
    {
        _shiftButton.UpdateButtons(swapToHYPFunctions);
        return Task.FromResult(inputText);
    }

    public void UpdateInputField(string newText)
    {
        var oldPosition = _inputField.CursorPosition;
        var difference = newText.Length - _inputField.Text.Length;

        _inputField.Text = newText;
        _inputField.CursorPosition = oldPosition + difference;
    }

    public async Task UpdateInputField(string newText, string result)
    {
        UpdateInputField(await AppendText(newText));
        _outputField.Text = result;
    }

    public void SetMode(string mode) =>
        _angleModeButton.Mode = Enum.Parse<AngleMode>(mode);
}