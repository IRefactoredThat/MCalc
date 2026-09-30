using _Microsoft.Android.Resource.Designer;
using Android.Content;
using Android.Widget;
using AndroidX.Core.Content;
using CalculatorApp.CalculatorPage.Input;
using Java.Lang;
using Java.Lang.Reflect;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace CalculatorApp.Platforms.Android.Handlers;

internal sealed class CursorAwareEditText(Context context) : MauiAppCompatEditText(context)
{
    public Func<int, int>? NormalizeCursorPosition { get; set; }

    protected override void OnSelectionChanged(int selStart, int selEnd)
    {
        if (NormalizeCursorPosition != null && selStart == selEnd)
        {
            var newPos = NormalizeCursorPosition.Invoke(selEnd);

            if (newPos != selEnd)
            {
                SetSelection(newPos);
                return;
            }
        }

        base.OnSelectionChanged(selStart, selEnd);
    }

    private static readonly Field? EditorField = CreateEditorField();
    private static readonly Field? CursorDrawableField = CreateCursorDrawableField();

    private static Field? CreateEditorField()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            return null;
        }
        try
        {
            var field = Class
                .FromType(typeof(TextView))
                .GetDeclaredField("mEditor");
            field.Accessible = true;
            return field;
        }
        catch
        {
            return null;
        }
    }

    private static Field? CreateCursorDrawableField()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            return null;
        }
        try
        {
            var editorClass = Class.ForName("android.widget.Editor");
            var field = editorClass.GetDeclaredField("mCursorDrawable");
            field.Accessible = true;
            return field;
        }
        catch
        {
            return null;
        }
    }

    public void SetCursorColor(Color color)
    {
        var platformColor = color.ToPlatform();
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            TextCursorDrawable?.SetTint(platformColor);
            return;
        }
        try
        {
            var drawable = ContextCompat.GetDrawable(Context, ResourceConstant.Drawable.abc_text_cursor_material);
            if (drawable is null)
            {
                return;
            }
            drawable.SetTint(platformColor);
            if (EditorField is null || CursorDrawableField is null)
                return;

            var editor = EditorField.Get(this);
            if (editor is null)
                return;

            CursorDrawableField.Set(editor, new[] { drawable, drawable });
        }
        catch
        {
            // Ignore and use default cursor.
        }
    }
}

public class CalculatorInputHandler() : EntryHandler(InputMapper)
{
    private static readonly
        IPropertyMapper<CalculatorInput, CalculatorInputHandler> InputMapper =
        new PropertyMapper<CalculatorInput, CalculatorInputHandler>(Mapper)
        {
            [nameof(CalculatorInput.CursorColor)] = MapCursorColor,
            [nameof(InputView.CursorPosition)] = MapCursorPosition,
            [nameof(CalculatorInput.NormalizePosition)] = MapNormalizePosition
        };

    protected override MauiAppCompatEditText CreatePlatformView() =>
        new CursorAwareEditText(Context);

    protected override void ConnectHandler(MauiAppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);
        platformView.SetPadding(0, 0, 0, 0);
        platformView.SetCursorVisible(true);
        platformView.ShowSoftInputOnFocus = false;
        platformView.Background = null;
    }

    private static void MapCursorColor(CalculatorInputHandler handler, CalculatorInput view)
    {
        if (handler.PlatformView is CursorAwareEditText editText)
        {
            editText.SetCursorColor(view.CursorColor);
        }
    }

    private static void MapCursorPosition(CalculatorInputHandler handler, CalculatorInput view)
    {
        handler.PlatformView.SetSelection(view.CursorPosition);
    }

    private static void MapNormalizePosition(CalculatorInputHandler handler,
        CalculatorInput view)
    {
        if (handler.PlatformView is CursorAwareEditText editText)
        {
            editText.NormalizeCursorPosition = view.NormalizePosition;
        }
    }
}