using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.Colors.ColorChooser;

public partial class ColorChooser
{
    private List<ColorPreset> _presets = [];
    private ColorPreset? _selectedPreset;

    public ColorChooser()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        PresetsView.ScrollTo(
            item: _selectedPreset,
            position: ScrollToPosition.Start,
            animate: false);
        Loaded -= OnLoaded;
    }

    public static readonly BindableProperty ColorsProperty =
        BindableProperty.Create(
            nameof(Colors),
            typeof(IEnumerable<Color>),
            typeof(ColorChooser),
            propertyChanged: OnColorsChanged);

    public IEnumerable<Color> Colors
    {
        get => (IEnumerable<Color>)GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    private static void OnColorsChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        RebuildPresets(bindable);
    }

    public static readonly BindableProperty DefaultColorIndexProperty =
        BindableProperty.Create(
            nameof(DefaultColorIndex),
            typeof(int),
            typeof(ColorChooser),
            defaultValue: 0,
            propertyChanged: OnDefaultColorIndexChanged);

    private static void OnDefaultColorIndexChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        RebuildPresets(bindable);
    }

    private static void RebuildPresets(BindableObject bindable)
    {
        if (bindable is not ColorChooser chooser)
            return;

        chooser._presets = [.. chooser.Colors.Select(x => new ColorPreset(x))];

        chooser.PresetsView.ItemsSource = chooser._presets;
    }

    public int DefaultColorIndex
    {
        get => (int)GetValue(DefaultColorIndexProperty);
        set => SetValue(DefaultColorIndexProperty, value);
    }

    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ColorChooser),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnSelectedColorChanged);

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    private static void OnSelectedColorChanged(
        BindableObject bindable,
        object? oldValue,
        object? newValue)
    {
        if (bindable is ColorChooser chooser)
        {
            chooser._selectedPreset?.IsSelected = false;
            chooser._selectedPreset = chooser._presets.FirstOrDefault(preset =>
                preset.Color.Equals(chooser.SelectedColor),
                chooser._presets[chooser.DefaultColorIndex]);
            chooser._selectedPreset?.IsSelected = true;
        }
    }

    private void OnClicked(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: ColorPreset preset })
        {
            SelectedColor = preset.Color;
        }
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(ColorChooser));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(ColorChooser));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(ColorChooser));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}