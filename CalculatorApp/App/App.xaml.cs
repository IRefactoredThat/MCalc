namespace CalculatorApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override void OnStart()
    {
        base.OnStart();
        if(Current != null)
        {
            Current.UserAppTheme = AppTheme.Dark;
        }
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());
        var displayInfo = DeviceDisplay.MainDisplayInfo;
        double screenWidth = displayInfo.Width / displayInfo.Density;
        double screenHeight = displayInfo.Height / displayInfo.Density;

        window.MinimumWidth = screenWidth * 0.2d;
        window.MinimumHeight = screenHeight * 0.5d;

        return window;
    }
#endif
}