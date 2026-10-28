using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CalculatorApp.SettingsPage.App;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Typography;
using Google.Android.Material.Dialog;
using Google.Android.Material.Shape;
using Microsoft.Maui.Platform;
using Color = Microsoft.Maui.Graphics.Color;
using DialogButtonType = Android.Content.DialogButtonType;
using Orientation = Android.Widget.Orientation;
using ResourceStyle = Android.Resource.Style;

namespace CalculatorApp.Platforms.Android.Alerts;

public class ExceptionLogDialog(
    AppSettings appSettings,
    AppearanceSettings appearanceSettings,
    TypographySettings typographySettings)
{
    private const double MaxListHeight = 360;

    public void ShowExceptionLog()
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
        var error = appearanceSettings.GetColorByName(nameof(MaterialScheme.Error));
        var entrySurface = appearanceSettings.GetColorByName(nameof(MaterialScheme.SurfaceContainer));

        var exceptions = appSettings.Exceptions.ToList();
        var container = new LinearLayout(activity)
        {
            Orientation = Orientation.Vertical
        };
        container.SetPadding(
            (int)activity.ToPixels(24),
            (int)activity.ToPixels(8),
            (int)activity.ToPixels(24),
            (int)activity.ToPixels(8));

        RecyclerView? recycler = null;
        ExceptionAdapter? adapter = null;

        if (exceptions.Count == 0)
        {
            container.AddView(CreateEmptyView(activity, onSurfaceVariant));
        }
        else
        {
            var maxListHeight = Math.Min(
                (int)activity.ToPixels(MaxListHeight),
                (int)((activity.Resources?.DisplayMetrics?.HeightPixels ?? 0) * 0.5));
            recycler = new BoundedRecyclerView(activity, maxListHeight)
            {
                VerticalScrollBarEnabled = false,
                OverScrollMode = OverScrollMode.IfContentScrolls
            };
            recycler.SetLayoutManager(new LinearLayoutManager(activity));
            adapter = new ExceptionAdapter(
                activity,
                exceptions,
                CopyException,
                typographySettings,
                entrySurface,
                error,
                onSurfaceVariant);
            container.AddView(recycler);
        }

        var builder = new MaterialAlertDialogBuilder(activity);
        builder.SetTitle("Exception log");
        builder.SetView(container);
        builder.SetNegativeButton("Close", (_, _) => { });
        builder.SetPositiveButton("Copy", (_, _) =>
            appSettings.CopyErrorsCommand.Execute(null));

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

        if (recycler is not null && adapter is not null)
        {
            recycler.SetAdapter(adapter);
            recycler.ScrollToPosition(exceptions.Count - 1);
        }

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
    }

    private static TextView CreateEmptyView(Activity activity, Color onSurfaceVariant)
    {
        var empty = new TextView(activity)
        {
            Text = "No exceptions were thrown",
            Gravity = GravityFlags.Center
        };
        empty.SetTextColor(onSurfaceVariant.ToPlatform());
        empty.SetPadding(0, (int)activity.ToPixels(24), 0, (int)activity.ToPixels(24));
        return empty;
    }

    private static void CopyException(CapturedException exception)
    {
        var text = $"{exception.Time:yyyy-MM-dd HH:mm:ss}" +
            $"{Environment.NewLine}{exception.Message}" +
            $"{Environment.NewLine}{exception.StackTrace}";
        _ = Clipboard.SetTextAsync(text);
    }

    private sealed class BoundedRecyclerView(Context context, int maxHeight)
        : RecyclerView(context)
    {
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var mode = MeasureSpec.GetMode(heightMeasureSpec);
            var height = MeasureSpec.GetSize(heightMeasureSpec);

            if (mode == MeasureSpecMode.Unspecified ||
                (mode == MeasureSpecMode.AtMost && height > maxHeight))
            {
                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(
                    maxHeight, MeasureSpecMode.AtMost);
            }

            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}
