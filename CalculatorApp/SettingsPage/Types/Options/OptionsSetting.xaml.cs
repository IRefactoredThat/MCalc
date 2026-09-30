using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SharedElements.ContentButton;

namespace CalculatorApp.SettingsPage.Types.Options;

public partial class OptionsSetting
{
    public OptionsSetting()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SelectedOptionProperty =
        BindableProperty.Create(
            nameof(SelectedOption),
            typeof(Enum),
            typeof(OptionsSetting),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnSelectedOptionChanged);

    public Enum SelectedOption
    {
        get => (Enum)GetValue(SelectedOptionProperty);
        set => SetValue(SelectedOptionProperty, value);
    }

    private static void OnSelectedOptionChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not OptionsSetting optionsSetting)
        {
            return;
        }

        var newOptionType = newValue?.GetType();
        if (newOptionType is not null &&
            oldValue?.GetType() != newOptionType)
        {
            optionsSetting.RebuildItems(newOptionType);
        }

        optionsSetting.SyncOptionStates(newValue);
    }

    private void RebuildItems(Type optionType)
    {
        BindableLayout.SetItemsSource(Options, Enum.GetValues(optionType));
    }

    private void SyncOptionStates(object? newValue)
    {
        foreach (var child in Options.Children.OfType<ContentButton>())
        {
            var state = Equals(child.BindingContext, newValue) ? "Checked" : "Normal";
            VisualStateManager.GoToState(child, state);
            VisualStateManager.GoToState(child.Content, state);
        }
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        if (sender is not BindableObject { BindingContext: Enum context })
        {
            return;
        }

        SelectedOption = context;
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(OptionsSetting));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(OptionsSetting));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(OptionsSetting));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}
