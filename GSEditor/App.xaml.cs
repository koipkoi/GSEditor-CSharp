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

    Injector.Register(this);
    Injector.Register(new Pokegold());
    Injector.Register(new AppSettings());

    AeroTheme.SetAsCurrentTheme();

    var window = new MainWindow();
    window.ShowDialog();

    Shutdown();
  }
}
