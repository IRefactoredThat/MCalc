using Android.Views;
using Android.Widget;
using Color = Android.Graphics.Color;
using Context = Android.Content.Context;

namespace CalculatorApp.Platforms.Android.HueWheel;

public sealed class HueWheelView : FrameLayout
{
    private const float TouchSlop = 20f;
    private const float DegreesPerRadian = 180f / MathF.PI;
    private const float IndicatorExtent = IndicatorView.Radius + IndicatorView.StrokeWidth / 2f + 2f;
    private const int IndicatorSize = (int)(IndicatorExtent * 2f);

    public float Hue { get; private set; }
    public float Saturation { get; private set; } = 1f;
    public event Action<float, float>? ColorChanged;

    private readonly IndicatorView _indicatorView;
    private readonly float[] _indicatorHsv = new float[3];
    private float _x;
    private float _y;
    private float _radius;
    private float _inverseRadius;
    private float _maxTouchDistanceSquared;

    public HueWheelView(Context context) : base(context)
    {
        var wheelView = new WheelView(context);
        AddView(wheelView, new LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent));

        _indicatorView = new IndicatorView(context);
        AddView(_indicatorView, new LayoutParams(IndicatorSize, IndicatorSize));
    }

    public void SetColor(float hue, float saturation)
    {
        Hue = ((hue % 1f) + 1f) % 1f;
        Saturation = float.Clamp(saturation, 0f, 1f);
        ColorChanged?.Invoke(Hue, Saturation);
        UpdateIndicator();
    }

    protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
    {
        base.OnSizeChanged(w, h, oldw, oldh);

        _x = w / 2f;
        _y = h / 2f;
        _radius = float.Max(float.Min(w, h) / 2f - WheelView.Inset, 0f);
        _inverseRadius = _radius > 0f ? 1f / _radius : 0f;

        var maxTouchDistance = _radius + TouchSlop;
        _maxTouchDistanceSquared = maxTouchDistance * maxTouchDistance;

        UpdateIndicator();
    }

    public override bool OnTouchEvent(MotionEvent? e)
    {
        if (e is not { Action: MotionEventActions.Down or MotionEventActions.Move })
        {
            return base.OnTouchEvent(e);
        }

        var dx = e.GetX() - _x;
        var dy = e.GetY() - _y;
        var distanceSquared = dx * dx + dy * dy;

        if (distanceSquared > _maxTouchDistanceSquared)
        {
            return false;
        }

        var angle = float.Atan2(dy, dx) * DegreesPerRadian;
        if (angle < 0f)
        {
            angle += 360f;
        }

        var saturation = float.Clamp(float.Sqrt(distanceSquared) * _inverseRadius, 0f, 1f);
        SetColor(angle / 360f, saturation);
        return true;
    }

    private void UpdateIndicator()
    {
        var hueDegrees = Hue * 360f;
        var angle = hueDegrees / DegreesPerRadian;
        var distance = _radius * Saturation;
        var (sin, cos) = float.SinCos(angle);

        _indicatorView.TranslationX = _x + distance * cos - IndicatorSize / 2f;
        _indicatorView.TranslationY = _y + distance * sin - IndicatorSize / 2f;

        _indicatorHsv[0] = hueDegrees;
        _indicatorHsv[1] = Saturation;
        _indicatorHsv[2] = 1f;

        var selectedColor = Color.HSVToColor(_indicatorHsv);
        _indicatorView.SetColors(selectedColor, Color.Black);
    }
}
