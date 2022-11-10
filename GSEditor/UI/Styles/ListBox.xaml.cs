using GSEditor.UI.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GSEditor.UI.Styles;

public partial class ListBox
{
  private void OnListBoxPreviewMouseRightButtonDown(object _, MouseButtonEventArgs e)
  {
    e.Handled = true;
  }

  private void OnListBoxContextMenuOpening(object sender, ContextMenuEventArgs _)
  {
    if (sender is System.Windows.Controls.ListBox listBox && listBox.IsEnabled)
    {
      var result = ListBoxSearchDialog.Show(listBox, listBox.Items);
      listBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
      {
        if (result != -1)
        {
          listBox.SelectedIndex = result;
          listBox.ScrollIntoView(listBox.SelectedItem);
        }
      });
    }
  }
}
