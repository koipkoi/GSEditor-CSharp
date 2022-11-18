using GSEditor.UI.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GSEditor.UI.Styles;

public partial class ListBox
{
  private void OnListBoxPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
  {
    e.Handled = true;
  }

  private void OnListBoxContextMenuOpening(object sender, ContextMenuEventArgs _)
  {
    if (sender is System.Windows.Controls.ListBox listBox && listBox.IsEnabled)
    {
      var result = ListBoxSearchDialog.Show(listBox, listBox.Items);
      if (result != -1)
      {
        listBox.SelectedIndex = result;
        listBox.ScrollIntoView(listBox.SelectedItem);
      }
    }
  }
}
