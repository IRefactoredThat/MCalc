using Android.Content.Res;
using Android.Widget;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Typography;
using Google.Android.Material.Dialog;
using Google.Android.Material.Shape;
using Microsoft.Maui.Platform;
using Id = Android.Resource.Id;
using ResourceStyle = Android.Resource.Style;
using DialogButtonType = Android.Content.DialogButtonType;

namespace CalculatorApp.Platforms.Android.Alerts;

public class ClearHistoryDialog(AppearanceSettings appearanceSettings, TypographySettings typographySettings)
{
    public void ShowClearHistoryConfirmation(Action onConfirm)
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
        {
            return;
        }

        var primary = appearanceSettings.GetColorByName(nameof(MaterialScheme.Primary));
        var surface = appearanceSettings.GetColorByName(nameof(MaterialScheme.SurfaceContainerHigh));
        var onSurface = appearanceSettings.GetColorByName(nameof(MaterialScheme.OnSurface));

        var builder = new MaterialAlertDialogBuilder(activity);
        builder.SetTitle("Clear history");
        builder.SetMessage("Are you sure you want to clear all history?");
        builder.SetNegativeButton("Cancel", (_, _) => { });
        builder.SetPositiveButton("Clear", (_, _) => onConfirm());

        var dialog = builder.Create();
        if (dialog.Window is not null)
        {
            dialog.Window.SetWindowAnimations(ResourceStyle.AnimationDialog);

            var radius = activity.ToPixels(28);
            var shape = new ShapeAppearanceModel.Builder()
                .SetAllCorners(CornerFamily.Rounded, radius)
                .Build();

            var drawable = new MaterialShapeDrawable(shape);
            drawable.FillColor = ColorStateList.ValueOf(surface.ToPlatform());
            dialog.Window.SetBackgroundDrawable(drawable);
        }

        dialog.Show();
        DialogFontApplier.Apply(dialog, typographySettings);

        var positive = dialog.GetButton((int)DialogButtonType.Positive);
        var negative = dialog.GetButton((int)DialogButtonType.Negative);

        if (positive is not null)
        {
            positive.SetTextColor(primary.ToPlatform());
            positive.SetAllCaps(false);
        }
        if (negative is not null)
        {
            negative.SetTextColor(onSurface.ToPlatform());
            negative.SetAllCaps(false);
        }

        var titleId = activity.Resources?.GetIdentifier("alertTitle", "id", "android");
        if (titleId.HasValue && titleId.Value != 0 &&
            dialog.FindViewById<TextView>(titleId.Value) is { } title)
        {
            title.SetTextColor(onSurface.ToPlatform());
        }

        if (dialog.FindViewById<TextView>(Id.Message) is { } message)
        {
            message.SetTextColor(onSurface.ToPlatform());
        }
    }
}
