using GSEditor.Contract.Services;
using System.Windows.Controls;

namespace GSEditor.Services;

public sealed class PopupMenuService : IPopupMenuService
{
  public void Show(params PopupMenuItem[] menuItems)
  {
    var menu = new ContextMenu();

    foreach (var item in menuItems)
    {
      if (item.Caption == "-")
      {
        menu.Items.Add(new Separator());
      }
      else
      {
        var newMenuItem = new MenuItem { Header = item.Caption, };
        newMenuItem.Click += (_, __) => item.OnClickAction?.Invoke();
        menu.Items.Add(newMenuItem);
      }
    }

    menu.IsOpen = true;
  }
}
