using Android.Content.Res;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CalculatorApp.Platforms.Android.Alerts;
using CalculatorApp.Platforms.Android.Handlers;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Typography;
using Google.Android.Material.Dialog;
using Google.Android.Material.Shape;
using Microsoft.Maui.Platform;
using Color = Microsoft.Maui.Graphics.Color;
using Orientation = Android.Widget.Orientation;
using ResourceStyle = Android.Resource.Style;

namespace CalculatorApp.Platforms.Android.ColorWheel;

public class ColorWheelDialog(AppearanceSettings appearanceSettings, TypographySettings typographySettings)
{
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
        var onSurfaceVariant = appearanceSettings.GetColorByName(nameof(MaterialScheme.OnSurfaceVariant));
        var outline = appearanceSettings.GetColorByName(nameof(MaterialScheme.Outline));

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

        var hueView = new ColorWheelView(activity)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                (int)activity.ToPixels(WheelSize),
                (int)activity.ToPixels(WheelSize))
        };
        container.AddView(hueView);

        hueView.SetColor(initialColor);

        var hexRow = new LinearLayout(activity)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.Center
            }
        };
        hexRow.SetGravity(GravityFlags.Center);

        var hashPrefix = new TextView(activity)
        {
            Text = "#",
            Gravity = GravityFlags.Center
        };
        hashPrefix.SetTextColor(primary.ToPlatform());
        hexRow.AddView(hashPrefix);

        var initialHex = initialColor.ToHex();
        var input = new CursorAwareEditText(activity)
        {
            Text = initialHex,
            InputType = InputTypes.ClassText |
                        InputTypes.TextFlagCapCharacters |
                        InputTypes.TextFlagNoSuggestions,
            Gravity = GravityFlags.Center,
        };
        input.SetEms(6);
        input.SetFilters([new InputFilterLengthFilter(6)]);
        input.SetCursorColor(outline);
        input.SetTextColor(onSurface.ToPlatform());
        input.SetHintTextColor(onSurfaceVariant.ToPlatform());
        input.BackgroundTintList = ColorStateList.ValueOf(outline.ToPlatform());
        hexRow.AddView(input);
        container.AddView(hexRow);

        var syncingHex = false;
        hueView.ColorChanged += color =>
        {
            var hex = color.ToHex();
            if (input.Text == hex || input.EditableText is null)
            {
                return;
            }
            syncingHex = true;
            input.EditableText.Replace(0, input.Text.Length, hex);
            syncingHex = false;
        };
        input.TextChanged += (_, args) =>
        {
            if (syncingHex)
            {
                return;
            }

            var hex = args.Text?.ToString();
            if (hex is null || hex.Length != 6 || !hex.All(char.IsAsciiHexDigit))
            {
                return;
            }
            hueView.SetColor(Color.FromArgb($"#{hex}"));
        };

        var builder = new MaterialAlertDialogBuilder(activity);
        builder.SetTitle("Pick color");
        builder.SetView(container);
        builder.SetNegativeButton("Cancel", (_, _) => { });
        builder.SetPositiveButton("Select", (_, _) =>
        {
            onColorSelected.Invoke(hueView.MauiColor);
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
        DialogFontApplier.Apply(dialog, typographySettings);
        var titleSize = (float)(typographySettings.DialogTitleScale * TypographySettings.DialogTitleSize);
        hashPrefix.SetTextSize(ComplexUnitType.Sp, titleSize);
        input.SetTextSize(ComplexUnitType.Sp, titleSize);

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
