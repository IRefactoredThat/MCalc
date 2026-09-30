using Android.Content;
using Android.Views;
using Google.Android.Material.Card;
using Microsoft.Maui.Platform;
using Rectangle = Microsoft.Maui.Graphics.Rect;

namespace CalculatorApp.Platforms.Android.Handlers;

public sealed class ContentCardView(Context context) : MaterialCardView(context)
{
    private readonly Context _context = context;

    public ICrossPlatformLayout? CrossPlatformLayout { get; init; }

    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        if (CrossPlatformLayout is null)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            return;
        }

        var deviceIndependentWidth = widthMeasureSpec.ToDouble(_context);
        var deviceIndependentHeight = heightMeasureSpec.ToDouble(_context);

        var paddingLeft = _context.FromPixels(PaddingLeft);
        var paddingTop = _context.FromPixels(PaddingTop);
        var paddingRight = _context.FromPixels(PaddingRight);
        var paddingBottom = _context.FromPixels(PaddingBottom);

        var availableWidth = Math.Max(0, deviceIndependentWidth - paddingLeft - paddingRight);
        var availableHeight = Math.Max(0, deviceIndependentHeight - paddingTop - paddingBottom);

        var widthMode = MeasureSpec.GetMode(widthMeasureSpec);
        var heightMode = MeasureSpec.GetMode(heightMeasureSpec);

        var measure = CrossPlatformLayout.CrossPlatformMeasure(availableWidth, availableHeight);

        var width = widthMode == MeasureSpecMode.Exactly
            ? deviceIndependentWidth
            : measure.Width + paddingLeft + paddingRight;
        var height = heightMode == MeasureSpecMode.Exactly
            ? deviceIndependentHeight
            : measure.Height + paddingTop + paddingBottom;

        var platformWidth = _context.ToPixels(width);
        var platformHeight = _context.ToPixels(height);

        platformWidth = Math.Max(MinimumWidth, platformWidth);
        platformHeight = Math.Max(MinimumHeight, platformHeight);

        SetMeasuredDimension((int)platformWidth, (int)platformHeight);
    }

    protected override void OnLayout(bool changed, int l, int t, int r, int b)
    {
        if (CrossPlatformLayout is null)
        {
            base.OnLayout(changed, l, t, r, b);
            return;
        }

        var destination = _context.ToCrossPlatformRectInReferenceFrame(l, t, r, b);

        var paddingLeft = _context.FromPixels(PaddingLeft);
        var paddingTop = _context.FromPixels(PaddingTop);
        var paddingRight = _context.FromPixels(PaddingRight);
        var paddingBottom = _context.FromPixels(PaddingBottom);

        destination = new Rectangle(
            destination.X + paddingLeft,
            destination.Y + paddingTop,
            Math.Max(0, destination.Width - paddingLeft - paddingRight),
            Math.Max(0, destination.Height - paddingTop - paddingBottom));

        CrossPlatformLayout.CrossPlatformArrange(destination);
    }
}
