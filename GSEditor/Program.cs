using GSEditor.Core;

namespace GSEditor;

public static class Program
{
  [STAThread]
  static void Main()
  {
    ApplicationConfiguration.Initialize();

    var pokegold = new Pokegold();
    pokegold.Read("test.gbc");
    pokegold.Write("test_mod.gbc");
  }
}
