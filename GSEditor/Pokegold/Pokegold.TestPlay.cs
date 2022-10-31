using System.Diagnostics;
using System.IO;

namespace GSEditor.Core;

public sealed partial class Pokegold
{
  public bool StartTestPlay(string emulatorPath)
  {
    try
    {
      var path = Path.GetDirectoryName(Filename)!;
      var withoutExtFilename = Path.GetFileNameWithoutExtension(Filename);
      var saveFilename = Path.Combine(path, $"{withoutExtFilename}.sav");

      var newFilename = Path.Combine(Platform.AppDataDir, $"test_play_temp.gbc");
      var newSaveFilename = Path.Combine(Platform.AppDataDir, $"test_play_temp.sav");
      File.Copy(Filename, newFilename, true);
      if (File.Exists(saveFilename))
        File.Copy(saveFilename, newSaveFilename, true);

      var process = new Process();
      process.StartInfo.FileName = emulatorPath;
      process.StartInfo.Arguments = $"\"{newFilename}\"";
      process.StartInfo.UseShellExecute = true;
      process.StartInfo.CreateNoWindow = true;
      process.Start();
      process.WaitForExit();

      File.Delete(newFilename);
      File.Delete(newSaveFilename);

      return true;
    }
    catch { }
    return false;
  }
}
