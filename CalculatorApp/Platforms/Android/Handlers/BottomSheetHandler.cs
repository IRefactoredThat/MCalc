using Android.Graphics.Drawables;
using Microsoft.Maui.Platform;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.View;
using CalculatorApp.SharedElements.BottomSheet;
using Google.Android.Material.BottomSheet;
using Microsoft.Maui.Handlers;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.Handlers;

internal sealed class BottomSheetStateCallback(BottomSheetHandler handler)
    : BottomSheetBehavior.BottomSheetCallback
{
    public override void OnStateChanged(View bottomSheet, int newState)
    {
        switch (newState)
        {
            case BottomSheetBehavior.StateHidden:
                if (handler.VirtualView.IsOpen)
                {
                    handler.VirtualView.IsOpen = false;
                }
                break;

            case BottomSheetBehavior.StateExpanded:
                if (!handler.VirtualView.IsOpen)
                {
                    handler.VirtualView.IsOpen = true;
                }
                break;
        }
    }

    public override void OnSlide(View bottomSheet, float slideOffset) { }
}

public class BottomSheetHandler()
    : ViewHandler<BottomSheet, CoordinatorLayout>(Mapper)
{
    private BottomSheetBehavior? _behavior;
    private GradientDrawable? _drawable;
    private BottomSheetBehavior.BottomSheetCallback? _callback;

    private static readonly IPropertyMapper<BottomSheet, BottomSheetHandler> Mapper =
        new PropertyMapper<BottomSheet, BottomSheetHandler>(ViewMapper)
        {
            [nameof(BottomSheet.Content)] = MapContent,
            [nameof(BottomSheet.CornerRadius)] = MapCorners,
            [nameof(BottomSheet.IsOpen)] = MapIsOpen,
            [nameof(BottomSheet.Background)] = MapBackground
        };

    protected override CoordinatorLayout CreatePlatformView()
    {
        var coordinator = new CoordinatorLayout(Context);

        coordinator.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

        _behavior = new BottomSheetBehavior
        {
            Hideable = true,
            SkipCollapsed = true
        };

        _callback = new BottomSheetStateCallback(this);
        _behavior.AddBottomSheetCallback(_callback);

        return coordinator;
    }

    protected override void DisconnectHandler(CoordinatorLayout platformView)
    {
        if (_behavior != null && _callback != null)
        {
            _behavior.RemoveBottomSheetCallback(_callback);
        }

        base.DisconnectHandler(platformView);
    }

    private View? _content;

    public static void MapContent(BottomSheetHandler handler, BottomSheet view)
    {
        var virtualContent = view.Content;

        if (virtualContent.Handler is null)
        {
            virtualContent.ToHandler(handler.MauiContext!);
        }

        if (virtualContent.Handler?.PlatformView is not View content)
        {
            return;
        }

        handler._content = content;

        if (content.Parent is ViewGroup oldParent)
        {
            oldParent.RemoveView(content);
        }

        var coordinator = handler.PlatformView;
        coordinator.RemoveAllViews();

        content.Background = handler._drawable;

        coordinator.AddView(content);
        coordinator.Post(() =>
        {
            content.LayoutParameters = new CoordinatorLayout.LayoutParams(
                coordinator.Width, coordinator.Height)
            {
                Behavior = handler._behavior
            };
            handler._behavior?.State = GetNextState(view);
        });
    }

    private static void MapBackground(BottomSheetHandler handler, BottomSheet view)
    {
        handler._drawable = new GradientDrawable();
        handler._drawable.SetColor(view.BackgroundColor.ToPlatform());
    }

    private static void MapCorners(BottomSheetHandler handler, BottomSheet view)
    {
        var context = handler.Context;
        var radius = view.CornerRadius;

        handler._drawable?.SetCornerRadii(
        [
            context.ToPixels(radius.TopLeft),
            context.ToPixels(radius.TopLeft),

            context.ToPixels(radius.TopRight),
            context.ToPixels(radius.TopRight),

            context.ToPixels(radius.BottomRight),
            context.ToPixels(radius.BottomRight),

            context.ToPixels(radius.BottomLeft),
            context.ToPixels(radius.BottomLeft)
        ]);
    }

    public static void MapIsOpen(BottomSheetHandler handler, BottomSheet view)
    {
        if (handler._behavior is null || handler._content is null)
        {
            return;
        }

        var desiredState = GetNextState(view);

        if (handler._behavior.State != desiredState)
        {
            handler._behavior.State = desiredState;
        }

        if (desiredState is BottomSheetBehavior.StateExpanded)
        {
            var content = handler._content;
            var inset = ViewCompat.GetRootWindowInsets(handler.PlatformView)?
                .GetInsets(WindowInsetsCompat.Type.SystemBars())?.Bottom ?? 0;
            if (handler.PlatformView.PaddingBottom != inset)
            {
                content.SetPadding(
                    content.PaddingLeft, content.PaddingTop,
                    content.PaddingRight, inset);
            }
        }
    }

    private static int GetNextState(BottomSheet view) => view.IsOpen
        ? BottomSheetBehavior.StateExpanded
        : BottomSheetBehavior.StateHidden;
}