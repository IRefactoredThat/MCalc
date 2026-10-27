using Android.Graphics;
using Android.Views;
using Microsoft.Maui.Platform;
using AColor = Android.Graphics.Color;
using Color = Microsoft.Maui.Graphics.Color;
using Paint = Android.Graphics.Paint;
using Context = Android.Content.Context;
using RectF = Android.Graphics.RectF;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.HueWheel;

public sealed class WheelView : View
{
    private const int HueSteps = 360;
    private const float Inset = 30f;

    private const float RingStartAngle = 135f;
    private const float RingSweepAngle = 270f;

    private enum DragTarget { None, Wheel, Ring }

    private readonly Paint _paint = new();
    private readonly Paint _ringPaint = new();
    private readonly Paint _thumbFillPaint = new();
    private readonly Paint _thumbStrokePaint = new();
    private readonly RectF _ringRect = new();
    private readonly float[] _hsv = new float[3];

    private Shader? _hueShader, _saturationShader, _ringShader;
    private float _x, _y, _radius;
    private float _ringRadius;
    private readonly float _ringWidth, _ringGap, _touchSlop, _thumbRadius;

    private DragTarget _drag = DragTarget.None;
    private float _hue, _saturation = 1f, _value = 1f;

    private static readonly int[] HueColors = CreateHueColors();

    public Color MauiColor => Color.FromHsv(_hue / 360f, _saturation, _value);

    public WheelView(Context context) : base(context)
    {
        _ringWidth = context.ToPixels(10f);
        _ringGap = context.ToPixels(14f);
        _touchSlop = context.ToPixels(14f);
        _thumbRadius = context.ToPixels(11f);

        _paint.AntiAlias = true;
        _paint.SetStyle(Paint.Style.Fill);

        _ringPaint.AntiAlias = true;
        _ringPaint.SetStyle(Paint.Style.Stroke!);
        _ringPaint.StrokeWidth = _ringWidth;
        _ringPaint.StrokeCap = Paint.Cap.Round!;

        _thumbFillPaint.AntiAlias = true;
        _thumbFillPaint.SetStyle(Paint.Style.Fill);

        _thumbStrokePaint.AntiAlias = true;
        _thumbStrokePaint.SetStyle(Paint.Style.Stroke!);
        _thumbStrokePaint.StrokeWidth = context.ToPixels(2.5f);
        _thumbStrokePaint.Color = AColor.White;
    }

    private static int[] CreateHueColors()
    {
        var colors = new int[HueSteps + 1];
        var hsv = new float[3];
        hsv[1] = 1f;
        hsv[2] = 1f;

        for (var i = 0; i <= HueSteps; i++)
        {
            hsv[0] = i;
            colors[i] = AColor.HSVToColor(hsv);
        }

        return colors;
    }

    protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
    {
        base.OnSizeChanged(w, h, oldw, oldh);

        _x = w / 2f;
        _y = h / 2f;

        // Outer edge of everything, same Inset as before.
        var outer = float.Max(float.Min(w, h) / 2f - Inset, 1f);

        // Ring sits on the outside, the wheel shrinks to make room for it.
        _ringRadius = float.Max(outer - _ringWidth / 2f, 1f);
        _radius = float.Max(outer - _ringWidth - _ringGap, 1f);
        _ringRect.Set(_x - _ringRadius, _y - _ringRadius, _x + _ringRadius, _y + _ringRadius);

        _hueShader?.Dispose();
        _hueShader = new SweepGradient(_x, _y, HueColors, null);

        _saturationShader?.Dispose();
        _saturationShader = new RadialGradient(
            _x,
            _y,
            _radius,
            AColor.White,
            AColor.Argb(0, 255, 255, 255),
            Shader.TileMode.Clamp!);

        UpdateRingShader();
    }

    private void UpdateRingShader()
    {
        if (_x <= 0f) return;

        _hsv[0] = _hue;
        _hsv[1] = _saturation;
        _hsv[2] = 1f;
        var full = AColor.HSVToColor(_hsv);

        var capDeg = _ringWidth / 2f / _ringRadius * 180f / MathF.PI;
        var capFrac = capDeg / 360f;
        var sweepFrac = RingSweepAngle / 360f;

        _ringShader?.Dispose();
        var shader = new SweepGradient(
            _x,
            _y,
            [AColor.Black, AColor.Black, full, full],
            [0f, capFrac, capFrac + sweepFrac, 1f]);

        using (var m = new Matrix())
        {
            m.SetRotate(RingStartAngle - capDeg, _x, _y);
            shader.SetLocalMatrix(m);
        }

        _ringShader = shader;
        _ringPaint.SetShader(_ringShader);
    }

