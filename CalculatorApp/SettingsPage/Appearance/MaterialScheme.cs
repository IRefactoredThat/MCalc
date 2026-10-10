using Google.Android.Material.Color.Utilities;

namespace CalculatorApp.SettingsPage.Appearance;

public sealed class MaterialScheme(Dictionary<string, int> colors,
    TonalPalette neutral)
{
    public TonalPalette Neutral { get; } = neutral;
    public MaterialScheme(DynamicScheme scheme)
        : this(Roles(scheme), scheme.NeutralPalette!) { }

    private static Dictionary<string, int> Roles(DynamicScheme scheme) => new()
    {
        [nameof(Primary)] = scheme.Primary,
        [nameof(OnPrimary)] = scheme.OnPrimary,
        [nameof(PrimaryContainer)] = scheme.PrimaryContainer,
        [nameof(OnPrimaryContainer)] = scheme.OnPrimaryContainer,
        [nameof(Secondary)] = scheme.Secondary,
        [nameof(OnSecondary)] = scheme.OnSecondary,
        [nameof(SecondaryContainer)] = scheme.SecondaryContainer,
        [nameof(OnSecondaryContainer)] = scheme.OnSecondaryContainer,
        [nameof(Tertiary)] = scheme.Tertiary,
        [nameof(OnTertiary)] = scheme.OnTertiary,
        [nameof(TertiaryContainer)] = scheme.TertiaryContainer,
        [nameof(OnTertiaryContainer)] = scheme.OnTertiaryContainer,
        [nameof(Error)] = scheme.Error,
        [nameof(OnError)] = scheme.OnError,
        [nameof(ErrorContainer)] = scheme.ErrorContainer,
        [nameof(OnErrorContainer)] = scheme.OnErrorContainer,
        [nameof(Background)] = scheme.Background,
        [nameof(OnBackground)] = scheme.OnBackground,
        [nameof(Surface)] = scheme.Surface,
        [nameof(OnSurface)] = scheme.OnSurface,
        [nameof(SurfaceVariant)] = scheme.SurfaceVariant,
        [nameof(OnSurfaceVariant)] = scheme.OnSurfaceVariant,
        [nameof(Outline)] = scheme.Outline,
        [nameof(Shadow)] = scheme.Shadow,
        [nameof(InverseSurface)] = scheme.InverseSurface,
        [nameof(InverseOnSurface)] = scheme.InverseOnSurface,
        [nameof(InversePrimary)] = scheme.InversePrimary,
        [nameof(SurfaceDim)] = scheme.SurfaceDim,
        [nameof(SurfaceBright)] = scheme.SurfaceBright,
        [nameof(SurfaceContainerLowest)] = scheme.SurfaceContainerLowest,
        [nameof(SurfaceContainerLow)] = scheme.SurfaceContainerLow,
        [nameof(SurfaceContainer)] = scheme.SurfaceContainer,
        [nameof(SurfaceContainerHigh)] = scheme.SurfaceContainerHigh,
        [nameof(SurfaceContainerHighest)] = scheme.SurfaceContainerHighest,
        [nameof(OutlineVariant)] = scheme.OutlineVariant,
    };

    public Color Primary => Color.FromInt(colors[nameof(Primary)]);
    public Color OnPrimary => Color.FromInt(colors[nameof(OnPrimary)]);
    public Color PrimaryContainer => Color.FromInt(colors[nameof(PrimaryContainer)]);
    public Color OnPrimaryContainer => Color.FromInt(colors[nameof(OnPrimaryContainer)]);
    public Color Secondary => Color.FromInt(colors[nameof(Secondary)]);
    public Color OnSecondary => Color.FromInt(colors[nameof(OnSecondary)]);
    public Color SecondaryContainer => Color.FromInt(colors[nameof(SecondaryContainer)]);
    public Color OnSecondaryContainer => Color.FromInt(colors[nameof(OnSecondaryContainer)]);
    public Color Tertiary => Color.FromInt(colors[nameof(Tertiary)]);
    public Color OnTertiary => Color.FromInt(colors[nameof(OnTertiary)]);
    public Color TertiaryContainer => Color.FromInt(colors[nameof(TertiaryContainer)]);
    public Color OnTertiaryContainer => Color.FromInt(colors[nameof(OnTertiaryContainer)]);
    public Color Error => Color.FromInt(colors[nameof(Error)]);
    public Color OnError => Color.FromInt(colors[nameof(OnError)]);
    public Color ErrorContainer => Color.FromInt(colors[nameof(ErrorContainer)]);
    public Color OnErrorContainer => Color.FromInt(colors[nameof(OnErrorContainer)]);
    public Color Background => Color.FromInt(colors[nameof(Background)]);
    public Color OnBackground => Color.FromInt(colors[nameof(OnBackground)]);
    public Color Surface => Color.FromInt(colors[nameof(Surface)]);
    public Color OnSurface => Color.FromInt(colors[nameof(OnSurface)]);
    public Color SurfaceVariant => Color.FromInt(colors[nameof(SurfaceVariant)]);
    public Color OnSurfaceVariant => Color.FromInt(colors[nameof(OnSurfaceVariant)]);
    public Color Outline => Color.FromInt(colors[nameof(Outline)]);
    public Color Shadow => Color.FromInt(colors[nameof(Shadow)]);
    public Color InverseSurface => Color.FromInt(colors[nameof(InverseSurface)]);
    public Color InverseOnSurface => Color.FromInt(colors[nameof(InverseOnSurface)]);
    public Color InversePrimary => Color.FromInt(colors[nameof(InversePrimary)]);
    public Color SurfaceDim => Color.FromInt(colors[nameof(SurfaceDim)]);
    public Color SurfaceBright => Color.FromInt(colors[nameof(SurfaceBright)]);
    public Color SurfaceContainerLowest => Color.FromInt(colors[nameof(SurfaceContainerLowest)]);
    public Color SurfaceContainerLow => Color.FromInt(colors[nameof(SurfaceContainerLow)]);
    public Color SurfaceContainer => Color.FromInt(colors[nameof(SurfaceContainer)]);
    public Color SurfaceContainerHigh => Color.FromInt(colors[nameof(SurfaceContainerHigh)]);
    public Color SurfaceContainerHighest => Color.FromInt(colors[nameof(SurfaceContainerHighest)]);
    public Color OutlineVariant => Color.FromInt(colors[nameof(OutlineVariant)]);

    public IReadOnlyDictionary<string, int> Colors => colors;
}