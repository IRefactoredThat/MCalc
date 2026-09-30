using System.Windows.Input;
using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SharedElements.ContentButton;

public class ContentButton : ContentView
{
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ContentButton));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty StrokeThicknessProperty =
        BindableProperty.Create(
            nameof(StrokeThickness),
            typeof(double),
            typeof(ContentButton),
            defaultValue: 0d);

    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public static readonly BindableProperty StrokeProperty =
        BindableProperty.Create(
            nameof(Stroke),
            typeof(Color),
            typeof(ContentButton),
            defaultValue: Colors.Transparent);

    public Color Stroke
    {
        get => (Color)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(ContentButton));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(ContentButton));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly BindableProperty LongCommandProperty =
        BindableProperty.Create(
            nameof(LongCommand),
            typeof(ICommand),
            typeof(ContentButton));

    public ICommand? LongCommand
    {
        get => (ICommand?)GetValue(LongCommandProperty);
        set => SetValue(LongCommandProperty, value);
    }

    public static readonly BindableProperty LongCommandParameterProperty =
        BindableProperty.Create(
            nameof(LongCommandParameter),
            typeof(object),
            typeof(ContentButton));

    public object? LongCommandParameter
    {
        get => GetValue(LongCommandParameterProperty);
        set => SetValue(LongCommandParameterProperty, value);
    }

    private EventHandler? _clicked;

    public static readonly BindableProperty HasClickSubscribersProperty =
        BindableProperty.Create(
            nameof(HasClickSubscribers),
            typeof(bool),
            typeof(ContentButton));

    public bool HasClickSubscribers => (bool)GetValue(HasClickSubscribersProperty);

    public event EventHandler? Clicked
    {
        add
        {
            _clicked += value;
            SetValue(HasClickSubscribersProperty, _clicked is not null);
        }
        remove
        {
            _clicked -= value;
            SetValue(HasClickSubscribersProperty, _clicked is not null);
        }
    }

    private ScaleBehavior? _scaleBehavior;

    public static readonly BindableProperty ClickAnimationProperty =
        BindableProperty.Create(
            nameof(ClickAnimation),
            typeof(ClickAnimation),
            typeof(ContentButton),
            defaultValue: ClickAnimation.None,
            propertyChanged: OnClickAnimationChanged);

    public ClickAnimation ClickAnimation
    {
        get => (ClickAnimation)GetValue(ClickAnimationProperty);
        set => SetValue(ClickAnimationProperty, value);
    }

    private static void OnClickAnimationChanged(BindableObject bindable,
        object oldValue, object newValue)
    {
        if (bindable is not ContentButton button ||
            newValue is not ClickAnimation clickAnimation)
        {
            return;
        }

        if (button._scaleBehavior is { } previous)
        {
            button.Behaviors.Remove(previous);
        }

        button._scaleBehavior = clickAnimation switch
        {
            ClickAnimation.Scale => new ScaleBehavior
            {
                PressedScale = 0.98,
                PressedEasing = Easing.CubicOut,
                ReleasedEasing = Easing.CubicInOut
            },
            ClickAnimation.Spring => new ScaleBehavior
            {
                PressedScale = 0.9,
                PressedEasing = Easing.CubicOut,
                ReleasedEasing = Easing.SpringOut
            },
            _ => null
        };

        if (button._scaleBehavior is { } behavior)
        {
            button.Behaviors.Add(behavior);
        }
    }

    public event EventHandler? Released;
    public event EventHandler? Pressed;

    public void SendPressed() => Pressed?.Invoke(this, EventArgs.Empty);
    public void SendReleased() => Released?.Invoke(this, EventArgs.Empty);
    public void SendClicked()
    {
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
        _clicked?.Invoke(this, EventArgs.Empty);
    }

    public bool SendLongClicked()
    {
        if (LongCommand?.CanExecute(LongCommandParameter) != true)
        {
            return false;
        }

        LongCommand.Execute(LongCommandParameter);

        return true;
    }
}
