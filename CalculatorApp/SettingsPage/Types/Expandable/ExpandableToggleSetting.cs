using System.ComponentModel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Types.Toggle;

namespace CalculatorApp.SettingsPage.Types.Expandable;

public enum OnToggledBehavior
{
    ShowContentWhenToggledOn,
    ShowContentWhenToggledOff
}

public class ExpandableToggleSetting : ToggleSetting
{
    public ExpandableToggleSetting() => PropertyChanged += OnPropertyChanged;

    private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IsToggled) or nameof(ToggledBehavior))
        {
            var shouldShowContent = ShouldShowContent;
            ExpandableView?.InputTransparent = !shouldShowContent;

            if (TransitionAnimation is TransitionAnimation.Fade)
            {
                await (ExpandableView?.FadeToAsync(
                    shouldShowContent ? 1d : 0.3d,
                    250,
                    shouldShowContent ? Easing.SinIn : Easing.SinOut) ?? Task.CompletedTask);
            }
            if (TransitionAnimation is TransitionAnimation.None)
            {
                ExpandableView?.Opacity = shouldShowContent ? 1d : 0.3d;
            }
        }
    }

    private bool ShouldShowContent =>
        IsToggled == (ToggledBehavior == OnToggledBehavior.ShowContentWhenToggledOn);

    public static readonly BindableProperty ToggledBehaviorProperty =
        BindableProperty.Create(
            nameof(ToggledBehavior),
            typeof(OnToggledBehavior),
            typeof(ExpandableToggleSetting),
            defaultValue: OnToggledBehavior.ShowContentWhenToggledOn);

    public OnToggledBehavior ToggledBehavior
    {
        get => (OnToggledBehavior)GetValue(ToggledBehaviorProperty);
        set => SetValue(ToggledBehaviorProperty, value);
    }

    public static readonly BindableProperty ExpandableViewProperty =
        BindableProperty.Create(
            nameof(ExpandableView),
            typeof(VisualElement),
            typeof(ExpandableToggleSetting));

    public VisualElement ExpandableView
    {
        get => (VisualElement)GetValue(ExpandableViewProperty);
        set => SetValue(ExpandableViewProperty, value);
    }

    public static readonly BindableProperty TransitionAnimationProperty =
        BindableProperty.Create(
            nameof(TransitionAnimation),
            typeof(TransitionAnimation),
            typeof(ExpandableOptionsSetting));

    public TransitionAnimation TransitionAnimation
    {
        get => (TransitionAnimation)GetValue(TransitionAnimationProperty);
        set => SetValue(TransitionAnimationProperty, value);
    }
}