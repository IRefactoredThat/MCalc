using System.ComponentModel;

namespace CalculatorApp.UIComponents;

public partial class CalculatorInputField : Editor
{
    private const double FontSizeFactor = 0.2d;

    public CalculatorInputField()
    {
        InitializeComponent();
        SetHandler();

        CalculatorButton.FontSizeChanged += UpdateFont;
    }

    public delegate void CursorPositionDelegate(int position);
    public static event CursorPositionDelegate? CursorPositionChanged;

    public delegate void TextLengthDelegate(int length);
    public static event TextLengthDelegate? TextLengthChanged;

    public delegate void FontSizeDelegate(double size);
    public static event FontSizeDelegate? FontSizeChanged;

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CursorPosition))
        {
            CursorPositionChanged?.Invoke(CursorPosition);
        }
        if (e.PropertyName == nameof(Text))
        {
            TextLengthChanged?.Invoke(Text.Length);
        }
    }

    private void OnFocused(object sender, FocusEventArgs e)
    {
#if WINDOWS
        IsReadOnly = true;
#endif
    }
    private void OnUnfocused(object sender, FocusEventArgs e)
    {
#if WINDOWS
        IsReadOnly = false;
#endif
    }

    private void UpdateFont(double size)
    {
        var minSize = Math.Min(Width, Height);
        FontSize = minSize * FontSizeFactor;
        FontSizeChanged?.Invoke(FontSize);
    }
#if ANDROID
    private class NoSelectionCallback : Java.Lang.Object, 
        Android.Views.ActionMode.ICallback
    {
        public bool OnActionItemClicked(Android.Views.ActionMode? mode, 
            Android.Views.IMenuItem? item) => false;
        public bool OnCreateActionMode(Android.Views.ActionMode? mode, 
            Android.Views.IMenu? menu) => false;
        public void OnDestroyActionMode(Android.Views.ActionMode? mode) { }
        public bool OnPrepareActionMode(Android.Views.ActionMode? mode, 
            Android.Views.IMenu? menu) => false;
    }
#endif

    private static void SetHandler()
    {
        Microsoft.Maui.Handlers.EditorHandler.Mapper
            .AppendToMapping("NoKeyboardEditorCustomization", (handler, view) =>
        {
            if (view is Editor)
            {
#if ANDROID
                var res = handler.PlatformView;
                res.ShowSoftInputOnFocus = false;
                res.BackgroundTintList = Android.Content.Res.ColorStateList
                                    .ValueOf(Android.Graphics.Color.Transparent);
                res.SetTextIsSelectable(false);
                res.CustomSelectionActionModeCallback = new NoSelectionCallback();
#elif WINDOWS
                var res = handler.PlatformView;
                res.SelectionChanging += (sender, e) =>
                {
                    if (e.SelectionLength != 0)
                    {
                        res.SelectionLength = 0;
                    }
                };
#endif
            }
        });
    }
}