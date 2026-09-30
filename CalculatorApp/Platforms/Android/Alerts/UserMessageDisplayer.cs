using _Microsoft.Android.Resource.Designer;
using Android.Util;
using Android.Widget;
using CalculatorApp.SettingsPage.Typography;
using Google.Android.Material.Snackbar;
using Microsoft.Maui.Platform;
using Font = Microsoft.Maui.Font;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.Alerts;

public static class UserMessageDisplayer
{
    private sealed class SnackbarActionClickListener(Action? action) : Java.Lang.Object, View.IOnClickListener
    {
        public void OnClick(View? v) => action?.Invoke();
    }

    // Relying on snackbar tinting since Material Flag in project in combination
    // with MAUI's way of coloring (the MaterialDrawable) doesn't allow for customization
    private static Snackbar? _platformSnackbar;

    public static void ShowAndroidMessage(string message, Color backgroundColor,
        Color textColor, Action? action, Color actionTextColor, string fontFamily, int duration)
    {
        _platformSnackbar?.Dismiss();

        var parentView = Platform.CurrentActivity?.Window?.DecorView
            .FindViewById(global::Android.Resource.Id.Content)
            ?? throw new InvalidOperationException("Unable to create snackbar");

        _platformSnackbar = Snackbar.Make(parentView, message, duration);

        _platformSnackbar.SetBackgroundTint(backgroundColor.ToPlatform());
        _platformSnackbar.SetTextColor(textColor.ToPlatform());
        _platformSnackbar.SetActionTextColor(actionTextColor.ToPlatform());
        _platformSnackbar.SetAction("OK", new SnackbarActionClickListener(action));

        var fontManager = Application.Current?.Handler?.MauiContext?.Services
            .GetRequiredService<IFontManager>()
            ?? throw new InvalidOperationException($"{nameof(IFontManager)} Required");

        var fontSize = (float)TypographySettings.GetCurrentSize(
            nameof(TypographySettings.SnackbarSize), TypographySettings.SnackbarSize);
        var platformFont = fontManager.GetTypeface(Font.OfSize(fontFamily, fontSize));

        var messageView = _platformSnackbar.View.FindViewById<TextView>(ResourceConstant.Id.snackbar_text);
        if (messageView is not null)
        {
            messageView.SetMaxLines(10);
            messageView.SetTextSize(ComplexUnitType.Dip, fontSize);
            messageView.SetTypeface(platformFont, global::Android.Graphics.TypefaceStyle.Normal);
        }

        var actionView = _platformSnackbar.View.FindViewById<TextView>(ResourceConstant.Id.snackbar_action);
        if (actionView is not null)
        {
            actionView.SetTextSize(ComplexUnitType.Dip, fontSize);
            actionView.SetTypeface(platformFont, global::Android.Graphics.TypefaceStyle.Normal);
        }

        SetLayoutParametersForView(_platformSnackbar.View);

        _platformSnackbar.Show();
    }

    static void SetLayoutParametersForView(View snackbarView)
    {
        if (Application.Current?.Windows[0].Page is not { } mainPage
            || mainPage.Navigation.ModalStack.Count == 0
            || snackbarView.Context?.Resources is null)
        {
            return;
        }

        var resourceId = snackbarView.Context.Resources.GetIdentifier(
            "navigation_bar_height", "dimen", "android");
        var navBarHeight = snackbarView.Context.Resources.GetDimensionPixelSize(resourceId);
        if (snackbarView.LayoutParameters is FrameLayout.LayoutParams layoutParameters)
        {
            layoutParameters.SetMargins(layoutParameters.LeftMargin, layoutParameters.TopMargin,
                layoutParameters.RightMargin, layoutParameters.BottomMargin + navBarHeight);
            snackbarView.LayoutParameters = layoutParameters;
        }
    }
}
