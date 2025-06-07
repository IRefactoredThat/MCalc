using Essentials.Calculator;

namespace CalculatorApp.Utilities;

class AngleModeButton(Button angleModeButton)
{
    private readonly Button _angleModeButton = angleModeButton;

    public AngleMode Mode
    {
        get => Enum.Parse<AngleMode>(_angleModeButton.Text);
        set => _angleModeButton.Text = value.ToString();
    }

    public void SwitchMode() => Mode = _angleModeButton.Text == 
        AngleMode.RAD.ToString() ? AngleMode.DEG : AngleMode.RAD;
}