using GSEditor;
using GSEditor.Contract.Services;
using GSEditor.Services;
using Microsoft.Extensions.DependencyInjection;
using PresentationTheme.Aero;
using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;

[assembly: AssemblyProduct(Program.Name)]
[assembly: AssemblyDescription(Program.Name)]
[assembly: AssemblyTitle(Program.Description)]
[assembly: AssemblyCopyright(Program.Author)]
[assembly: AssemblyTrademark(Program.Author)]
[assembly: AssemblyFileVersion(Program.Version)]
[assembly: AssemblyVersion(Program.Version)]
[assembly: AssemblyInformationalVersion(Program.Version)]
[assembly: TargetPlatform("Windows7.0")]
[assembly: SupportedOSPlatform("Windows7.0")]

namespace GSEditor;

public static class Program
{
  public const string Name = "GS 에디터";
  public const string Description = "포켓몬스터 금 버전 에디터";
  public const string Author = "koipkoi";
  public const string Version = "1.2.7.0";

  public static IServiceProvider Services { get; private set; } = new ServiceCollection()
      .AddSingleton<IPokegoldService, PokegoldService>()
      .AddSingleton<ISettingsService, SettingsService>()
      .AddSingleton<IDialogService, DialogService>()
      .AddSingleton<IPopupMenuService, PopupMenuService>()
      .BuildServiceProvider();

  [STAThread]
  public static int Main()
  {
    AeroTheme.SetAsCurrentTheme();

    var app = new App();
    app.InitializeComponent();
    app.Startup += AppOnStartup;
    return app.Run();
  }

  private static void AppOnStartup(object? sender, StartupEventArgs _)
  {
    var app = (sender as Application)!;
    AppMain(app);

    app.Startup -= AppOnStartup;
    app.Shutdown();
  }

  private static void AppMain(Application _)
  {
    Services.GetRequiredService<IDialogService>().ShowMain();
  }
}
