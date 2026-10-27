using Android.Graphics;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using Context = Android.Content.Context;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.HueWheel;

public sealed class WheelView : View
{
    private const int HueSteps = 360;
    public const float Inset = 30f;

    private readonly Paint _paint = new();
    private Shader? _hueShader, _saturationShader;
    private float _x, _y, _radius;
    private static readonly int[] HueColors = CreateHueColors();

    public WheelView(Context context) : base(context)
    {
        _paint.AntiAlias = true;
        _paint.SetStyle(Paint.Style.Fill);
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
            colors[i] = Color.HSVToColor(hsv);
        }

        return colors;
    }

    protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
    {
        base.OnSizeChanged(w, h, oldw, oldh);

        _x = w / 2f;
        _y = h / 2f;
        _radius = float.Max(float.Min(w, h) / 2f - Inset, 1f);

        _hueShader?.Dispose();
        _hueShader = new SweepGradient(_x, _y, HueColors, null);

        _saturationShader?.Dispose();
        _saturationShader = new RadialGradient(
            _x,
            _y,
            _radius,
            Color.White,
            Color.Argb(0, 255, 255, 255),
            Shader.TileMode.Clamp!);
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        _paint.SetShader(_hueShader);
        canvas.DrawCircle(_x, _y, _radius, _paint);

        _paint.SetShader(_saturationShader);
        canvas.DrawCircle(_x, _y, _radius, _paint);
    }
}
