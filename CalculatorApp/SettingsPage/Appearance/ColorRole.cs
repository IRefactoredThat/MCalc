using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.Appearance;

public readonly record struct ColorRoleDefinition(string Key, string Description);

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
    public string Name { get; } = Humanize(definition.Key);
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

    private static string Humanize(string key)
    {
        var name = new char[key.Length + 4];
        var length = 0;

        for (var index = 0; index < key.Length; index++)
        {
            var character = key[index];

            if (index > 0 && char.IsUpper(character))
            {
                name[length++] = ' ';
                character = char.ToLowerInvariant(character);
            }
            else if (index == 0)
            {
                character = char.ToUpperInvariant(character);
            }

            name[length++] = character;
        }

        return new string(name, 0, length);
    }
}

public static class ColorRoleDefinitions
{
    public static readonly IReadOnlyList<ColorRoleDefinition> All =
    [
        new(nameof(MaterialScheme.Primary), "Main accent color"),
        new(nameof(MaterialScheme.OnPrimary), "Content shown on primary"),
        new(nameof(MaterialScheme.PrimaryContainer), "Prominent primary surfaces"),
        new(nameof(MaterialScheme.OnPrimaryContainer), "Content shown on primary container"),
        new(nameof(MaterialScheme.Secondary), "Supporting accent color"),
        new(nameof(MaterialScheme.OnSecondary), "Content shown on secondary"),
        new(nameof(MaterialScheme.SecondaryContainer), "Prominent secondary surfaces"),
        new(nameof(MaterialScheme.OnSecondaryContainer), "Content shown on secondary container"),
        new(nameof(MaterialScheme.Tertiary), "Contrasting accent color"),
        new(nameof(MaterialScheme.OnTertiary), "Content shown on tertiary"),
        new(nameof(MaterialScheme.TertiaryContainer), "Prominent tertiary surfaces"),
        new(nameof(MaterialScheme.OnTertiaryContainer), "Content shown on tertiary container"),
        new(nameof(MaterialScheme.Error), "Error and destructive actions"),
        new(nameof(MaterialScheme.OnError), "Content shown on error"),
        new(nameof(MaterialScheme.ErrorContainer), "Prominent error surfaces"),
        new(nameof(MaterialScheme.OnErrorContainer), "Content shown on error container"),
        new(nameof(MaterialScheme.Background), "App background"),
        new(nameof(MaterialScheme.OnBackground), "Content shown on background"),
        new(nameof(MaterialScheme.Surface), "Default surface color"),
        new(nameof(MaterialScheme.OnSurface), "Content shown on surface"),
        new(nameof(MaterialScheme.SurfaceVariant), "Alternative surface color"),
        new(nameof(MaterialScheme.OnSurfaceVariant), "Content shown on surface variant"),
        new(nameof(MaterialScheme.Outline), "Borders and separators"),
        new(nameof(MaterialScheme.Shadow), "Drop shadow color"),
        new(nameof(MaterialScheme.InverseSurface), "High-contrast inverse surface"),
        new(nameof(MaterialScheme.InverseOnSurface), "Content shown on inverse surface"),
        new(nameof(MaterialScheme.InversePrimary), "Primary color on inverse surface"),
        new(nameof(MaterialScheme.SurfaceDim), "Dimmed surface color"),
        new(nameof(MaterialScheme.SurfaceBright), "Brightened surface color"),
        new(nameof(MaterialScheme.SurfaceContainerLowest), "Lowest-emphasis container"),
        new(nameof(MaterialScheme.SurfaceContainerLow), "Low-emphasis container"),
        new(nameof(MaterialScheme.SurfaceContainer), "Standard container"),
        new(nameof(MaterialScheme.SurfaceContainerHigh), "High-emphasis container"),
        new(nameof(MaterialScheme.SurfaceContainerHighest), "Highest-emphasis container"),
        new(nameof(MaterialScheme.OutlineVariant), "Subtle borders and separators"),
    ];
}
