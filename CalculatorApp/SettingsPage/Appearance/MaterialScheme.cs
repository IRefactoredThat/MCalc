using Google.Android.Material.Color.Utilities;

namespace CalculatorApp.SettingsPage.Appearance;

public sealed class MaterialScheme(Dictionary<string, Color> colors,
    TonalPalette neutral)
{
    public TonalPalette Neutral { get; } = neutral;
    public MaterialScheme(DynamicScheme scheme)
        : this(Roles(scheme), scheme.NeutralPalette!) { }

    private static Dictionary<string, Color> Roles(DynamicScheme scheme) => new()
    {
        [nameof(Primary)] = ToColor(scheme.Primary),
        [nameof(OnPrimary)] = ToColor(scheme.OnPrimary),
        [nameof(PrimaryContainer)] = ToColor(scheme.PrimaryContainer),
        [nameof(OnPrimaryContainer)] = ToColor(scheme.OnPrimaryContainer),
        [nameof(Secondary)] = ToColor(scheme.Secondary),
        [nameof(OnSecondary)] = ToColor(scheme.OnSecondary),
        [nameof(SecondaryContainer)] = ToColor(scheme.SecondaryContainer),
        [nameof(OnSecondaryContainer)] = ToColor(scheme.OnSecondaryContainer),
        [nameof(Tertiary)] = ToColor(scheme.Tertiary),
        [nameof(OnTertiary)] = ToColor(scheme.OnTertiary),
        [nameof(TertiaryContainer)] = ToColor(scheme.TertiaryContainer),
        [nameof(OnTertiaryContainer)] = ToColor(scheme.OnTertiaryContainer),
        [nameof(Error)] = ToColor(scheme.Error),
        [nameof(OnError)] = ToColor(scheme.OnError),
        [nameof(ErrorContainer)] = ToColor(scheme.ErrorContainer),
        [nameof(OnErrorContainer)] = ToColor(scheme.OnErrorContainer),
        [nameof(Background)] = ToColor(scheme.Background),
        [nameof(OnBackground)] = ToColor(scheme.OnBackground),
        [nameof(Surface)] = ToColor(scheme.Surface),
        [nameof(OnSurface)] = ToColor(scheme.OnSurface),
        [nameof(SurfaceVariant)] = ToColor(scheme.SurfaceVariant),
        [nameof(OnSurfaceVariant)] = ToColor(scheme.OnSurfaceVariant),
        [nameof(Outline)] = ToColor(scheme.Outline),
        [nameof(Shadow)] = ToColor(scheme.Shadow),
        [nameof(InverseSurface)] = ToColor(scheme.InverseSurface),
        [nameof(InverseOnSurface)] = ToColor(scheme.InverseOnSurface),
        [nameof(InversePrimary)] = ToColor(scheme.InversePrimary),
        [nameof(SurfaceDim)] = ToColor(scheme.SurfaceDim),
        [nameof(SurfaceBright)] = ToColor(scheme.SurfaceBright),
        [nameof(SurfaceContainerLowest)] = ToColor(scheme.SurfaceContainerLowest),
        [nameof(SurfaceContainerLow)] = ToColor(scheme.SurfaceContainerLow),
        [nameof(SurfaceContainer)] = ToColor(scheme.SurfaceContainer),
        [nameof(SurfaceContainerHigh)] = ToColor(scheme.SurfaceContainerHigh),
        [nameof(SurfaceContainerHighest)] = ToColor(scheme.SurfaceContainerHighest),
        [nameof(OutlineVariant)] = ToColor(scheme.OutlineVariant),
    };

    public Color Primary => colors[nameof(Primary)];
    public Color OnPrimary => colors[nameof(OnPrimary)];
    public Color PrimaryContainer => colors[nameof(PrimaryContainer)];
    public Color OnPrimaryContainer => colors[nameof(OnPrimaryContainer)];
    public Color Secondary => colors[nameof(Secondary)];
    public Color OnSecondary => colors[nameof(OnSecondary)];
    public Color SecondaryContainer => colors[nameof(SecondaryContainer)];
    public Color OnSecondaryContainer => colors[nameof(OnSecondaryContainer)];
    public Color Tertiary => colors[nameof(Tertiary)];
    public Color OnTertiary => colors[nameof(OnTertiary)];
    public Color TertiaryContainer => colors[nameof(TertiaryContainer)];
    public Color OnTertiaryContainer => colors[nameof(OnTertiaryContainer)];
    public Color Error => colors[nameof(Error)];
    public Color OnError => colors[nameof(OnError)];
    public Color ErrorContainer => colors[nameof(ErrorContainer)];
    public Color OnErrorContainer => colors[nameof(OnErrorContainer)];
    public Color Background => colors[nameof(Background)];
    public Color OnBackground => colors[nameof(OnBackground)];
    public Color Surface => colors[nameof(Surface)];
    public Color OnSurface => colors[nameof(OnSurface)];
    public Color SurfaceVariant => colors[nameof(SurfaceVariant)];
    public Color OnSurfaceVariant => colors[nameof(OnSurfaceVariant)];
    public Color Outline => colors[nameof(Outline)];
    public Color Shadow => colors[nameof(Shadow)];
    public Color InverseSurface => colors[nameof(InverseSurface)];
    public Color InverseOnSurface => colors[nameof(InverseOnSurface)];
    public Color InversePrimary => colors[nameof(InversePrimary)];
    public Color SurfaceDim => colors[nameof(SurfaceDim)];
    public Color SurfaceBright => colors[nameof(SurfaceBright)];
    public Color SurfaceContainerLowest => colors[nameof(SurfaceContainerLowest)];
    public Color SurfaceContainerLow => colors[nameof(SurfaceContainerLow)];
    public Color SurfaceContainer => colors[nameof(SurfaceContainer)];
    public Color SurfaceContainerHigh => colors[nameof(SurfaceContainerHigh)];
    public Color SurfaceContainerHighest => colors[nameof(SurfaceContainerHighest)];
    public Color OutlineVariant => colors[nameof(OutlineVariant)];

    public IEnumerable<(string Name, Color Color)> Enumerate() =>
    [
        (nameof(Primary), Primary),
        (nameof(OnPrimary), OnPrimary),
        (nameof(PrimaryContainer), PrimaryContainer),
        (nameof(OnPrimaryContainer), OnPrimaryContainer),
        (nameof(Secondary), Secondary),
        (nameof(OnSecondary), OnSecondary),
        (nameof(SecondaryContainer), SecondaryContainer),
        (nameof(OnSecondaryContainer), OnSecondaryContainer),
        (nameof(Tertiary), Tertiary),
        (nameof(OnTertiary), OnTertiary),
        (nameof(TertiaryContainer), TertiaryContainer),
        (nameof(OnTertiaryContainer), OnTertiaryContainer),
        (nameof(Error), Error),
        (nameof(OnError), OnError),
        (nameof(ErrorContainer), ErrorContainer),
        (nameof(OnErrorContainer), OnErrorContainer),
        (nameof(Background), Background),
        (nameof(OnBackground), OnBackground),
        (nameof(Surface), Surface),
        (nameof(OnSurface), OnSurface),
        (nameof(SurfaceVariant), SurfaceVariant),
        (nameof(OnSurfaceVariant), OnSurfaceVariant),
        (nameof(Outline), Outline),
        (nameof(Shadow), Shadow),
        (nameof(InverseSurface), InverseSurface),
        (nameof(InverseOnSurface), InverseOnSurface),
        (nameof(InversePrimary), InversePrimary),
        (nameof(SurfaceDim), SurfaceDim),
        (nameof(SurfaceBright), SurfaceBright),
        (nameof(SurfaceContainerLowest), SurfaceContainerLowest),
        (nameof(SurfaceContainerLow), SurfaceContainerLow),
        (nameof(SurfaceContainer), SurfaceContainer),
        (nameof(SurfaceContainerHigh), SurfaceContainerHigh),
        (nameof(SurfaceContainerHighest), SurfaceContainerHighest),
        (nameof(OutlineVariant), OutlineVariant),
    ];

    public static Color ToColor(int argb) => Color.FromInt(argb);
}
