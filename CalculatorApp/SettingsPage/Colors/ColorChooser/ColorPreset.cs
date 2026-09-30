using System.ComponentModel;

namespace CalculatorApp.SettingsPage.Colors.ColorChooser;

public class ColorPreset(Color color) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsSelected
    {
        get;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public Color Color => color;
}