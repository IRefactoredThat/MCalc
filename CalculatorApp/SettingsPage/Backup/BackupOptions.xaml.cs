namespace CalculatorApp.SettingsPage.Backup;

public partial class BackupOptions
{
    public BackupOptions() => InitializeComponent();

    public static readonly BindableProperty IncludePreferencesProperty =
        BindableProperty.Create(
            nameof(IncludePreferences),
            typeof(bool),
            typeof(BackupSetting),
            defaultBindingMode: BindingMode.TwoWay);

    public bool IncludePreferences
    {
        get => (bool)GetValue(IncludePreferencesProperty);
        set => SetValue(IncludePreferencesProperty, value);
    }

    public static readonly BindableProperty IncludeHistoryProperty =
        BindableProperty.Create(
            nameof(IncludeHistory),
            typeof(bool),
            typeof(BackupSetting),
            defaultBindingMode: BindingMode.TwoWay);

    public bool IncludeHistory
    {
        get => (bool)GetValue(IncludeHistoryProperty);
        set => SetValue(IncludeHistoryProperty, value);
    }

    public static readonly BindableProperty IncludeCustomModelProperty =
        BindableProperty.Create(
            nameof(IncludeCustomModel),
            typeof(bool),
            typeof(BackupSetting),
            defaultBindingMode: BindingMode.TwoWay);

    public bool IncludeCustomModel
    {
        get => (bool)GetValue(IncludeCustomModelProperty);
        set => SetValue(IncludeCustomModelProperty, value);
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(BackupSetting),
            defaultBindingMode: BindingMode.TwoWay);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(BackupSetting),
            defaultBindingMode: BindingMode.TwoWay);

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(BackupSetting),
            defaultBindingMode: BindingMode.TwoWay);

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}

