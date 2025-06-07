using CalculatorApp.Utilities;

#if WINDOWS
using SharpHook;
using SharpHook.Native;
using CalculatorApp.Platforms.Windows;
#endif

namespace CalculatorApp.UIComponents;

public partial class CalculatorGrid : ContentPage, IQueryAttributable
{
    private const double PaddingAndMarginFactor = 0.05;

    private readonly Functionality _functionality;
    private readonly DeletionButtons _deletionButtons;

#if WINDOWS
    private SimpleGlobalHook? _hook;
    private ModifierMask _mask;
#endif

    public CalculatorGrid()
    {
        InitializeComponent();
        CalculatorButton.ButtonSizeChanged += SetPaddingAndMargins;

        _deletionButtons = new(buttonBackspace, buttonC);
        _functionality = new(grid, inputField, outputField, buttonShiftAngleMode);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
#if WINDOWS
        _hook = new SimpleGlobalHook
            (
            globalHookType: GlobalHookType.Keyboard,
            runAsyncOnBackgroundThread: false
            );

        _hook.KeyTyped += async (sender, e) =>
        {
            if (!WindowHelper.IsAppActive())
            {
                return;
            }

            var key = e.Data.KeyCode;

            if (key is KeyCode.VcRight && inputField.CursorPosition < inputField.Text.Length)
            {
                inputField.CursorPosition++;
            }
            else if(key is KeyCode.VcLeft && inputField.CursorPosition > 0)
            {
                inputField.CursorPosition--;
            }
            else
            {
                var buttonToClick = key switch
                {
                    KeyCode.Vc0 when _mask.HasShift() => buttonClosed,
                    KeyCode.Vc0 or KeyCode.VcNumPad0 => button0,
                    KeyCode.Vc1 when _mask.HasShift() => buttonExclamationMark,
                    KeyCode.Vc1 or KeyCode.VcNumPad1 => button1,
                    KeyCode.Vc2 when _mask.HasShift() => buttonSqrt,
                    KeyCode.Vc2 or KeyCode.VcNumPad2 => button2,
                    KeyCode.Vc3 when _mask.HasShift() => buttonCbrt,
                    KeyCode.Vc3 or KeyCode.VcNumPad3 => button3,
                    KeyCode.Vc4 or KeyCode.VcNumPad4 => button4,
                    KeyCode.Vc5 when _mask.HasShift() => buttonPercent,
                    KeyCode.Vc5 or KeyCode.VcNumPad5 => button5,
                    KeyCode.Vc6 when _mask.HasShift() => buttonCaret,
                    KeyCode.Vc6 or KeyCode.VcNumPad6 => button6,
                    KeyCode.Vc7 or KeyCode.VcNumPad7 => button7,
                    KeyCode.VcNumPadMultiply => buttonTimes,
                    KeyCode.Vc8 when _mask.HasShift() => buttonTimes,
                    KeyCode.Vc8 or KeyCode.VcNumPad8 => button8,
                    KeyCode.Vc9 when _mask.HasShift() => buttonOpen,
                    KeyCode.Vc9 or KeyCode.VcNumPad9 => button9,
                    KeyCode.VcA => buttonAns,
                    KeyCode.VcBackspace when _mask.HasShift() => buttonC,
                    KeyCode.VcBackspace => buttonBackspace,
                    KeyCode.VcC when _mask.HasFlag(ModifierMask.LeftShift) => buttonCot,
                    KeyCode.VcC when _mask.HasFlag(ModifierMask.RightShift) => buttonCsc,
                    KeyCode.VcC => buttonCos,
                    KeyCode.VcPeriod => buttonDot,
                    KeyCode.VcE => buttonE,
                    KeyCode.VcNumPadAdd => buttonPlus,
                    KeyCode.VcEquals when _mask.HasShift() => buttonPlus,
                    KeyCode.VcEquals => buttonEquals,
                    KeyCode.VcL when _mask.HasShift() => buttonLn,
                    KeyCode.VcL => buttonLog,
                    KeyCode.VcMinus or KeyCode.VcNumPadSubtract => buttonMinus,
                    KeyCode.VcSlash or KeyCode.VcNumPadDivide => buttonSlash,
                    KeyCode.VcP => buttonPi,
                    KeyCode.VcS when _mask.HasShift() => buttonSec,
                    KeyCode.VcS => buttonSin,
                    KeyCode.VcT => buttonTan,
                    KeyCode.VcTab => buttonShiftAngleMode,
                    KeyCode.VcLeftControl => buttonShiftOp,
                    KeyCode.VcLeftAlt => buttonShiftTrigOp,
                    _ => null,
                };

                if (buttonToClick != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        OnButtonClicked(buttonToClick, EventArgs.Empty));
                }
            }
        };

        _hook.KeyPressed += (sender, e) =>
        {
            _mask |= e.RawEvent.Mask;
        };

        _hook.KeyReleased += (sender, e) =>
        {
            if(e.Data.KeyCode == KeyCode.VcLeftShift)
            {
                _mask &= ~ModifierMask.LeftShift;
            }
            if(e.Data.KeyCode == KeyCode.VcRightShift)
            {
                _mask &= ~ModifierMask.RightShift;
            }
        };

        _ = _hook.RunAsync();
#endif
    }

    protected override void OnDisappearing()
    {
#if WINDOWS
        _hook?.Dispose();
#endif
    }

    private void SetPaddingAndMargins(double buttonWidth, double buttonHeight)
    {
        grid.RowSpacing = buttonHeight * PaddingAndMarginFactor;
        grid.ColumnSpacing = buttonWidth * PaddingAndMarginFactor;
        grid.Margin = Math.Min(grid.Width, grid.Height) * PaddingAndMarginFactor;

        CalculatorButton.ButtonSizeChanged -= SetPaddingAndMargins;
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }
        await MapInput(button.Text);
    }

    private async Task MapInput(string input)
    {
        if (input == "=")
        {
            await _functionality.EqualButton(outputField.Text != string.Empty);
            return;
        }

        var newText = await(input switch
        {
            "⌫" => _functionality.BackspaceButton(inputField.Text),
            "C" => Functionality.ClearButton(),
            "RAD" or "DEG" => _functionality.AngleModeButton(inputField.Text),
            "ANS" => _functionality.PreviousAnswerButton(),
            "1/2" or "2/2" or "CIR" or "HYP" => _functionality
                .ShiftButton(inputField.Text, input is "CIR" or "HYP"),
            _ => _functionality.AppendText(input)
        });

        var reevaluatedText = _functionality.Evaluate(newText);
        _functionality.UpdateInputField(reevaluatedText);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.ContainsKey("expression"))
        {
            return;
        }

        var castedQuery = query.ToDictionary(keyValue => keyValue.Key,
            keyValue => (keyValue.Value as string) ?? string.Empty);
        await _functionality.UpdateInputField(
            castedQuery["expression"], castedQuery["result"]);
        _functionality.SetMode(castedQuery["mode"]);

        query.Clear();
    }
}