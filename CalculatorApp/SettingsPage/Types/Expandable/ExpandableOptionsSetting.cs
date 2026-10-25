using System.ComponentModel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Types.Options;

namespace CalculatorApp.SettingsPage.Types.Expandable;

public class ExpandableOptionsSetting : OptionsSetting
{
    public ExpandableOptionsSetting() => PropertyChanged += OnPropertyChanged;

    private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(SelectedOption) or nameof(ExpandableValues)
            or nameof(ExpandableView) or nameof(NonExpandableView))
        {
            var shouldShowContent = ExpandableValues.Contains(SelectedOption);
            ExpandableView?.InputTransparent = !shouldShowContent;
            NonExpandableView?.InputTransparent = shouldShowContent;

            if (TransitionAnimation is TransitionAnimation.Fade)
            {
                await Task.WhenAll(
                    ExpandableView?.FadeToAsync(
                        shouldShowContent ? 1d : 0.3d,
                        250,
                        shouldShowContent ? Easing.SinIn : Easing.SinOut
                    ) ?? Task.CompletedTask,

                    NonExpandableView?.FadeToAsync(
                        !shouldShowContent ? 1d : 0.3d,
                        250,
                        !shouldShowContent ? Easing.SinIn : Easing.SinOut
                    ) ?? Task.CompletedTask
                );
            }
            else if (TransitionAnimation is TransitionAnimation.None)
            {
                ExpandableView?.Opacity = shouldShowContent ? 1d : 0.3d;
                NonExpandableView?.Opacity = !shouldShowContent ? 1d : 0.3d;
            }
        }
    }

    public static readonly BindableProperty ExpandableViewProperty =
        BindableProperty.Create(
            nameof(ExpandableView),
            typeof(VisualElement),
            typeof(ExpandableOptionsSetting));

    public VisualElement? ExpandableView
    {
        get => (VisualElement?)GetValue(ExpandableViewProperty);
        set => SetValue(ExpandableViewProperty, value);
    }

   public static readonly BindableProperty NonExpandableViewProperty =
        BindableProperty.Create(
            nameof(NonExpandableView),
            typeof(VisualElement),
            typeof(ExpandableOptionsSetting));

    public VisualElement? NonExpandableView
    {
        get => (VisualElement?)GetValue(NonExpandableViewProperty);
        set => SetValue(NonExpandableViewProperty, value);
    }

    public static readonly BindableProperty ExpandableValuesProperty =
        BindableProperty.Create(
            nameof(ExpandableValues),
            typeof(Enum[]),
            typeof(ExpandableOptionsSetting),
            defaultValue: Array.Empty<Enum>());

    public Enum[] ExpandableValues
    {
        get => (Enum[])GetValue(ExpandableValuesProperty);
        set => SetValue(ExpandableValuesProperty, value);
    }

    public static readonly BindableProperty TransitionAnimationProperty =
        BindableProperty.Create(
            nameof(TransitionAnimation),
            typeof(TransitionAnimation),
            typeof(ExpandableOptionsSetting),
            defaultValue: TransitionAnimation.None);

    public TransitionAnimation TransitionAnimation
    {
        get => (TransitionAnimation)GetValue(TransitionAnimationProperty);
        set => SetValue(TransitionAnimationProperty, value);
    }
}
