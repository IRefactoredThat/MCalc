using System.ComponentModel;
using System.Runtime.CompilerServices;
using CalculatorApp.CalculatorPage.Page;
using CalculatorApp.SettingsPage.Backup;
using Google.Android.Material.Color.Utilities;

namespace CalculatorApp.SettingsPage.Appearance;

using Shell = Microsoft.Maui.Controls.Shell;

public enum Theme { Auto, Light, Dark }
public enum Style { Vibrant, TonalSpot, Expressive,
    Rainbow, FruitSalad, Content, Fidelity }

public class AppearanceSettings(IPreferences preferences,
    SystemScheme systemScheme, AppearanceSchemeStorage schemeStorage) :
    INotifyPropertyChanged, IReload
{
    private const double ContrastLevel = 0.0;
    private const double SurfaceChroma = 30.0;
    private static readonly Color FallbackSeed = SeedPresets.
        Presets[DefaultColorIndex];

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

    private ResourceDictionary _resources = null!;
    private MaterialScheme _currentScheme = null!;

    private (Style Style, int Argb, bool IsDark) _seededKey;
    private bool _suspendApply;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public const int DefaultColorIndex = 0;

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

    public Color ShellColor => Shell.Current?.CurrentPage is CalculatorGridPage
        ? (Color)_resources[nameof(MaterialScheme.SurfaceContainerHighest)]
        : (Color)_resources[nameof(MaterialScheme.Surface)];

    private void Apply()
    {
        if (_suspendApply)
        {
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
        var scheme = DynamicColors && systemScheme.ToScheme(IsDark) is { } system
            ? system
            : GetSeedScheme(SeedColor.ToInt(), isDark);
        ApplyColors(scheme, isDark);
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

    private void ApplyCachedColors(IReadOnlyDictionary<string, int> roleColors)
    {
        foreach (var (name, value) in roleColors)
        {
            _resources[name] = Color.FromInt(value);
        }
    }

    private void ApplyColors(MaterialScheme scheme, bool isDark)
    {
        var amoled = AmoledEnabled && isDark;
        var neutral = scheme.Neutral;

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

            if (!_resources.TryGetValue(name, out var current) ||
                !color.Equals(current))
            {
                _resources[name] = Color.FromInt(color);
            }

            schemeStorage.SetColorRole(name, color);
        }
    }

    private static int Tint(int color)
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

    public void Initialize(ResourceDictionary resourceDictionary)
    {
        _resources = resourceDictionary;
        Application.Current!.RequestedThemeChanged += OnThemeChanged;
    }

    public void Reload()
    {
        _suspendApply = true;
        SeedColor = Color.FromInt(preferences.Get(nameof(SeedColor),
            FallbackSeed.ToInt()));
        Style = (Style)preferences.Get(nameof(Style), 0);
        DynamicColors = preferences.Get(nameof(DynamicColors), true);
        PreferredTheme = (Theme)preferences.Get(nameof(PreferredTheme), 0);
        AmoledEnabled = preferences.Get(nameof(AmoledEnabled), false);
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
