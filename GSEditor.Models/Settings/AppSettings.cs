using Newtonsoft.Json;

namespace GSEditor.Models.Settings;

public sealed record AppSettings
{
  [JsonProperty("emulator_path")]
  public string? EmulatorPath { get; set; } = null;

  [JsonProperty("main_window_setting")]
  public WindowSettings MainWindowSetting { get; set; } = new()
  {
    Left = 128.0,
    Top = 128.0,
    Width = 720.0,
    Height = 560.0,
    MinWidth = 720.0,
    MinHeight = 560.0,
  };
}
