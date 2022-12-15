using Newtonsoft.Json;

namespace GSEditor.Models.Settings;

public sealed record WindowSettings
{
  [JsonProperty("left")]
  public double Left { get; set; } = 128.0;
  [JsonProperty("top")]
  public double Top { get; set; } = 128.0;

  [JsonProperty("width")]
  public double Width { get; set; } = 720.0;
  [JsonProperty("height")]
  public double Height { get; set; } = 560.0;

  [JsonProperty("min_width")]
  public double MinWidth { get; set; } = 720.0;
  [JsonProperty("min_height")]
  public double MinHeight { get; set; } = 560.0;

  [JsonProperty("state")]
  public int State { get; set; } = -1;
}
