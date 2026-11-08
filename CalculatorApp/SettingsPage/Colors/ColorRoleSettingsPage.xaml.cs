using System.ComponentModel;
using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public partial class ColorRoleSettingsPage
{
    private readonly AppearanceSettings _appearanceSettings;
    private readonly bool _isDarkPalette;
    private bool _subscribed;

    public ColorWheelDialog ColorWheelDialog { get; }
    public IReadOnlyList<ColorRole> ColorRoles { get; }

    public ColorPreset Preset
    {
        get => _isDarkPalette
            ? _appearanceSettings.DarkColorPreset
            : _appearanceSettings.LightColorPreset;
        set
        {
            if (Preset == value)
            {
                return;
            }

            if (_isDarkPalette)
            {
                _appearanceSettings.DarkColorPreset = value;
            }
            else
            {
                _appearanceSettings.LightColorPreset = value;
            }

            OnPropertyChanged();
        }
    }

    public ColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog)
        : this(appearanceSettings, animationSettings, colorWheelDialog,
            appearanceSettings.DarkColorRoles, true)
    {
    }

    protected ColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog,
        IReadOnlyList<ColorRole> colorRoles, bool isDarkPalette)
        : base(animationSettings)
    {
        _appearanceSettings = appearanceSettings;
        _isDarkPalette = isDarkPalette;
        ColorWheelDialog = colorWheelDialog;
        ColorRoles = colorRoles;
        Header = isDarkPalette ? "Dark palette" : "Light palette";
        InitializeComponent();
        BindingContext = appearanceSettings;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_subscribed)
        {
            return;
        }

        _appearanceSettings.PropertyChanged += OnAppearanceSettingsChanged;
        _subscribed = true;
    }

    protected override void OnDisappearing()
    {
        if (_subscribed)
        {
            _appearanceSettings.PropertyChanged -= OnAppearanceSettingsChanged;
            _subscribed = false;
        }

        base.OnDisappearing();
    }

    private void OnAppearanceSettingsChanged(object? sender,
        PropertyChangedEventArgs e)
    {
        var presetProperty = _isDarkPalette
            ? nameof(AppearanceSettings.DarkColorPreset)
            : nameof(AppearanceSettings.LightColorPreset);

        if (e.PropertyName == presetProperty)
        {
            OnPropertyChanged(nameof(Preset));
        }
    }
}
