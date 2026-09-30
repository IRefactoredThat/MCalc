using Android.Graphics.Drawables;
using Google.Android.Material.AppBar;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace CalculatorApp.Platforms.Android.Shell;

/// <summary>
/// Custom render that fixes top bar appearance by manually matching its color to App shell background
/// </summary>
public class AppShellRender : ShellRenderer
{
    protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker() =>
        new AppBarSyncedToolbarAppearanceTracker(base.CreateToolbarAppearanceTracker());

    sealed class AppBarSyncedToolbarAppearanceTracker(IShellToolbarAppearanceTracker inner)
        : IShellToolbarAppearanceTracker
    {
        public void SetAppearance(AToolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
        {
            inner.SetAppearance(toolbar, toolbarTracker, appearance);
            SyncAppBar(toolbar, appearance.BackgroundColor);
        }

        public void ResetAppearance(AToolbar toolbar, IShellToolbarTracker toolbarTracker)
        {
            inner.ResetAppearance(toolbar, toolbarTracker);
            SyncAppBar(toolbar, null);
        }

        public void Dispose() => inner.Dispose();
    }

    static void SyncAppBar(AToolbar? toolbar, Color? color)
    {
        if (toolbar is null)
        {
            return;
        }

        var barColor = color ?? DefaultBackgroundColor;
        var appBar = FindAppBarLayout(toolbar);

        if (appBar is not null)
        {
            PaintAppBar(appBar, barColor);
        }
    }

    static void PaintAppBar(AppBarLayout appBar, Color color)
    {
        var platformColor = color.ToPlatform();
        appBar.LiftOnScroll = false;

        appBar.BackgroundTintMode = null;
        appBar.BackgroundTintList = null;

        var drawable = new ColorDrawable(platformColor);
        appBar.Background = drawable;
        appBar.StatusBarForeground = drawable;
    }

    static AppBarLayout? FindAppBarLayout(AToolbar toolbar)
    {
        var parent = toolbar.Parent;
        while (parent is not null)
        {
            if (parent is AppBarLayout appBar)
            {
                return appBar;
            }

            parent = parent.Parent;
        }

        return null;
    }
}
