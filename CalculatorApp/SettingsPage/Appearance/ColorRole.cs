using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.Appearance;

public readonly record struct ColorRoleDefinition(string Name, string Description);

public sealed class ColorRole(
    ColorRoleDefinition definition,
    CornerRadius corners,
    Color color,
    Action<ColorRole> colorChanged)
    : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Name { get; } = definition.Name;
    public string Description { get; } = definition.Description;
    public CornerRadius Corners { get; } = corners;

    public Color Color
    {
        get;
        set
        {
            if (field.Equals(value))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
            colorChanged(this);
        }
    } = color;

    public static IEnumerable<ColorRoleDefinition> GetAllDefinitions()
    {
        yield return new ColorRoleDefinition("Primary", "Main accent color");
        yield return new ColorRoleDefinition("On primary", "Content shown on primary");
        yield return new ColorRoleDefinition("Primary container", "Prominent primary surfaces");
        yield return new ColorRoleDefinition("On primary container", "Content shown on primary container");
        yield return new ColorRoleDefinition("Secondary", "Supporting accent color");
        yield return new ColorRoleDefinition("On secondary", "Content shown on secondary");
        yield return new ColorRoleDefinition("Secondary container", "Prominent secondary surfaces");
        yield return new ColorRoleDefinition("On secondary container", "Content shown on secondary container");
        yield return new ColorRoleDefinition("Tertiary", "Contrasting accent color");
        yield return new ColorRoleDefinition("On tertiary", "Content shown on tertiary");
        yield return new ColorRoleDefinition("Tertiary container", "Prominent tertiary surfaces");
        yield return new ColorRoleDefinition("On tertiary container", "Content shown on tertiary container");
        yield return new ColorRoleDefinition("Error", "Error and destructive actions");
        yield return new ColorRoleDefinition("On error", "Content shown on error");
        yield return new ColorRoleDefinition("Error container", "Prominent error surfaces");
        yield return new ColorRoleDefinition("On error container", "Content shown on error container");
        yield return new ColorRoleDefinition("Background", "App background");
        yield return new ColorRoleDefinition("On background", "Content shown on background");
        yield return new ColorRoleDefinition("Surface", "Default surface color");
        yield return new ColorRoleDefinition("On surface", "Content shown on surface");
        yield return new ColorRoleDefinition("Surface variant", "Alternative surface color");
        yield return new ColorRoleDefinition("On surface variant", "Content shown on surface variant");
        yield return new ColorRoleDefinition("Outline", "Borders and separators");
        yield return new ColorRoleDefinition("Shadow", "Drop shadow color");
        yield return new ColorRoleDefinition("Inverse surface", "High-contrast inverse surface");
        yield return new ColorRoleDefinition("Inverse on surface", "Content shown on inverse surface");
        yield return new ColorRoleDefinition("Inverse primary", "Primary color on inverse surface");
        yield return new ColorRoleDefinition("Surface dim", "Dimmed surface color");
        yield return new ColorRoleDefinition("Surface bright", "Brightened surface color");
        yield return new ColorRoleDefinition("Surface container lowest", "Lowest-emphasis container");
        yield return new ColorRoleDefinition("Surface container low", "Low-emphasis container");
        yield return new ColorRoleDefinition("Surface container", "Standard container");
        yield return new ColorRoleDefinition("Surface container high", "High-emphasis container");
        yield return new ColorRoleDefinition("Surface container highest", "Highest-emphasis container");
        yield return new ColorRoleDefinition("Outline variant", "Subtle borders and separators");
    }
}
