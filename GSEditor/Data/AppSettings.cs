using System.Windows;

namespace GSEditor.Core;

/// <summary>
/// 앱 설정
/// </summary>
public sealed class AppSettings
{
  public const double WindowMinWidth = 720;
  public const double WindowMinHeight = 560;

  private readonly JsonIOHelper _data = new("app_settings.bin");

  public string EmulatorPath
  {
    get => _data.Read("emulator_path", "");
    set => _data.Write("emulator_path", value);
  }

  public double WindowWidth
  {
    get => _data.Read("window_width", WindowMinWidth);
    set => _data.Write("window_width", value);
  }

  public double WindowHeight
  {
    get => _data.Read("window_height", WindowMinHeight);
    set => _data.Write("window_height", value);
  }

  public double WindowLeft
  {
    get => _data.Read("window_left", 128);
    set => _data.Write("window_left", value);
  }

  public double WindowTop
  {
    get => _data.Read("window_top", 128);
    set => _data.Write("window_top", value);
  }

  public WindowState WindowState
  {
    get => (WindowState)_data.Read("window_state", (int)WindowState.Normal);
    set => _data.Write("window_state", (int)value);
  }
}
