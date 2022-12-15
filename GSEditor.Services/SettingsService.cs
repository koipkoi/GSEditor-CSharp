using GSEditor.Contract.Services;
using GSEditor.Models.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

namespace GSEditor.Services;

public sealed class SettingsService : ISettingsService
{
  private readonly JsonSerializer _serializer = new();

  private AppSettings? _appSettings;
  public AppSettings AppSettings
  {
    get
    {
      if (_appSettings == null)
        _appSettings = ReadInternal<AppSettings>("app_settings.bin");
      return _appSettings!;
    }

    set => _appSettings = value;
  }

  public void Apply()
  {
    WriteInternal(_appSettings, "app_settings.bin");
  }

  private T ReadInternal<T>(string name) where T : new()
  {
    if (!File.Exists(name))
      return new();
    using var ms = new MemoryStream(File.ReadAllBytes(name));
    using var reader = new BsonDataReader(ms);
    return _serializer.Deserialize<T>(reader)!;
  }

  private void WriteInternal(object? obj, string name)
  {
    if (obj != null)
    {
      using var ms = new MemoryStream();
      using var writer = new BsonDataWriter(ms);
      _serializer.Serialize(writer, obj);
      File.WriteAllBytes(name, ms.ToArray());
    }
  }
}
