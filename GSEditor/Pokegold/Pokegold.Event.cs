using System.Windows;

namespace GSEditor.Core;

public sealed partial class Pokegold
{
  public event EventHandler? RomChanged;
  public event EventHandler? DataChanged;

  public void RegisterRomChanged(FrameworkElement element, EventHandler e)
  {
    void onUnloaded(object? _, EventArgs __)
    {
      RomChanged -= e;
      element.Unloaded -= onUnloaded;
    }
    element.Loaded += (_, __) =>
    {
      RomChanged += e;
      element.Unloaded += onUnloaded;
    };
  }

  public void NotifyDataChanged(bool changed = true)
  {
    IsChanged = changed;
    DataChanged?.Invoke(this, EventArgs.Empty);
  }

  public void RegisterDataChanged(FrameworkElement element, EventHandler e)
  {
    void onUnloaded(object? _, EventArgs __)
    {
      DataChanged -= e;
      element.Unloaded -= onUnloaded;
    }
    element.Loaded += (_, __) =>
    {
      DataChanged += e;
      element.Unloaded += onUnloaded;
    };
  }
}
