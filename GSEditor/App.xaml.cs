using GSEditor.Contract.Services;
using GSEditor.Services;
using Microsoft.Extensions.DependencyInjection;
using PresentationTheme.Aero;
using System.Windows;

namespace GSEditor;

public partial class App
{
  public static IServiceProvider Services { get; private set; } = new ServiceCollection()
      .AddSingleton<IPokegoldService, PokegoldService>()
      .AddSingleton<ISettingsService, SettingsService>()
      .AddSingleton<IUpdateService, UpdateService>()
      .AddSingleton<IDialogService, DialogService>()
      .AddSingleton<IPopupMenuService, PopupMenuService>()
      .BuildServiceProvider();

  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    AeroTheme.SetAsCurrentTheme();
    Services.GetRequiredService<IDialogService>().ShowMain();
    Shutdown();
  }
}
