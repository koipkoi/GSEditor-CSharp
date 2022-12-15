using GSEditor.Models.Settings;
using System.Linq;
using System.Windows;

namespace GSEditor.Common.Extensions;

public static class Windows
{
  public static void ShowDialogCenterOwner(this Window window)
  {
    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    window.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
    window.ShowDialog();
  }

  public static void SetWindowSetting(this Window window, WindowSettings setting)
  {
    window.MinWidth = setting.MinWidth;
    window.MinHeight = setting.MinHeight;
    window.Width = setting.Width;
    window.Height = setting.Height;
    window.Left = setting.Left;
    window.Top = setting.Top;
    window.WindowState = setting.State == -1 ? WindowState.Normal : (WindowState)setting.State;
  }

  public static WindowSettings GetWindowSetting(this Window window)
  {
    return new()
    {
      MinWidth = window.MinWidth,
      MinHeight = window.MinHeight,
      Width = window.Width,
      Height = window.Height,
      Left = window.Left,
      Top = window.Top,
      State = (int)window.WindowState,
    };
  }
}
