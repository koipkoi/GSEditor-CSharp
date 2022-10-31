using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace GSEditor.Core;

public sealed class Platform
{
  /// <summary>
  /// 이름
  /// </summary>
  public static string AppName => Assembly.GetEntryAssembly()!.GetName().Name!;

  /// <summary>
  /// 버전
  /// </summary>
  public static string AppVersion => Assembly.GetEntryAssembly()!.GetName().Version!.ToString();

  /// <summary>
  /// 앱 경로
  /// </summary>
  public static string AppDir => AppDomain.CurrentDomain.BaseDirectory;

  /// <summary>
  /// 앱 데이터 경로
  /// </summary>
  public static string AppDataDir
  {
    get
    {
      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      return path;
    }
  }

  /// <summary>
  /// 윈도우 OS 빌드 버전
  /// </summary>
  public static int WindowsBuildVersion
  {
    get
    {
      var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")!;
      var currentBuild = int.Parse((reg.GetValue("CurrentBuild") as string)!);
      return currentBuild;
    }
  }
}
