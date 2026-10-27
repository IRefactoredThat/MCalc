using Android.App;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Platform;
using Application = Microsoft.Maui.Controls.Application;
using Font = Microsoft.Maui.Font;
using TypefaceStyle = Android.Graphics.TypefaceStyle;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.Alerts;

public static class DialogFontApplier
{
    public static void Apply(Dialog? dialog, string fontFamily)
    {
        if (dialog?.Window?.DecorView is not { } decorView)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(fontFamily))
        {
            return;
        }

        var fontManager = Application.Current?.Handler?.MauiContext?.Services
            .GetService<IFontManager>();
        if (fontManager is null)
        {
            return;
        }

        ApplyToView(decorView, fontManager, fontFamily);
    }

    private static void ApplyToView(
        View view, IFontManager fontManager, string fontFamily)
    {
        if (view is TextView textView)
        {
            var typeface = fontManager.GetTypeface(Font.OfSize(fontFamily,
                view.Context.FromPixels(textView.TextSize)));
            var style = textView.Typeface?.Style ?? TypefaceStyle.Normal;
            textView.SetTypeface(typeface, style);
        }

        if (view is not ViewGroup viewGroup)
        {
            return;
        }

        for (var i = 0; i < viewGroup.ChildCount; i++)
        {
            if (viewGroup.GetChildAt(i) is { } child)
            {
                ApplyToView(child, fontManager, fontFamily);
            }
        }
    }
}
