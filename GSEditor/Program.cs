using GSEditor.Core;
using GSEditor.UI.Forms;

namespace GSEditor;

public static class Program
{
  [STAThread]
  static void Main()
  {
    ApplicationConfiguration.Initialize();

    Injector.Register(new Pokegold());

    var form = new MainForm();
    form.ShowDialog();
  }
}