    private ColorFilter? _wheelFilter;

    private void UpdateWheelFilter()
    {
        var g = (int)MathF.Round(_value * 255f);
        _wheelFilter?.Dispose();
        _wheelFilter = new LightingColorFilter(AColor.Rgb(g, g, g), AColor.Black);
    }

    private void OnHsvChanged()
    {
        UpdateWheelFilter();
        UpdateRingShader();
        Invalidate();
    }

    public void SetColor(Color mauiColor)
    {
        AColor.ColorToHSV(mauiColor.ToPlatform(), _hsv);
        _hue = _hsv[0];
        _saturation = _hsv[1];
        _value = _hsv[2];
        OnHsvChanged();
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        // --- wheel (RGB scaled by value) ---
        _paint.SetColorFilter(_wheelFilter);

        _paint.SetShader(_hueShader);
        canvas.DrawCircle(_x, _y, _radius, _paint);

        _paint.SetShader(_saturationShader);
        canvas.DrawCircle(_x, _y, _radius, _paint);

        _paint.SetColorFilter(null);
        _paint.SetShader(null);

        // --- value ring (outline only) ---
        canvas.DrawArc(_ringRect, RingStartAngle, RingSweepAngle, false, _ringPaint);

        // --- thumbs ---
        _hsv[0] = _hue;
        _hsv[1] = _saturation;
        _hsv[2] = _value;
        _thumbFillPaint.Color = AColor.HSVToColor(_hsv);

        // wheel thumb
        var hueRad = _hue * MathF.PI / 180f;
        var wx = _x + MathF.Cos(hueRad) * _saturation * _radius;
        var wy = _y + MathF.Sin(hueRad) * _saturation * _radius;
        canvas.DrawCircle(wx, wy, _thumbRadius * 0.8f, _thumbFillPaint);
        canvas.DrawCircle(wx, wy, _thumbRadius * 0.8f, _thumbStrokePaint);

        // ring thumb
        var ringRad = (RingStartAngle + _value * RingSweepAngle) * MathF.PI / 180f;
        var rx = _x + MathF.Cos(ringRad) * _ringRadius;
        var ry = _y + MathF.Sin(ringRad) * _ringRadius;
        canvas.DrawCircle(rx, ry, _thumbRadius, _thumbFillPaint);
        canvas.DrawCircle(rx, ry, _thumbRadius, _thumbStrokePaint);
    }

    public override bool OnTouchEvent(MotionEvent? e)
    {
        if (e is null) return false;

        var dx = e.GetX() - _x;
        var dy = e.GetY() - _y;
        var dist = MathF.Sqrt(dx * dx + dy * dy);

        switch (e.ActionMasked)
        {
            case MotionEventActions.Down:
                // Decide the target once and keep it until the finger lifts,
                // so dragging across the boundary doesn't switch controls.
                if (MathF.Abs(dist - _ringRadius) <= _ringWidth / 2f + _touchSlop)
                    _drag = DragTarget.Ring;
                else if (dist <= _radius)
                    _drag = DragTarget.Wheel;
                else
                    _drag = DragTarget.None;

                if (_drag == DragTarget.None) return false;
                Parent?.RequestDisallowInterceptTouchEvent(true);
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                _drag = DragTarget.None;
                return true;
        }

        switch (_drag)
        {
            case DragTarget.Wheel:
                _hue = AngleDegrees(dx, dy);
                _saturation = Math.Clamp(dist / _radius, 0f, 1f);
                break;

            case DragTarget.Ring:
                var rel = (AngleDegrees(dx, dy) - RingStartAngle + 360f) % 360f;
                if (rel > RingSweepAngle)
                {
                    // Finger is in the gap at the bottom: snap to the nearer end.
                    rel = rel - RingSweepAngle < (360f - RingSweepAngle) / 2f ? RingSweepAngle : 0f;
                }
                _value = rel / RingSweepAngle;
                break;

            default:
                return false;
        }

        OnHsvChanged();
        return true;
    }

    private static float AngleDegrees(float dx, float dy)
    {
        var deg = MathF.Atan2(dy, dx) * 180f / MathF.PI;
        return deg < 0f ? deg + 360f : deg;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _wheelFilter?.Dispose();
            _hueShader?.Dispose();
            _saturationShader?.Dispose();
            _ringShader?.Dispose();
            _paint.Dispose();
            _ringPaint.Dispose();
            _thumbFillPaint.Dispose();
            _thumbStrokePaint.Dispose();
            _ringRect.Dispose();
        }

        base.Dispose(disposing);
    }
}
