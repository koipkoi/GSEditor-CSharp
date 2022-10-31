namespace GSEditor.Core;

/// <summary>
/// 앱 설정
/// </summary>
public sealed class AppSettings
{
  private readonly JsonIOHelper _data = new("app_settings.bin");

  public string EmulatorPath
  {
    get => _data.Read("emulator_path", "");
    set => _data.Write("emulator_path", value);
  }
}
