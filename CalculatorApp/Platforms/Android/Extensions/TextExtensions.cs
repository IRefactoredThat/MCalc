using Android.Graphics;
using Android.Text;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.Platforms.Android.Extensions;

public class TextExtensions(TypographySettings typographySettings)
{
    private readonly Dictionary<string, TextPaint> _paintCache = new();

    private TextPaint GetPaint(string fontFamily, float textSize)
    {
        if (!_paintCache.TryGetValue(fontFamily, out var paint))
        {
            paint = new TextPaint(PaintFlags.AntiAlias)
            {
                TextSize = textSize,
            };
            paint.SetTypeface(Typeface.Create(fontFamily, TypefaceStyle.Normal));
            _paintCache[fontFamily] = paint;
        }

        paint.TextSize = textSize;
        return paint;
    }

    private double MeasureWidth(string? text, double fontSize, string fontFamily)
    {
        var paint = GetPaint(fontFamily, (float)fontSize);
        return paint.MeasureText(text ?? string.Empty);
    }

    public double BestFontSize(string? text, string fontFamily, double availableWidth)
    {
        var minSize = typographySettings.ScaledInputMinSize;
        var maxSize = typographySettings.ScaledInputMaxSize;

        var bestSize = minSize;

        while (maxSize - minSize > TypographySettings.InputRescaleFactor)
        {
            var midSize = (minSize + maxSize) / 2;

            var measuredWidth = MeasureWidth(text, midSize, fontFamily);

            if (measuredWidth > availableWidth)
            {
                maxSize = midSize;
            }
            else
            {
                bestSize = midSize;
                minSize = midSize;
            }
        }

        return bestSize;
    }
}