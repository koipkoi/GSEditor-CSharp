using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GSEditor.Core;

internal sealed class JsonIOHelper
{
  private readonly JsonSerializer _serializer = new();
  private readonly string _filename;

  private JObject? _internalJson = null;
  private JObject Json
  {
    get
    {
      if (_internalJson == null)
      {
        if (!File.Exists(SettingsPath))
        {
          _internalJson = new JObject();
        }
        else
        {
          using var ms = new MemoryStream(File.ReadAllBytes(SettingsPath));
          using var reader = new BsonDataReader(ms);
          _internalJson = _serializer.Deserialize<JObject>(reader);
        }
      }
      return _internalJson!;
    }
  }

  private string SettingsPath => Path.Combine(Platform.AppDataDir, _filename);

  public JsonIOHelper(string filename)
  {
    _filename = filename;
  }

  public T Read<T>(string key, T defaultValue)
  {
    var v = Json[key];
    if (v == null)
      return defaultValue;
    return v.ToObject<T>() ?? defaultValue;
  }

  public void Write(string key, JToken value)
  {
    Json[key] = value;

    using var ms = new MemoryStream();
    using var writer = new BsonDataWriter(ms);
    _serializer.Serialize(writer, Json);
    File.WriteAllBytes(SettingsPath, ms.ToArray());
  }
}
