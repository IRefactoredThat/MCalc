using System.ComponentModel;
using System.Runtime.CompilerServices;
using CalculatorApp.CalculatorPage.Page;
using CalculatorApp.SettingsPage.Backup;
using Google.Android.Material.Color.Utilities;

namespace CalculatorApp.SettingsPage.Appearance;

using Shell = Microsoft.Maui.Controls.Shell;
using Colors = Microsoft.Maui.Graphics.Colors;

public enum Theme { Auto, Light, Dark }
public enum Style { Vibrant, TonalSpot, Expressive,
    Rainbow, FruitSalad, Content, Fidelity }
public enum ColorMode { Derived, Manual }

public class AppearanceSettings(IPreferences preferences,
    SystemScheme systemScheme, AppearanceSchemeStorage schemeStorage) :
    INotifyPropertyChanged, IReload
{
    private const double ContrastLevel = 0.0;
    private const string LegacyManualColorPrefix = "ManualColor_";
    private const string LegacyManualColorsInitializedKey = "ManualColorsInitialized";
    private const string DarkManualColorPrefix = "ManualColor_Dark_";
    private const string DarkManualColorsInitializedKey = "ManualColorsDarkInitialized";
    private const string LightManualColorPrefix = "ManualColor_Light_";
    private const string LightManualColorsInitializedKey = "ManualColorsLightInitialized";
    public const double MinSurfaceChroma = 0.0;
    public const double MaxSurfaceChroma = 100.0;
    public const double DefaultSurfaceChroma = 50.0;
    public const double SurfaceChromaStep = 1.0;
    public static readonly Color FallbackSeed = Colors.Teal;
    public static readonly Enum[] ManualColorModes = [ColorMode.Manual];

    private static readonly HashSet<string> TintedRoles =
    [
        nameof(MaterialScheme.Background),
        nameof(MaterialScheme.Surface),
        nameof(MaterialScheme.SurfaceDim),
        nameof(MaterialScheme.SurfaceBright),
        nameof(MaterialScheme.SurfaceContainerLowest),
        nameof(MaterialScheme.SurfaceContainerLow),
        nameof(MaterialScheme.SurfaceContainer),
        nameof(MaterialScheme.SurfaceContainerHigh),
        nameof(MaterialScheme.SurfaceContainerHighest),
        nameof(MaterialScheme.SurfaceVariant),
        nameof(MaterialScheme.Outline),
        nameof(MaterialScheme.OutlineVariant),
    ];
    private static readonly Dictionary<string, int> AmoledColors = new()
    {
        [nameof(MaterialScheme.Background)] = 1,
        [nameof(MaterialScheme.Surface)] = 1,
        [nameof(MaterialScheme.SurfaceDim)] = 1,
        [nameof(MaterialScheme.SurfaceBright)] = 1,
        [nameof(MaterialScheme.SurfaceContainerLowest)] = 1,
        [nameof(MaterialScheme.SurfaceContainerLow)] = 3,
        [nameof(MaterialScheme.SurfaceContainer)] = 5,
        [nameof(MaterialScheme.SurfaceContainerHigh)] = 7,
        [nameof(MaterialScheme.SurfaceContainerHighest)] = 9,
    };

    private readonly List<ColorRole> _darkColorRoles = [];
    private readonly List<ColorRole> _lightColorRoles = [];
    private ResourceDictionary _resources = null!;
    private MaterialScheme _currentScheme = null!;

    private (Style Style, int Argb, bool IsDark) _seededKey;
    private bool _suspendApply;

    public IReadOnlyList<ColorRole> DarkColorRoles => _darkColorRoles;
    public IReadOnlyList<ColorRole> LightColorRoles => _lightColorRoles;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Color GetColorByName(string name) => (Color)_resources[name];

    private static bool IsDark => Application.
        Current?.RequestedTheme == AppTheme.Dark;

    public Theme PreferredTheme
    {
        get => GetTheme();
        set
        {
            preferences.Set(nameof(PreferredTheme), (int)value);
            SetTheme(value);
            OnPropertyChanged();

            OnPropertyChanged(nameof(AmoledValues));
            OnPropertyChanged(nameof(ShellColor));
        }
    }

    private static Theme GetTheme() => Application.Current!.UserAppTheme switch
    {
        AppTheme.Dark => Theme.Dark,
        AppTheme.Light => Theme.Light,
        _ => Theme.Auto,
    };

    private static void SetTheme(Theme theme)
    {
        Application.Current!.UserAppTheme = theme switch
        {
            Theme.Dark => AppTheme.Dark,
            Theme.Light => AppTheme.Light,
            _ => AppTheme.Unspecified
        };
    }

    public bool AmoledEnabled
    {
        get;
        set
        {
            preferences.Set(nameof(AmoledEnabled), value);
            field = value;

            Apply();

            OnPropertyChanged();
            OnPropertyChanged(nameof(ShellColor));
        }
    }

    public Enum[] AmoledValues => IsDark ?
        [Theme.Dark, Theme.Auto] : [Theme.Dark];

    public bool DynamicColors
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(DynamicColors), value);

            Apply();

            OnPropertyChanged();
            OnPropertyChanged(nameof(ShellColor));
        }
    }

    public Style Style
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(Style), (int)value);
            Apply();

            OnPropertyChanged();
            OnPropertyChanged(nameof(ShellColor));
        }
    }

    public ColorMode ColorMode
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            preferences.Set(nameof(ColorMode), (int)value);

            if (_suspendApply)
            {
                return;
            }

            if (value == ColorMode.Manual)
            {
                EnsureManualColorsInitialized();
            }

            Apply();
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShellColor));
        }
    }

    public Color SeedColor
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(SeedColor), value.ToInt());
            Apply();

            OnPropertyChanged();
            OnPropertyChanged(nameof(ShellColor));
        }
    } = FallbackSeed;

    public double SurfaceChroma
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(SurfaceChroma), value);
            Apply();
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShellColor));
        }
    } = DefaultSurfaceChroma;

    public Color ShellColor => Shell.Current?.CurrentPage is CalculatorGridPage
        ? (Color)_resources[nameof(MaterialScheme.SurfaceContainerHighest)]
        : (Color)_resources[nameof(MaterialScheme.Surface)];

    private void Apply()
    {
        if (_suspendApply)
        {
            return;
        }

        if (ColorMode == ColorMode.Manual)
        {
            ApplyManualColors();
            return;
        }

        var isDark = IsDark;
        var currentKey = $"{Style}{SeedColor.ToInt()}|{isDark}|{DynamicColors}|{AmoledEnabled}";

        if (schemeStorage.Cache is
                { Key: var key, RoleColors: { Count: > 0 } roleColors }
            && key == currentKey)
        {
            ApplyCachedColors(roleColors);
            return;
        }

        ApplyColors(GetDerivedColors(isDark));
        _ = Task.Run(schemeStorage.Save);
    }

    private MaterialScheme GetSeedScheme(int argb, bool isDark)
    {
        var key = (Style, argb, isDark);

        if (_seededKey != key)
        {
            _currentScheme = new MaterialScheme(
                CreateScheme(Hct.FromInt(argb)!, isDark));
            _seededKey = key;
        }

        return _currentScheme;
    }

    private Dictionary<string, int> GetDerivedColors(bool isDark)
    {
        var scheme = DynamicColors && systemScheme.ToScheme(isDark) is { } system
            ? system
            : GetSeedScheme(SeedColor.ToInt(), isDark);

        var amoled = AmoledEnabled && isDark;
        var neutral = scheme.Neutral;
        var colors = new Dictionary<string, int>(scheme.Colors.Count);

        foreach (var (name, value) in scheme.Colors)
        {
            var color = value;

            if (amoled && AmoledColors.TryGetValue(name, out var tone))
            {
                color = Tint(neutral.Tone(tone));
            }
            else if (TintedRoles.Contains(name))
            {
                color = Tint(color);
            }

            colors[name] = color;
        }

        return colors;
    }

    private void ApplyCachedColors(IReadOnlyDictionary<string, int> roleColors)
    {
        foreach (var (name, value) in roleColors)
        {
            _resources[name] = Color.FromInt(value);
        }
    }

    private void ApplyColors(IReadOnlyDictionary<string, int> colors)
    {
        foreach (var (name, value) in colors)
        {
            if (!_resources.TryGetValue(name, out var current) ||
                current is not Color color || color.ToInt() != value)
            {
                _resources[name] = Color.FromInt(value);
            }

            schemeStorage.SetColorRole(name, value);
        }
    }

    private void ApplyManualColors()
    {
        var roles = IsDark ? _darkColorRoles : _lightColorRoles;

        foreach (var role in roles)
        {
            if (!_resources.TryGetValue(role.Key, out var current) ||
                current is not Color color || !role.Color.Equals(color))
            {
                _resources[role.Key] = role.Color;
            }
        }
    }

    private int Tint(int color)
    {
        var hct = Hct.FromInt(color);
        return hct is null ? color :
            TonalPalette.FromHueAndChroma(hct.Hue, SurfaceChroma)!.Tone((int)hct.Tone);
    }

    private DynamicScheme CreateScheme(Hct source, bool isDark) => Style switch
    {
        Style.Vibrant => new SchemeVibrant(source, isDark, ContrastLevel),
        Style.Expressive => new SchemeExpressive(source, isDark, ContrastLevel),
        Style.Rainbow => new SchemeRainbow(source, isDark, ContrastLevel),
        Style.FruitSalad => new SchemeFruitSalad(source, isDark, ContrastLevel),
        Style.Content => new SchemeContent(source, isDark, ContrastLevel),
        Style.TonalSpot => new SchemeTonalSpot(source, isDark, ContrastLevel),
        Style.Fidelity => new SchemeFidelity(source, isDark, ContrastLevel),
        _ => throw new ArgumentOutOfRangeException(nameof(Style), Style, null)
    };

    private void BuildColorRoles()
    {
        BuildColorRoles(_darkColorRoles, true);
        BuildColorRoles(_lightColorRoles, false);
    }

    private void BuildColorRoles(List<ColorRole> roles, bool isDark)
    {
        if (roles.Count > 0)
        {
            return;
        }

        var definitions = ColorRoleDefinitions.All;
        void ColorChanged(ColorRole role) => OnColorRoleChanged(role, isDark);

        for (var index = 0; index < definitions.Count; index++)
        {
            var corners = index switch
            {
                0 => new CornerRadius(15, 15, 5, 5),
                _ when index == definitions.Count - 1 =>
                    new CornerRadius(5, 5, 15, 15),
                _ => new CornerRadius(5),
            };

            roles.Add(new ColorRole(definitions[index], corners,
                FallbackSeed, ColorChanged));
        }
    }

    private void EnsureManualColorsInitialized()
    {
        var wasSuspended = _suspendApply;
        _suspendApply = true;

        EnsureManualColorsInitialized(_darkColorRoles, true);
        EnsureManualColorsInitialized(_lightColorRoles, false);

        _suspendApply = wasSuspended;
    }

    private void EnsureManualColorsInitialized(List<ColorRole> roles, bool isDark)
    {
        if (preferences.Get(GetManualColorsInitializedKey(isDark), false))
        {
            return;
        }

        var fallbackColors = GetDerivedColors(isDark);

        foreach (var role in roles)
        {
            var value = fallbackColors.TryGetValue(role.Key, out var fallback)
                ? fallback
                : FallbackSeed.ToInt();

            role.Color = Color.FromInt(value);
            preferences.Set(GetManualColorKey(isDark, role.Key), value);
        }

        preferences.Set(GetManualColorsInitializedKey(isDark), true);
    }

    private void LoadColorRoles()
    {
        LoadColorRoles(_darkColorRoles, true);
        LoadColorRoles(_lightColorRoles, false);
    }

    private void LoadColorRoles(List<ColorRole> roles, bool isDark)
    {
        var fallbackColors = GetDerivedColors(isDark);

        if (isDark)
        {
            MigrateLegacyManualColors(roles, fallbackColors);
        }

        foreach (var role in roles)
        {
            var fallback = fallbackColors.TryGetValue(role.Key, out var value)
                ? value
                : FallbackSeed.ToInt();
            var stored = preferences.Get(GetManualColorKey(isDark, role.Key), fallback);

            role.Color = Color.FromInt(stored);
        }
    }

    private void MigrateLegacyManualColors(List<ColorRole> roles,
        IReadOnlyDictionary<string, int> fallbackColors)
    {
        if (preferences.Get(DarkManualColorsInitializedKey, false) ||
            !preferences.Get(LegacyManualColorsInitializedKey, false))
        {
            return;
        }

        foreach (var role in roles)
        {
            var legacyKey = LegacyManualColorPrefix + role.Key;

            if (!preferences.ContainsKey(legacyKey))
            {
                continue;
            }

            var fallback = fallbackColors.TryGetValue(role.Key, out var value)
                ? value
                : FallbackSeed.ToInt();
            preferences.Set(GetManualColorKey(true, role.Key),
                preferences.Get(legacyKey, fallback));
        }

        preferences.Set(DarkManualColorsInitializedKey, true);
    }

    private static string GetManualColorKey(bool isDark, string role) =>
        (isDark ? DarkManualColorPrefix : LightManualColorPrefix) + role;

    private static string GetManualColorsInitializedKey(bool isDark) =>
        isDark ? DarkManualColorsInitializedKey : LightManualColorsInitializedKey;

    private void OnColorRoleChanged(ColorRole role, bool isDark)
    {
        if (_suspendApply)
        {
            return;
        }

        preferences.Set(GetManualColorKey(isDark, role.Key), role.Color.ToInt());

        if (ColorMode == ColorMode.Manual && IsDark == isDark)
        {
            _resources[role.Key] = role.Color;
            OnPropertyChanged(nameof(ShellColor));
        }
    }

    public void Initialize(ResourceDictionary resourceDictionary)
    {
        _resources = resourceDictionary;
        BuildColorRoles();
        Application.Current!.RequestedThemeChanged += OnThemeChanged;
    }

    public void Reload()
    {
        _suspendApply = true;
        ColorMode = (ColorMode)preferences.Get(nameof(ColorMode), 0);
        SeedColor = Color.FromInt(preferences.Get(nameof(SeedColor),
            FallbackSeed.ToInt()));
        Style = (Style)preferences.Get(nameof(Style), 0);
        DynamicColors = preferences.Get(nameof(DynamicColors), true);
        PreferredTheme = (Theme)preferences.Get(nameof(PreferredTheme), 0);
        AmoledEnabled = preferences.Get(nameof(AmoledEnabled), false);
        SurfaceChroma = preferences.Get(nameof(SurfaceChroma), DefaultSurfaceChroma);
        BuildColorRoles();
        LoadColorRoles();

        if (ColorMode == ColorMode.Manual)
        {
            EnsureManualColorsInitialized();
        }

        _suspendApply = false;
        UpdateTheme();
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        UpdateTheme();
    }

    private void UpdateTheme()
    {
        systemScheme.UpdatePalettes();
        Apply();

        OnPropertyChanged(nameof(PreferredTheme));
        OnPropertyChanged(nameof(AmoledValues));
        OnPropertyChanged(nameof(ShellColor));
    }

    public void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        OnPropertyChanged(nameof(ShellColor));
    }
}
