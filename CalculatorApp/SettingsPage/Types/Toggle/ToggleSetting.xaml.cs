namespace CalculatorApp.SettingsPage.Types.Toggle;

public partial class ToggleSetting
{
    public ToggleSetting() => InitializeComponent();

    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(
            nameof(IsToggled),
            typeof(bool),
            typeof(ToggleSetting),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnToggledChanged);

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    private static void OnToggledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ToggleSetting setting)
        {
            return;
        }

        VisualStateManager.GoToState(setting.Switch, setting.IsToggled ? "Checked" : "Normal");
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(ToggleSetting));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(ToggleSetting));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(ToggleSetting));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    private void OnClicked(object? sender, EventArgs e) => IsToggled = !IsToggled;
}
