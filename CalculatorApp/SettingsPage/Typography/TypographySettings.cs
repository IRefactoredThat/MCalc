using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.Typography;

public enum CustomFonts { Inconsolata, FantasqueSansMono, SometypeMono }

public class TypographySettings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private const string SystemFont = "System";
    public const string Icons = "MaterialSymbols";
    public const string IconsFilled = "MaterialSymbolsFilled";

    public const double FlyoutHeaderSize = 30, FlyoutItemSize = 20;
    public const double InputSize = 80;
    private const double InputMinSize = 50;
    private const double InputMaxSize = 90;
    public const double OutputSize = 50;
    public const double HistoryItemSize = 40, HistoryHeaderSize = 30;
    public const double SettingGroupHeaderSize = 40, SettingGroupSubheaderSize = 25;
    public const double SettingNameSize = 20, SettingDescriptionSize = 14, SettingOptionSize = 14;
    public const double HistoryClearQuestionSize = 30, HistoryEmptySize = 30;
    public const double DefaultFontSize = 14, ShellTitleSize = 25;
    public const double IconSize = 24, LayoutOptionSize = 18;
    public const double NumberPreviewSize = 34, LayoutBuilderSize = 25;
    public const double GridDrawableSize = 16, SnackbarSize = 18, ButtonMinFontSize = 10;

    public const double MinScale = 0.8, MaxScale = 1.2, DefaultScale = 1;
    public const double ScaleStep = 0.02;
    public const double InputWidthFactor = 0.95;
    public const double InputRescaleFactor = 0.2;

    private static readonly (string Key, string FactorKey, double Default)[] FontSizes =
    [
        (nameof(FlyoutHeaderSize), nameof(FlyoutHeaderScale), FlyoutHeaderSize),
        (nameof(FlyoutItemSize), nameof(FlyoutItemScale), FlyoutItemSize),
        (nameof(InputSize), nameof(InputScale), InputSize),
        (nameof(OutputSize), nameof(OutputScale), OutputSize),
        (nameof(HistoryItemSize), nameof(HistoryItemScale), HistoryItemSize),
        (nameof(HistoryHeaderSize), nameof(HistoryHeaderScale), HistoryHeaderSize),
        (nameof(SettingGroupHeaderSize), nameof(SettingGroupHeaderScale), SettingGroupHeaderSize),
        (nameof(SettingGroupSubheaderSize), nameof(SettingGroupSubheaderScale), SettingGroupSubheaderSize),
        (nameof(SettingNameSize), nameof(SettingNameScale), SettingNameSize),
        (nameof(SettingDescriptionSize), nameof(SettingDescriptionScale), SettingDescriptionSize),
        (nameof(SettingOptionSize), nameof(SettingOptionScale), SettingOptionSize),
        (nameof(HistoryClearQuestionSize), nameof(HistoryClearQuestionScale), HistoryClearQuestionSize),
        (nameof(HistoryEmptySize), nameof(HistoryEmptyScale), HistoryEmptySize),
        (nameof(DefaultFontSize), nameof(DefaultFontScale), DefaultFontSize),
        (nameof(ShellTitleSize), nameof(ShellTitleScale), ShellTitleSize),
        (nameof(IconSize), nameof(IconScale), IconSize),
        (nameof(LayoutOptionSize), nameof(LayoutOptionScale), LayoutOptionSize),
        (nameof(NumberPreviewSize), nameof(NumberPreviewScale), NumberPreviewSize),
        (nameof(LayoutBuilderSize), nameof(LayoutBuilderScale), LayoutBuilderSize),
        (nameof(GridDrawableSize), nameof(GridDrawableScale), GridDrawableSize),
        (nameof(SnackbarSize), nameof(SnackbarScale), SnackbarSize),
        (nameof(ButtonMinFontSize), nameof(ButtonMinFontScale), ButtonMinFontSize),
    ];

    private readonly Dictionary<string, double> _factors = new();

    public string AppFont =>
        UseSystemFont ? SystemFont : CustomAppFont.ToString();

    public bool UseSystemFont
    {
        get;
        set
        {
            field = value;
            Application.Current?.Resources[nameof(AppFont)] = AppFont;
            Preferences.Set(nameof(UseSystemFont), value);
            OnPropertyChanged();
        }
    }

    public CustomFonts CustomAppFont
    {
        get;
        set
        {
            field = value;
            Application.Current?.Resources[nameof(AppFont)] = AppFont;
            Preferences.Set(nameof(CustomAppFont), (int)value);
            OnPropertyChanged();
        }
    }

    public bool DynamicTextScaling
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(DynamicTextScaling), value);
            OnPropertyChanged();
        }
    }

    public bool GlobalScaling
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(GlobalScaling), value);
            ApplyScale();
            OnPropertyChanged();
            NotifyAllSizeScales();
        }
    }

    public double GlobalScale
    {
        get;
        set
        {
            value = Snap(value);

            if (Math.Abs(field - value) < 0.0001)
            {
                return;
            }

            field = value;
            Preferences.Set(nameof(GlobalScale), value);
            ApplyScale();
            OnPropertyChanged();

            if (GlobalScaling)
            {
                NotifyAllSizeScales();
            }
        }
    }

    public double FlyoutHeaderScale
    {
        get => GetSizeScale(nameof(FlyoutHeaderScale));
        set => SetSizeScale(nameof(FlyoutHeaderScale), value);
    }

    public double FlyoutItemScale
    {
        get => GetSizeScale(nameof(FlyoutItemScale));
        set => SetSizeScale(nameof(FlyoutItemScale), value);
    }

    public double InputScale
    {
        get => GetSizeScale(nameof(InputScale));
        set => SetSizeScale(nameof(InputScale), value);
    }

    public double OutputScale
    {
        get => GetSizeScale(nameof(OutputScale));
        set => SetSizeScale(nameof(OutputScale), value);
    }

    public double HistoryItemScale
    {
        get => GetSizeScale(nameof(HistoryItemScale));
        set => SetSizeScale(nameof(HistoryItemScale), value);
    }

    public double HistoryHeaderScale
    {
        get => GetSizeScale(nameof(HistoryHeaderScale));
        set => SetSizeScale(nameof(HistoryHeaderScale), value);
    }

    public double SettingGroupHeaderScale
    {
        get => GetSizeScale(nameof(SettingGroupHeaderScale));
        set => SetSizeScale(nameof(SettingGroupHeaderScale), value);
    }

    public double SettingGroupSubheaderScale
    {
        get => GetSizeScale(nameof(SettingGroupSubheaderScale));
        set => SetSizeScale(nameof(SettingGroupSubheaderScale), value);
    }

    public double SettingNameScale
    {
        get => GetSizeScale(nameof(SettingNameScale));
        set => SetSizeScale(nameof(SettingNameScale), value);
    }

    public double SettingDescriptionScale
    {
        get => GetSizeScale(nameof(SettingDescriptionScale));
        set => SetSizeScale(nameof(SettingDescriptionScale), value);
    }

    public double SettingOptionScale
    {
        get => GetSizeScale(nameof(SettingOptionScale));
        set => SetSizeScale(nameof(SettingOptionScale), value);
    }

    public double HistoryClearQuestionScale
    {
        get => GetSizeScale(nameof(HistoryClearQuestionScale));
        set => SetSizeScale(nameof(HistoryClearQuestionScale), value);
    }

    public double HistoryEmptyScale
    {
        get => GetSizeScale(nameof(HistoryEmptyScale));
        set => SetSizeScale(nameof(HistoryEmptyScale), value);
    }

    public double DefaultFontScale
    {
        get => GetSizeScale(nameof(DefaultFontScale));
        set => SetSizeScale(nameof(DefaultFontScale), value);
    }

    public double ShellTitleScale
    {
        get => GetSizeScale(nameof(ShellTitleScale));
        set => SetSizeScale(nameof(ShellTitleScale), value);
    }

    public double IconScale
    {
        get => GetSizeScale(nameof(IconScale));
        set => SetSizeScale(nameof(IconScale), value);
    }

    public double LayoutOptionScale
    {
        get => GetSizeScale(nameof(LayoutOptionScale));
        set => SetSizeScale(nameof(LayoutOptionScale), value);
    }

    public double NumberPreviewScale
    {
        get => GetSizeScale(nameof(NumberPreviewScale));
        set => SetSizeScale(nameof(NumberPreviewScale), value);
    }

    public double LayoutBuilderScale
    {
        get => GetSizeScale(nameof(LayoutBuilderScale));
        set => SetSizeScale(nameof(LayoutBuilderScale), value);
    }

    public double GridDrawableScale
    {
        get => GetSizeScale(nameof(GridDrawableScale));
        set => SetSizeScale(nameof(GridDrawableScale), value);
    }

    public double SnackbarScale
    {
        get => GetSizeScale(nameof(SnackbarScale));
        set => SetSizeScale(nameof(SnackbarScale), value);
    }

    public double ButtonMinFontScale
    {
        get => GetSizeScale(nameof(ButtonMinFontScale));
        set => SetSizeScale(nameof(ButtonMinFontScale), value);
    }

    public double ScaledInputMinSize => InputMinSize * GetSizeScale(nameof(InputScale));
    public double ScaledInputMaxSize => InputMaxSize * GetSizeScale(nameof(InputScale));

    public void Initialize()
    {
        foreach (var (_, factorKey, _) in FontSizes)
        {
            _factors[factorKey] = Preferences.Get(factorKey, DefaultScale);
        }

        UseSystemFont = Preferences.Get(nameof(UseSystemFont), false);
        CustomAppFont = (CustomFonts)Preferences.Get(nameof(CustomAppFont), 0);
        GlobalScale = Preferences.Get(nameof(GlobalScale), DefaultScale);
        GlobalScaling = Preferences.Get(nameof(GlobalScaling), true);
        DynamicTextScaling = Preferences.Get(nameof(DynamicTextScaling), true);
    }

    private double GetSizeScale(string factorKey) =>
        GlobalScaling ? GlobalScale : _factors[factorKey];

    private void SetSizeScale(string factorKey, double value)
    {
        value = Snap(value);

        if (GlobalScaling)
        {
            GlobalScale = value;
            return;
        }

        if (Math.Abs(_factors[factorKey] - value) < 0.0001)
        {
            return;
        }

        _factors[factorKey] = value;
        Preferences.Set(factorKey, value);
        ApplyScale();
        OnPropertyChanged(factorKey);
    }

    private static double Snap(double value) =>
        Math.Clamp(Math.Round(value / ScaleStep, MidpointRounding.AwayFromZero) * ScaleStep,
            MinScale, MaxScale);

    private void NotifyAllSizeScales()
    {
        foreach (var (_, factorKey, _) in FontSizes)
        {
            OnPropertyChanged(factorKey);
        }
    }

    private void ApplyScale()
    {
        if (Application.Current is not { } application)
        {
            return;
        }

        foreach (var (key, factorKey, defaultSize) in FontSizes)
        {
            application.Resources[key] = defaultSize *
                (GlobalScaling ? GlobalScale : _factors[factorKey]);
        }
    }

    public static double GetCurrentSize(string key, double defaultSize) =>
        Application.Current?.Resources.TryGetValue(key, out var value) == true
        && value is double size and > 0 ? size : defaultSize;
}
