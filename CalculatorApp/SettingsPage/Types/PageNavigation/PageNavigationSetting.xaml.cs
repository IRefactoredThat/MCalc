namespace CalculatorApp.SettingsPage.Types.PageNavigation;

using Shell = Microsoft.Maui.Controls.Shell;

public partial class PageNavigationSetting
{
    public PageNavigationSetting() => InitializeComponent();

    private async void OnNavigateClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(Route.Name);
    }

    public static readonly BindableProperty RouteProperty =
        BindableProperty.Create(
            nameof(Route),
            typeof(Type),
            typeof(PageNavigationSetting));

    public Type Route
    {
        get => (Type)GetValue(RouteProperty);
        set => SetValue(RouteProperty, value);
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(PageNavigationSetting));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(PageNavigationSetting));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(PageNavigationSetting));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}
