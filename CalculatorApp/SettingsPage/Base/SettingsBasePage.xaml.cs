using CalculatorApp.Platforms.Android.Behaviors;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Colors.ColorChooser;
using CalculatorApp.SettingsPage.CustomLayout.LayoutBuilder;
using CalculatorApp.SettingsPage.Types.Expandable;
using CalculatorApp.SettingsPage.Types.Options;
using CalculatorApp.SharedElements.ContentButton;

namespace CalculatorApp.SettingsPage.Base;

using Layout = Microsoft.Maui.Controls.Layout;

public partial class SettingsBasePage
{
    private const int ItemSpacing = 2;
    public double LayoutHeight => Height * 0.6;
    public AnimationSettings AnimationSettings { get; }

    public SettingsBasePage(AnimationSettings animationSettings)
    {
        AnimationSettings = animationSettings;
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(LayoutHeight));
        SizeChanged -= OnSizeChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TitleLabel.Opacity = 0;
    }

    public static readonly BindableProperty IsScrollableProperty =
        BindableProperty.Create(
            nameof(IsScrollable),
            typeof(bool),
            typeof(SettingsBasePage),
            defaultValue: true);

    public bool IsScrollable
    {
        get => (bool)GetValue(IsScrollableProperty);
        set => SetValue(IsScrollableProperty, value);
    }

    public static readonly BindableProperty HeaderProperty =
        BindableProperty.Create(
            nameof(Header),
            typeof(string),
            typeof(SettingsBasePage));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly BindableProperty SettingsContentProperty =
        BindableProperty.Create(
            nameof(SettingsContent),
            typeof(Layout),
            typeof(SettingsBasePage),
            propertyChanged: OnSettingsContentChanged);

    public VerticalStackLayout SettingsContent
    {
        get => (VerticalStackLayout)GetValue(SettingsContentProperty);
        set => SetValue(SettingsContentProperty, value);
    }

    private static void OnSettingsContentChanged(BindableObject bindable,
        object oldValue, object newValue)
    {
        if (bindable is not SettingsBasePage page ||
            newValue is not VerticalStackLayout layout)
        {
            return;
        }

        layout.Spacing = ItemSpacing;
        page.UpdateContent(layout);
    }

    private void SetContent(Layout layout)
    {
        foreach (var child in layout)
        {
            switch (child)
            {
                case ExpandableOptionsSetting expandableOptions:
                    expandableOptions.SetBinding(ExpandableOptionsSetting.TransitionAnimationProperty,
                        new Binding(nameof(AnimationSettings.SettingsTransitionAnimation),
                            source: AnimationSettings));
                    break;
                case ExpandableToggleSetting expandableToggle:
                    expandableToggle.SetBinding(ExpandableToggleSetting.TransitionAnimationProperty,
                        new Binding(nameof(AnimationSettings.SettingsTransitionAnimation),
                            source: AnimationSettings));
                    break;
            }

            switch (child)
            {
                case OptionsSetting options:
                    options.SetBinding(ContentButton.ClickAnimationProperty,
                        new Binding(nameof(AnimationSettings.SettingsClickAnimation),
                            source: AnimationSettings));
                    break;
                case LayoutBuilder builder:
                    builder.SetBinding(ContentButton.ClickAnimationProperty,
                        new Binding(nameof(AnimationSettings.SettingsClickAnimation),
                            source: AnimationSettings));
                    break;
                case ColorChooser chooser:
                    chooser.SetBinding(ContentButton.ClickAnimationProperty,
                        new Binding(nameof(AnimationSettings.SettingsClickAnimation),
                            source: AnimationSettings));
                    break;
                case ContentButton contentButton:
                    contentButton.SetBinding(ContentButton.ClickAnimationProperty,
                        new Binding(nameof(AnimationSettings.SettingsClickAnimation),
                            source: AnimationSettings));
                    break;
                case VerticalStackLayout childLayout:
                    childLayout.Spacing = ItemSpacing;
                    SetContent(childLayout);
                    break;
            }
        }
    }

    private void UpdateContent(Layout layout)
    {
        SetContent(layout);
        var headerLabel = new Label
        {
            Style = Application.Current?.Resources["SettingHeader"] as Style,
            Text = Header
        };
        layout.Insert(0, headerLabel);

        if (IsScrollable)
        {
            var scrollView = new ScrollView()
            {
                SafeAreaEdges = new SafeAreaEdges(SafeAreaRegions.None),
                Padding = new Thickness(10, 0, 10, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                Content = layout
            };
            scrollView.Scrolled += OnScrolled;
            ContentContainer.Content = scrollView;
        }
        else
        {
            layout.Padding = new Thickness(10, 0, 10, 0);
            ContentContainer.Content = layout;
        }
        layout.Behaviors.Add(new AndroidBottomInsetBehavior());
    }

    private const double ScrollTitleThreshold = 40, TranslationThreshold = 10;

    private void OnScrolled(object? sender, ScrolledEventArgs e)
    {
        var scrollY = e.ScrollY;

        if (scrollY > TitleLabel.Bounds.Bottom + ScrollTitleThreshold)
        {
            TitleLabel.Opacity = 1;
            TitleLabel.TranslationY = 0;
        }
        else if (scrollY < TitleLabel.Bounds.Bottom)
        {
            TitleLabel.Opacity = 0;
            TitleLabel.TranslationY = TranslationThreshold;
        }
        else
        {
            TitleLabel.Opacity = (scrollY - TitleLabel.Bounds.Bottom) / ScrollTitleThreshold;
            TitleLabel.TranslationY = TranslationThreshold * (1 - TitleLabel.Opacity);
        }
    }
}
