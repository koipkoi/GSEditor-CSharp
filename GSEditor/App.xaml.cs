using GSEditor.Core;
using GSEditor.UI.Windows;
using PresentationTheme.Aero;
using System.Windows;

namespace GSEditor;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _ = Injector.Register(this);
        _ = Injector.Register(new Pokegold());
        var appSettings = Injector.Register(new AppSettings());

        AeroTheme.SetAsCurrentTheme();

        var window = new MainWindow
        {
            WindowState = appSettings.WindowState,
            Left = appSettings.WindowLeft,
            Top = appSettings.WindowTop,
            Width = appSettings.WindowWidth,
            Height = appSettings.WindowHeight,
            MinWidth = AppSettings.WindowMinWidth,
            MinHeight = AppSettings.WindowMinHeight,
        };
        window.ShowDialog();

        appSettings.WindowLeft = window.Left;
        appSettings.WindowTop = window.Top;
        appSettings.WindowWidth = window.Width;
        appSettings.WindowHeight = window.Height;
        appSettings.WindowState = window.WindowState;

        Shutdown();
    }
}
