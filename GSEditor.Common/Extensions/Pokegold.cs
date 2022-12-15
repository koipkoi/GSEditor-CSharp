using GSEditor.Contract.Services;
using System;
using System.Windows;

namespace GSEditor.Common.Extensions;

public static class Pokegold
{
  public static void RegisterRomChanged(this IPokegoldService pokegold, FrameworkElement element, EventHandler e)
  {
    void onUnloaded(object? _, EventArgs __)
    {
      pokegold.RomChanged -= e;
      element.Unloaded -= onUnloaded;
    }
    element.Loaded += (_, __) =>
    {
      pokegold.RomChanged += e;
      element.Unloaded += onUnloaded;
    };
  }

  public static void RegisterDataChanged(this IPokegoldService pokegold, FrameworkElement element, EventHandler e)
  {
    void onUnloaded(object? _, EventArgs __)
    {
      pokegold.DataChanged -= e;
      element.Unloaded -= onUnloaded;
    }
    element.Loaded += (_, __) =>
    {
      pokegold.DataChanged += e;
      element.Unloaded += onUnloaded;
    };
  }
}
