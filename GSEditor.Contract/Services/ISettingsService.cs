using GSEditor.Models.Settings;

namespace GSEditor.Contract.Services;

public interface ISettingsService
{
  public AppSettings AppSettings { get; set; }

  public void Apply();
}
