namespace CalculatorApp.Animations;

internal class TextTransferAnimation
{
    public static async Task Animate(Label label, Editor editor)
    {
        editor.TextColor = Colors.Transparent;
        editor.Text = label.Text;

        label.TextColor = Colors.Transparent;
        label.Text = string.Empty;

        await Task.WhenAll(editor.TextColorTo(Colors.White, length: 400),
            label.TextColorTo(Colors.White, length: 400));
    }
}