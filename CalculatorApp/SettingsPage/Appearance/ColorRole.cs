using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.Appearance;

public readonly record struct ColorRoleDefinition(string Key, string Name, string Description);

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

    public string Key { get; } = definition.Key;
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
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Primary), "Primary", "Main accent color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnPrimary), "On primary", "Content shown on primary");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.PrimaryContainer), "Primary container", "Prominent primary surfaces");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnPrimaryContainer), "On primary container", "Content shown on primary container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Secondary), "Secondary", "Supporting accent color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnSecondary), "On secondary", "Content shown on secondary");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SecondaryContainer), "Secondary container", "Prominent secondary surfaces");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnSecondaryContainer), "On secondary container", "Content shown on secondary container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Tertiary), "Tertiary", "Contrasting accent color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnTertiary), "On tertiary", "Content shown on tertiary");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.TertiaryContainer), "Tertiary container", "Prominent tertiary surfaces");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnTertiaryContainer), "On tertiary container", "Content shown on tertiary container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Error), "Error", "Error and destructive actions");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnError), "On error", "Content shown on error");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.ErrorContainer), "Error container", "Prominent error surfaces");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnErrorContainer), "On error container", "Content shown on error container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Background), "Background", "App background");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnBackground), "On background", "Content shown on background");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Surface), "Surface", "Default surface color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnSurface), "On surface", "Content shown on surface");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceVariant), "Surface variant", "Alternative surface color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OnSurfaceVariant), "On surface variant", "Content shown on surface variant");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Outline), "Outline", "Borders and separators");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.Shadow), "Shadow", "Drop shadow color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.InverseSurface), "Inverse surface", "High-contrast inverse surface");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.InverseOnSurface), "Inverse on surface", "Content shown on inverse surface");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.InversePrimary), "Inverse primary", "Primary color on inverse surface");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceDim), "Surface dim", "Dimmed surface color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceBright), "Surface bright", "Brightened surface color");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceContainerLowest), "Surface container lowest", "Lowest-emphasis container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceContainerLow), "Surface container low", "Low-emphasis container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceContainer), "Surface container", "Standard container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceContainerHigh), "Surface container high", "High-emphasis container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.SurfaceContainerHighest), "Surface container highest", "Highest-emphasis container");
        yield return new ColorRoleDefinition(nameof(MaterialScheme.OutlineVariant), "Outline variant", "Subtle borders and separators");
    }
}
