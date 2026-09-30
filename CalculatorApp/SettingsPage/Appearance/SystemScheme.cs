using Google.Android.Material.Color;
using Google.Android.Material.Color.Utilities;

namespace CalculatorApp.SettingsPage.Appearance;

public sealed class SystemScheme
{
    private const double ErrorHue = 25.0;
    private const double ErrorChroma = 84.0;
    private static readonly TonalPalette ErrorPalette =
        TonalPalette.FromHueAndChroma(ErrorHue, ErrorChroma)!;

    private TonalPalette _primary = null!;
    private TonalPalette _secondary = null!;
    private TonalPalette _tertiary = null!;
    private TonalPalette _neutral = null!;
    private TonalPalette _neutralVariant = null!;

    private MaterialScheme? _light;
    private MaterialScheme? _dark;

    private bool _isAvailable;

    public SystemScheme() => UpdatePalettes();

    public void UpdatePalettes()
    {
        _light = _dark = null;

        var resources = Android.App.Application.Context.Resources!;

        _isAvailable = DynamicColors.IsDynamicColorAvailable
            && GetPalette(resources, "system_accent1_500", out _primary)
            && GetPalette(resources, "system_accent2_500", out _secondary)
            && GetPalette(resources, "system_accent3_500", out _tertiary)
            && GetPalette(resources, "system_neutral1_500", out _neutral)
            && GetPalette(resources, "system_neutral2_500", out _neutralVariant);
    }

    public MaterialScheme? ToScheme(bool isDark) => !_isAvailable ? null
        : isDark ? _dark ??= new MaterialScheme(Colors(true), _neutral)
        : _light ??= new MaterialScheme(Colors(false), _neutral);

    private static bool GetPalette(Android.Content.Res.Resources resources,
        string name, out TonalPalette palette)
    {
        var id = resources.GetIdentifier(name, "color", "android");

        if (id == 0)
        {
            palette = null!;
            return false;
        }

        palette = TonalPalette.FromInt(resources.GetColor(id, null).ToArgb())!;
        return true;
    }

    private static Color Tone(TonalPalette palette, int tone) =>
        MaterialScheme.ToColor(palette.Tone(tone));

    private static Color Error(int tone) => Tone(ErrorPalette, tone);

    private Dictionary<string, Color> Colors(bool isDark) => new()
    {
        [nameof(MaterialScheme.Primary)] = Tone(_primary, isDark ? 80 : 40),
        [nameof(MaterialScheme.OnPrimary)] = Tone(_primary, isDark ? 20 : 100),
        [nameof(MaterialScheme.PrimaryContainer)] = Tone(_primary, isDark ? 30 : 90),
        [nameof(MaterialScheme.OnPrimaryContainer)] = Tone(_primary, isDark ? 90 : 10),
        [nameof(MaterialScheme.Secondary)] = Tone(_secondary, isDark ? 80 : 40),
        [nameof(MaterialScheme.OnSecondary)] = Tone(_secondary, isDark ? 20 : 100),
        [nameof(MaterialScheme.SecondaryContainer)] = Tone(_secondary, isDark ? 30 : 90),
        [nameof(MaterialScheme.OnSecondaryContainer)] = Tone(_secondary, isDark ? 90 : 10),
        [nameof(MaterialScheme.Tertiary)] = Tone(_tertiary, isDark ? 80 : 40),
        [nameof(MaterialScheme.OnTertiary)] = Tone(_tertiary, isDark ? 20 : 100),
        [nameof(MaterialScheme.TertiaryContainer)] = Tone(_tertiary, isDark ? 30 : 90),
        [nameof(MaterialScheme.OnTertiaryContainer)] = Tone(_tertiary, isDark ? 90 : 10),
        [nameof(MaterialScheme.Error)] = Error(isDark ? 80 : 40),
        [nameof(MaterialScheme.OnError)] = Error(isDark ? 20 : 100),
        [nameof(MaterialScheme.ErrorContainer)] = Error(isDark ? 30 : 90),
        [nameof(MaterialScheme.OnErrorContainer)] = Error(isDark ? 90 : 10),
        [nameof(MaterialScheme.Background)] = Tone(_neutralVariant, isDark ? 6 : 98),
        [nameof(MaterialScheme.OnBackground)] = Tone(_neutral , isDark ? 90 : 10),
        [nameof(MaterialScheme.Surface)] = Tone(_neutralVariant, isDark ? 6 : 98),
        [nameof(MaterialScheme.OnSurface)] = Tone(_neutral, isDark ? 90 : 10),
        [nameof(MaterialScheme.SurfaceVariant)] = Tone(_neutralVariant, isDark ? 30 : 90),
        [nameof(MaterialScheme.OnSurfaceVariant)] = Tone(_neutralVariant, isDark ? 80 : 30),
        [nameof(MaterialScheme.Outline)] = Tone(_neutralVariant, isDark ? 60 : 50),
        [nameof(MaterialScheme.OutlineVariant)] = Tone(_neutralVariant, isDark ? 30 : 80),
        [nameof(MaterialScheme.Shadow)] = Tone(_neutral, 0),
        [nameof(MaterialScheme.InverseSurface)] = Tone(_neutral, isDark ? 90 : 20),
        [nameof(MaterialScheme.InverseOnSurface)] = Tone(isDark ? _neutral : _neutralVariant, isDark ? 20 : 95),
        [nameof(MaterialScheme.InversePrimary)] = Tone(_primary, isDark ? 40 : 80),
        [nameof(MaterialScheme.SurfaceDim)] = Tone(_neutralVariant, isDark ? 6 : 87),
        [nameof(MaterialScheme.SurfaceBright)] = Tone(_neutralVariant, isDark ? 24 : 98),
        [nameof(MaterialScheme.SurfaceContainerLowest)] = Tone(isDark ? _neutral : _neutralVariant, isDark ? 4 : 100),
        [nameof(MaterialScheme.SurfaceContainerLow)] = Tone(isDark ? _neutral : _neutralVariant, isDark ? 10 : 96),
        [nameof(MaterialScheme.SurfaceContainer)] = Tone(isDark ? _neutral : _neutralVariant, isDark ? 12 : 94),
        [nameof(MaterialScheme.SurfaceContainerHigh)] = Tone(isDark ? _neutral : _neutralVariant, isDark ? 17 : 92),
        [nameof(MaterialScheme.SurfaceContainerHighest)] = Tone(isDark ? _neutral : _neutralVariant, isDark ? 22 : 90),
    };
}
