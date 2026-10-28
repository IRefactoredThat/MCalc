using Android.Content;
using Android.Content.Res;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CalculatorApp.SettingsPage.App;
using CalculatorApp.SettingsPage.Typography;
using Google.Android.Material.Shape;
using Microsoft.Maui.Platform;
using Orientation = Android.Widget.Orientation;
using View = Android.Views.View;

namespace CalculatorApp.Platforms.Android.Alerts;

public sealed class ExceptionViewHolder(
    View itemView,
    TextView time,
    TextView message,
    TextView stack)
    : RecyclerView.ViewHolder(itemView)
{
    public CapturedException? Exception { get; set; }
    public TextView Time { get; } = time;
    public TextView Message { get; } = message;
    public TextView Stack { get; } = stack;
}

public sealed class ExceptionAdapter(
    Context context,
    IReadOnlyList<CapturedException> exceptions,
    Action<CapturedException> onCopy,
    TypographySettings typographySettings,
    Color entrySurface,
    Color error,
    Color onSurfaceVariant)
    : RecyclerView.Adapter
{
    private const double EntryPadding = 12;
    private const double EntrySpacing = 8;
    private const double EntryCornerRadius = 10;

    public override int ItemCount => exceptions.Count;

    public override RecyclerView.ViewHolder OnCreateViewHolder(
        ViewGroup parent, int viewType)
    {
        var card = new LinearLayout(context)
        {
            Orientation = Orientation.Vertical,
            Clickable = true
        };

        var shape = new ShapeAppearanceModel.Builder()
            .SetAllCorners(CornerFamily.Rounded, context.ToPixels(EntryCornerRadius))
            .Build();
        var drawable = new MaterialShapeDrawable(shape);
        drawable.FillColor = ColorStateList.ValueOf(entrySurface.ToPlatform());
        card.Background = drawable;
        ApplyRipple(context, card);

        var padding = (int)context.ToPixels(EntryPadding);
        card.SetPadding(padding, padding, padding, padding);

        var time = new TextView(context);
        var message = new TextView(context)
        {
            Ellipsize = TextUtils.TruncateAt.End
        };
        message.SetMaxLines(3);

        var stack = new TextView(context)
        {
            Ellipsize = TextUtils.TruncateAt.End
        };
        stack.SetMaxLines(3);

        card.AddView(time);
        card.AddView(message);
        card.AddView(stack);

        card.LayoutParameters = new RecyclerView.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent) { BottomMargin = (int)context.ToPixels(EntrySpacing) };

        var holder = new ExceptionViewHolder(card, time, message, stack);
        card.Click += (_, _) =>
        {
            if (holder.Exception is { } exception)
            {
                onCopy(exception);
            }
        };

        return holder;
    }

    public override void OnBindViewHolder(
        RecyclerView.ViewHolder holder, int position)
    {
        if (holder is not ExceptionViewHolder exceptionHolder)
        {
            return;
        }

        var exception = exceptions[position];
        exceptionHolder.Exception = exception;
        exceptionHolder.Time.Text = exception.Time.ToString("yyyy-MM-dd HH:mm:ss");
        exceptionHolder.Message.Text = exception.Message;
        exceptionHolder.Stack.Text = exception.StackTrace;

        exceptionHolder.Time.SetTextColor(onSurfaceVariant.ToPlatform());
        exceptionHolder.Message.SetTextColor(error.ToPlatform());
        exceptionHolder.Stack.SetTextColor(onSurfaceVariant.ToPlatform());

        DialogFontApplier.Apply(exceptionHolder.ItemView, typographySettings);

        var secondarySize = (float)(TypographySettings.DialogTextSize *
                                    typographySettings.DialogTextScale);
        exceptionHolder.Time.SetTextSize(ComplexUnitType.Sp, secondarySize);
        exceptionHolder.Stack.SetTextSize(ComplexUnitType.Sp, secondarySize);
    }

    private static void ApplyRipple(Context context, LinearLayout card)
    {
        var value = new TypedValue();
        if (context.Theme?.ResolveAttribute(
                global::Android.Resource.Attribute.SelectableItemBackground, value, true) != true ||
            context.GetDrawable(value.ResourceId) is not { } ripple)
        {
            return;
        }

        card.Foreground = ripple;
        card.ClipToOutline = true;
    }
}
