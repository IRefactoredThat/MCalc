using Android.Views;
using CalculatorApp.SharedElements.ContentButton;
using Google.Android.Material.Shape;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using View = Android.Views.View;
using Object = Java.Lang.Object;
using Attribute = Android.Resource.Attribute;

namespace CalculatorApp.Platforms.Android.Handlers;

internal sealed class CardTouchListener(ContentButton button) :
    Object, View.IOnTouchListener
{
    public bool OnTouch(View? v, MotionEvent? e)
    {
        if (v?.Clickable != true)
        {
            return false;
        }

        switch (e?.ActionMasked)
        {
            case MotionEventActions.Down:
                button.SendPressed();
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                button.SendReleased();
                break;
        }

        return false;
    }
}

public class ContentButtonHandler() :
    ViewHandler<ContentButton, ContentCardView>(Mapper)
{
    private static readonly
        IPropertyMapper<ContentButton, ContentButtonHandler> Mapper =
        new PropertyMapper<ContentButton, ContentButtonHandler>(ViewMapper)
        {
            [nameof(ContentButton.Content)] = MapContent,
            [nameof(ContentButton.CornerRadius)] = MapCornerRadius,
            [nameof(VisualElement.Background)] = MapBackground,
            [nameof(ContentButton.StrokeThickness)] = MapStrokeThickness,
            [nameof(ContentButton.Stroke)] = MapStroke,
            [nameof(ContentButton.Command)] = MapClickable,
            [nameof(ContentButton.HasClickSubscribers)] = MapClickable,
            [nameof(ContentButton.LongCommand)] = MapLongClickable
        };

    protected override ContentCardView CreatePlatformView()
    {
        var card = new ContentCardView(Context)
        {
            CrossPlatformLayout = VirtualView
        };

        card.Click += OnCardClick;
        card.LongClick += OnCardLongClick;
        card.SetOnTouchListener(new CardTouchListener(VirtualView));
        return card;
    }

    protected override void DisconnectHandler(ContentCardView platformView)
    {
        platformView.Click -= OnCardClick;
        platformView.LongClick -= OnCardLongClick;
        platformView.SetOnTouchListener(null);
        base.DisconnectHandler(platformView);
    }

    private void OnCardClick(object? sender, EventArgs e)
    {
        VirtualView.SendClicked();
    }

    private void OnCardLongClick(object? sender, View.LongClickEventArgs e)
    {
        // A long press that runs something has already consumed the click, but one that
        // cannot (no long command, or its CanExecute is false) would otherwise do nothing.
        if (!VirtualView.SendLongClicked())
        {
            VirtualView.SendClicked();
        }
    }

    private static void MapClickable(
        ContentButtonHandler handler,
        ContentButton view)
    {
        handler.PlatformView.Clickable =
            view.Command is not null || view.HasClickSubscribers;
    }

    private static void MapLongClickable(
        ContentButtonHandler handler,
        ContentButton view)
    {
        handler.PlatformView.LongClickable = view.LongCommand is not null;
    }

    public static void MapContent(
        ContentButtonHandler handler,
        ContentButton view)
    {
        if (view.Content is null)
        {
            return;
        }

        var card = handler.PlatformView;
        card.RemoveAllViews();
        var nativeView = view.Content.ToPlatform(handler.MauiContext!);
        card.AddView(nativeView);
    }

    private static void MapCornerRadius(
        ContentButtonHandler handler,
        ContentButton view)
    {
        var card = handler.PlatformView;
        var context = card.Context;

        var (topLeft, topRight, bottomLeft, bottomRight) = view.CornerRadius;

        card.ShapeAppearanceModel = new ShapeAppearanceModel.Builder()
            .SetTopLeftCorner(new RoundedCornerTreatment())
            .SetTopRightCorner(new RoundedCornerTreatment())
            .SetBottomLeftCorner(new RoundedCornerTreatment())
            .SetBottomRightCorner(new RoundedCornerTreatment())
            .SetTopLeftCornerSize(context.ToPixels(topLeft))
            .SetTopRightCornerSize(context.ToPixels(topRight))
            .SetBottomLeftCornerSize(context.ToPixels(bottomLeft))
            .SetBottomRightCornerSize(context.ToPixels(bottomRight))
            .Build();
    }

    private static void MapBackground(
        ContentButtonHandler handler,
        ContentButton view)
    {
        var color = (view.BackgroundColor ?? Colors.Transparent).ToPlatform();
        handler.PlatformView.SetCardBackgroundColor(color);

        var context = handler.PlatformView.Context;

        using var array = context?.ObtainStyledAttributes(
            [Attribute.ColorControlHighlight]);

        var rippleColor = array?.GetColorStateList(0);
        handler.PlatformView.RippleColor = rippleColor;
    }

    private static void MapStrokeThickness(ContentButtonHandler handler, ContentButton view)
    {
        handler.PlatformView.StrokeWidth = (int)handler.Context
            .ToPixels(view.StrokeThickness);
    }

    private static void MapStroke(ContentButtonHandler handler, ContentButton view)
    {
        var color = view.Stroke;
        handler.PlatformView.StrokeColor = color.ToPlatform();
    }
}
