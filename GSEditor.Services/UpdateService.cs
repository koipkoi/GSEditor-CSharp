using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using GSEditor.Models;
using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace GSEditor.Services;

public sealed class UpdateService : IUpdateService
{
  private readonly HttpClient _httpClient = new();

  public async Task<AppUpdate> GetAppUpdate()
  {
    var url = "https://api.github.com/repos/koipkoi/GSEditor/releases/latest";
    var message = new HttpRequestMessage(HttpMethod.Get, url);
    message.Headers.Add("Accept", "application/vnd.github+json");
    message.Headers.Add("User-Agent", "gs-editor");
    var response = await _httpClient.SendAsync(message);
    var source = await response.Content.ReadAsStringAsync();
    var json = JsonNode.Parse(source)!;
    var appVersion = Version.Parse(Platforms.AppVersion);
    var version = Version.Parse(json["name"]!.ToString());
    return new(version > appVersion, version);
  }
}
