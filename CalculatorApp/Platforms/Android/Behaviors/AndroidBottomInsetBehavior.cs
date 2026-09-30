using AndroidX.Core.View;
using View = Android.Views.View;
using Object = Java.Lang.Object;

namespace CalculatorApp.Platforms.Android.Behaviors;

internal sealed class BottomInsetListener : Object, IOnApplyWindowInsetsListener
{
    public WindowInsetsCompat? OnApplyWindowInsets(View? v, WindowInsetsCompat? insets)
    {
        if (v is null || insets is null)
        {
            return insets;
        }

        var bottomInset = insets.GetInsets(
            WindowInsetsCompat.Type.SystemBars())?.Bottom ?? 0;

        v.SetPadding(
            v.PaddingLeft,
            v.PaddingTop,
            v.PaddingRight,
            bottomInset);

        return insets;
    }
}

public class AndroidBottomInsetBehavior : Behavior<VisualElement>
{
    protected override void OnAttachedTo(VisualElement element)
    {
        base.OnAttachedTo(element);
        element.HandlerChanged += OnHandlerChanged;
    }

    protected override void OnDetachingFrom(VisualElement element)
    {
        element.HandlerChanged -= OnHandlerChanged;
        base.OnDetachingFrom(element);
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is not VisualElement obj ||
            obj.Handler?.PlatformView is not View platformView)
        {
            return;
        }

        ViewCompat.SetOnApplyWindowInsetsListener(
            platformView, new BottomInsetListener());
    }
}