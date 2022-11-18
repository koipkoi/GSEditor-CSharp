using GSEditor.Core;
using GSEditor.UI.Windows;
using Microsoft.Extensions.DependencyInjection;
using PresentationTheme.Aero;
using System.Windows;

namespace GSEditor;

public partial class App
{
  public static IServiceProvider Services { get; private set; } = new ServiceCollection()
      .AddSingleton<Pokegold>()
      .AddSingleton<AppSettings>()
      .AddTransient<MainWindow>()
      .BuildServiceProvider();

  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    AeroTheme.SetAsCurrentTheme();

    var appSettings = Services.GetRequiredService<AppSettings>();
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
