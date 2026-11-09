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
    private const string DarkColorPresetKey = "ColorPreset_Dark";
    private const string LightColorPresetKey = "ColorPreset_Light";
    private const string ManualColorsAmoledRepairKey = "ManualColorsAmoledRepair";
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
    private bool _applyingPreset;

    public IReadOnlyList<ColorRole> DarkColorRoles => _darkColorRoles;
    public IReadOnlyList<ColorRole> LightColorRoles => _lightColorRoles;

    public ColorPreset DarkColorPreset
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnColorPresetChanged(true, value);
        }
    }

    public ColorPreset LightColorPreset
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnColorPresetChanged(false, value);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void ApplyAndNotify([CallerMemberName] string? propertyName = null)
    {
        Apply();
        OnPropertyChanged(propertyName);
        OnPropertyChanged(nameof(ShellColor));
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

    private static void SetTheme(Theme theme) =>
        Application.Current!.UserAppTheme = theme switch
        {
            Theme.Dark => AppTheme.Dark,
            Theme.Light => AppTheme.Light,
            _ => AppTheme.Unspecified
        };

    public bool AmoledEnabled
    {
        get;
        set
        {
            preferences.Set(nameof(AmoledEnabled), value);
            field = value;
            ApplyAndNotify();
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
            ApplyAndNotify();
        }
    }

    public Style Style
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(Style), (int)value);
            ApplyAndNotify();
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
                LoadColorPresets();
            }

            ApplyAndNotify();
        }
    }

    public Color SeedColor
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(SeedColor), value.ToInt());
            ApplyAndNotify();
        }
    } = FallbackSeed;

    public double SurfaceChroma
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(SurfaceChroma), value);
            ApplyAndNotify();
        }
    } = DefaultSurfaceChroma;

    public Color ShellColor => GetColorByName(
        Shell.Current?.CurrentPage is CalculatorGridPage
            ? nameof(MaterialScheme.SurfaceContainerHighest)
            : nameof(MaterialScheme.Surface));

    private List<ColorRole> GetRoles(bool isDark) =>
        isDark ? _darkColorRoles : _lightColorRoles;

    private static void ForBothThemes(Action<bool> action)
    {
        action(true);
        action(false);
    }

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
            foreach (var (name, value) in roleColors)
            {
                SetResource(name, Color.FromInt(value));
            }

            return;
        }

        foreach (var (name, value) in GetDerivedColors(isDark))
        {
            SetResource(name, Color.FromInt(value));
            schemeStorage.SetColorRole(name, value);
        }

        _ = Task.Run(schemeStorage.Save);
    }

    private void SetResource(string name, Color color)
    {
        if (_resources.TryGetValue(name, out var current) &&
            current is Color existing && existing.Equals(color))
        {
            return;
        }

        _resources[name] = color;
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

    private Dictionary<string, int> GetDerivedColors(bool isDark,
        bool? applyAmoled = null)
    {
        var scheme = DynamicColors && systemScheme.ToScheme(isDark) is { } system
            ? system
            : GetSeedScheme(SeedColor.ToInt(), isDark);

        var amoled = (applyAmoled ?? AmoledEnabled) && isDark;
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

    private void ApplyManualColors()
    {
        foreach (var role in GetRoles(IsDark))
        {
            SetResource(role.Key, GetManualColor(role));
        }
    }

    private Color GetManualColor(ColorRole role)
    {
        if (!AmoledEnabled || !IsDark ||
            !AmoledColors.TryGetValue(role.Key, out var tone))
        {
            return role.Color;
        }

        var hct = Hct.FromInt(role.Color.ToInt());

        return hct is null ? role.Color : Color.FromInt(
            TonalPalette.FromHueAndChroma(hct.Hue, SurfaceChroma)!.Tone(tone));
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

    private void BuildColorRoles(bool isDark)
    {
        var roles = GetRoles(isDark);

        if (roles.Count > 0)
        {
            return;
        }

        using var enumerator = ColorRole.GetAllDefinitions().GetEnumerator();
        var hasNext = enumerator.MoveNext();

        while (hasNext)
        {
            var definition = enumerator.Current;
            hasNext = enumerator.MoveNext();

            var corners = hasNext
                ? new CornerRadius(5)
                : new CornerRadius(5, 5, 15, 15);

            roles.Add(new ColorRole(definition, corners, FallbackSeed,
                role => OnColorRoleChanged(role, isDark)));
        }
    }

    private void EnsureManualColorsInitialized()
    {
        var wasSuspended = _suspendApply;
        _suspendApply = true;

        ForBothThemes(EnsureManualColorsInitialized);

        _suspendApply = wasSuspended;
    }

    private void EnsureManualColorsInitialized(bool isDark)
    {
        if (preferences.Get(GetManualColorsInitializedKey(isDark), false))
        {
            return;
        }

        var fallbackColors = GetDerivedColors(isDark, applyAmoled: false);

        foreach (var role in GetRoles(isDark))
        {
            var value = fallbackColors.GetValueOrDefault(role.Key, FallbackSeed.ToInt());

            role.Color = Color.FromInt(value);
            preferences.Set(GetManualColorKey(isDark, role.Key), value);
        }

        preferences.Set(GetManualColorsInitializedKey(isDark), true);
    }

    private void LoadColorRoles(bool isDark)
    {
        var roles = GetRoles(isDark);
        var fallbackColors = GetDerivedColors(isDark, applyAmoled: false);

        if (isDark)
        {
            MigrateLegacyManualColors(roles, fallbackColors);
        }

        foreach (var role in roles)
        {
            var fallback = fallbackColors.GetValueOrDefault(role.Key, FallbackSeed.ToInt());

            role.Color = Color.FromInt(
                preferences.Get(GetManualColorKey(isDark, role.Key), fallback));
        }
    }

    private void MigrateLegacyManualColors(List<ColorRole> roles,
        Dictionary<string, int> fallbackColors)
    {
        if (preferences.Get(DarkManualColorsInitializedKey, false) ||
            !preferences.Get(LegacyManualColorsInitializedKey, false))
        {
            return;
        }

        foreach (var role in roles)
        {
            var legacyKey = $"{LegacyManualColorPrefix}{role.Key}";

            if (!preferences.ContainsKey(legacyKey))
            {
                continue;
            }

            var fallback = fallbackColors.GetValueOrDefault(role.Key, FallbackSeed.ToInt());

            preferences.Set(GetManualColorKey(true, role.Key),
                preferences.Get(legacyKey, fallback));
        }

        preferences.Set(DarkManualColorsInitializedKey, true);
    }

    private void RepairAmoledSeededManualColors()
    {
        if (preferences.Get(ManualColorsAmoledRepairKey, false))
        {
            return;
        }

        preferences.Set(ManualColorsAmoledRepairKey, true);

        var baseColors = GetDerivedColors(true, applyAmoled: false);
        var amoledColors = GetDerivedColors(true, applyAmoled: true);

        foreach (var role in _darkColorRoles)
        {
            if (!AmoledColors.ContainsKey(role.Key) ||
                !baseColors.TryGetValue(role.Key, out var baseValue) ||
                !amoledColors.TryGetValue(role.Key, out var amoledValue) ||
                baseValue == amoledValue ||
                role.Color.ToInt() != amoledValue)
            {
                continue;
            }

            role.Color = Color.FromInt(baseValue);
            preferences.Set(GetManualColorKey(true, role.Key), baseValue);
        }
    }

    private static string GetManualColorKey(bool isDark, string role) =>
        (isDark ? DarkManualColorPrefix : LightManualColorPrefix) + role;

    private static string GetManualColorsInitializedKey(bool isDark) =>
        isDark ? DarkManualColorsInitializedKey : LightManualColorsInitializedKey;

    private static string GetColorPresetKey(bool isDark) =>
        isDark ? DarkColorPresetKey : LightColorPresetKey;

    private ColorPreset GetColorPreset(bool isDark) =>
        isDark ? DarkColorPreset : LightColorPreset;

    private void OnColorPresetChanged(bool isDark, ColorPreset preset)
    {
        if (ColorPresets.TryGetColors(preset, isDark, out var colors))
        {
            _applyingPreset = true;

            try
            {
                foreach (var role in GetRoles(isDark))
                {
                    if (colors.TryGetValue(role.Key, out var value))
                    {
                        role.Color = Color.FromInt(value);
                    }
                }
            }
            finally
            {
                _applyingPreset = false;
            }
        }

        preferences.Set(GetColorPresetKey(isDark), (int)preset);
        OnPropertyChanged(isDark
            ? nameof(DarkColorPreset)
            : nameof(LightColorPreset));
    }

    private void LoadColorPresets()
    {
        DarkColorPreset = LoadColorPreset(true);
        LightColorPreset = LoadColorPreset(false);
    }

    private ColorPreset LoadColorPreset(bool isDark)
    {
        var value = preferences.Get(GetColorPresetKey(isDark), (int)ColorPreset.Custom);
        var preset = Enum.IsDefined((ColorPreset)value)
            ? (ColorPreset)value
            : ColorPreset.Custom;

        if (preset != ColorPreset.Custom &&
            !ColorPresets.Matches(preset, isDark, GetRoles(isDark)))
        {
            preset = ColorPreset.Custom;
            preferences.Set(GetColorPresetKey(isDark), (int)preset);
        }

        return preset;
    }

    private void OnColorRoleChanged(ColorRole role, bool isDark)
    {
        if (_suspendApply)
        {
            return;
        }

        preferences.Set(GetManualColorKey(isDark, role.Key), role.Color.ToInt());

        if (!_applyingPreset && GetColorPreset(isDark) != ColorPreset.Custom)
        {
            if (isDark)
            {
                DarkColorPreset = ColorPreset.Custom;
            }
            else
            {
                LightColorPreset = ColorPreset.Custom;
            }
        }

        if (ColorMode == ColorMode.Manual && IsDark == isDark)
        {
            SetResource(role.Key, GetManualColor(role));
            OnPropertyChanged(nameof(ShellColor));
        }
    }

    public void Initialize(ResourceDictionary resourceDictionary)
    {
        _resources = resourceDictionary;
        ForBothThemes(BuildColorRoles);
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
        ForBothThemes(BuildColorRoles);
        ForBothThemes(LoadColorRoles);
        RepairAmoledSeededManualColors();

        if (ColorMode == ColorMode.Manual)
        {
            EnsureManualColorsInitialized();
        }

        LoadColorPresets();
        _suspendApply = false;
        UpdateTheme();
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) =>
        UpdateTheme();

    private void UpdateTheme()
    {
        systemScheme.UpdatePalettes();
        Apply();

        OnPropertyChanged(nameof(PreferredTheme));
        OnPropertyChanged(nameof(AmoledValues));
        OnPropertyChanged(nameof(ShellColor));
    }

    public void OnNavigated(object? sender, ShellNavigatedEventArgs e) =>
        OnPropertyChanged(nameof(ShellColor));
}
