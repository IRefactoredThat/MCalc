using Android.Content.Res;
using Android.Views;
using Android.Widget;
using CalculatorApp.Platforms.Android.Alerts;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Typography;
using Google.Android.Material.Dialog;
using Google.Android.Material.Shape;
using Microsoft.Maui.Platform;
using Color = Microsoft.Maui.Graphics.Color;
using AColor = Android.Graphics.Color;
using Orientation = Android.Widget.Orientation;
using ResourceStyle = Android.Resource.Style;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.HueWheel;

public class HueWheelDialog(AppearanceSettings appearanceSettings, TypographySettings typographySettings)
{
    private const double PreviewSize = 48;
    private const double WheelSize = 220;

    public void ShowColorPicker(Color initialColor, Action<Color> onColorSelected)
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
        {
            return;
        }

        var primary = appearanceSettings.GetColorByName(nameof(MaterialScheme.Primary));
        var surface = appearanceSettings.GetColorByName(nameof(MaterialScheme.SurfaceContainerHigh));
        var onSurface = appearanceSettings.GetColorByName(nameof(MaterialScheme.OnSurface));

        var container = new LinearLayout(activity)
        {
            Orientation = Orientation.Vertical
        };
        container.SetPadding(
            (int)activity.ToPixels(24),
            (int)activity.ToPixels(24),
            (int)activity.ToPixels(24),
            (int)activity.ToPixels(24));
        container.SetGravity(GravityFlags.CenterHorizontal);

        var hueView = new HueWheelView(activity)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                (int)activity.ToPixels(WheelSize),
                (int)activity.ToPixels(WheelSize))
        };
        container.AddView(hueView);

        var previewSize = (int)activity.ToPixels(PreviewSize);
        var preview = new View(activity)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                previewSize, previewSize)
            {
                TopMargin = (int)activity.ToPixels(16)
            }
        };

        var previewRadius = activity.ToPixels(24);
        var previewShape = new ShapeAppearanceModel.Builder()
            .SetAllCorners(CornerFamily.Rounded, previewRadius)
            .Build();
        var previewDrawable = new MaterialShapeDrawable(previewShape);
        previewDrawable.FillColor = ColorStateList.ValueOf(surface.ToPlatform());
        preview.Background = previewDrawable;

        container.AddView(preview);

        hueView.ColorChanged += (hue, saturation) =>
        {
            var color = Color.FromHsv(hue, saturation, 1f);
            previewDrawable.FillColor = ColorStateList.ValueOf(color.ToPlatform());
        };

        var initialHsv = new float[3];
        AColor.ColorToHSV(initialColor.ToPlatform(), initialHsv);
        hueView.SetColor(initialHsv[0] / 360f, initialHsv[1]);

        var builder = new MaterialAlertDialogBuilder(activity);
        builder.SetTitle("Pick color");
        builder.SetView(container);
        builder.SetNegativeButton("Cancel", (_, _) => { });
        builder.SetPositiveButton("Select", (_, _) =>
        {
            var selected = Color.FromHsv(hueView.Hue, hueView.Saturation, 1f);
            onColorSelected.Invoke(selected);
        });

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
        DialogFontApplier.Apply(dialog, typographySettings.AppFont);

        var positive = dialog.GetButton(-1);
        var negative = dialog.GetButton(-2);

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

        var titleId = activity.Resources?.GetIdentifier("alertTitle", "id", "android") ?? 0;
        if (titleId != 0 && dialog.FindViewById(titleId) is TextView titleView)
        {
            titleView.SetTextColor(onSurface.ToPlatform());
        }
    }
}
