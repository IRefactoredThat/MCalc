namespace CalculatorApp.Platforms.Windows;

using System.Runtime.InteropServices;

public static class WindowHelper
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId
        (IntPtr hWnd, out uint lpdwProcessId);

    public static bool IsAppActive()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
        {
            return false;
        }

        _ = GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);

        return Environment.ProcessId == foregroundProcessId;
    }
}
