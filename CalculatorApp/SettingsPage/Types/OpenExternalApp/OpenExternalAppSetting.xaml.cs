namespace CalculatorApp.SettingsPage.Types.OpenExternalApp;

public partial class OpenExternalAppSetting
{
    public OpenExternalAppSetting() => InitializeComponent();

    private void OnOpenClicked(object? sender, EventArgs e)
    {
        _ = Launcher.OpenAsync(Uri);
    }

    public static readonly BindableProperty UriProperty =
        BindableProperty.Create(
            nameof(Uri),
            typeof(Uri),
            typeof(OpenExternalAppSetting));

    public Uri Uri
    {
        get => (Uri)GetValue(UriProperty);
        set => SetValue(UriProperty, value);
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(OpenExternalAppSetting));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(OpenExternalAppSetting));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(OpenExternalAppSetting));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}
