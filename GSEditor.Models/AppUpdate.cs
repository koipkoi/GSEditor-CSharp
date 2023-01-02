using System;

namespace GSEditor.Models;

public sealed record AppUpdate
{
  public bool HasUpdate { get; }
  public Version LatestVersion { get; }

  public AppUpdate(bool hasUpdate, Version version)
  {
    HasUpdate = hasUpdate;
    LatestVersion = version;
  }
}
