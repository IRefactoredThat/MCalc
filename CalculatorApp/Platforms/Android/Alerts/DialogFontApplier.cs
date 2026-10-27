using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CalculatorApp.SettingsPage.Typography;
using Application = Microsoft.Maui.Controls.Application;
using Font = Microsoft.Maui.Font;
using TypefaceStyle = Android.Graphics.TypefaceStyle;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.Alerts;

public static class DialogFontApplier
{
    public static void Apply(
        Dialog? dialog,
        TypographySettings typographySettings)
    {
        if (dialog?.Window?.DecorView is not { } decorView ||
            Application.Current?.Handler?.MauiContext?
                .Services.GetService<IFontManager>() is not { } fontManager)
        {
            return;
        }

        ApplyToView(decorView, fontManager, typographySettings);
    }

    private static void ApplyToView(
        View view,
        IFontManager fontManager,
        TypographySettings typographySettings)
    {
        if (view is TextView textView)
        {
            var isTitle = IsDialogTitle(textView);
            var fontFamily = typographySettings.AppFont;
            var fontSize = isTitle ? typographySettings.DialogTitleScale * TypographySettings.DialogTitleSize
                : typographySettings.DialogTextScale * TypographySettings.DialogTextSize;

            var typeface = fontManager.GetTypeface(Font.OfSize(fontFamily, fontSize));
            var style = textView.Typeface?.Style ?? TypefaceStyle.Normal;

            textView.SetTypeface(typeface, style);
            textView.SetTextSize(ComplexUnitType.Sp, (float)fontSize);
        }

        if (view is not ViewGroup viewGroup)
        {
            return;
        }

        for (var i = 0; i < viewGroup.ChildCount; i++)
        {
            if (viewGroup.GetChildAt(i) is { } child)
            {
                ApplyToView(child, fontManager, typographySettings);
            }
        }
    }

    private static bool IsDialogTitle(View view) =>
        view.Id != View.NoId && view.Resources?
            .GetResourceEntryName(view.Id) == "alertTitle";
}
