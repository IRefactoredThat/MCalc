using Android.Content;
using Android.Graphics;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.HueWheel;

public sealed class IndicatorView : View
{
    public const float StrokeWidth = 6f;
    public const float Radius = 16f;

    private readonly Paint _fillPaint = new();
    private readonly Paint _strokePaint = new();

    public IndicatorView(Context context) : base(context)
    {
        _fillPaint.AntiAlias = true;
        _fillPaint.SetStyle(Paint.Style.Fill);

        _strokePaint.AntiAlias = true;
        _strokePaint.SetStyle(Paint.Style.Stroke);
        _strokePaint.StrokeWidth = StrokeWidth;
    }

    public void SetColors(Color fillColor, Color strokeColor)
    {
        _fillPaint.Color = fillColor;
        _strokePaint.Color = strokeColor;
        Invalidate();
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        var cx = Width / 2f;
        var cy = Height / 2f;
        canvas.DrawCircle(cx, cy, Radius, _fillPaint);
        canvas.DrawCircle(cx, cy, Radius, _strokePaint);
    }
}
