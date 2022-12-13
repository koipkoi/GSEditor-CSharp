using GSEditor.UI.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace GSEditor.Resources;

public partial class Styles
{
  private void OnToolBarLoaded(object sender, RoutedEventArgs _)
  {
    if (sender is ToolBar toolBar)
    {
      if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
        overflowGrid.Visibility = Visibility.Collapsed;
    }
  }

  private void OnSelectorPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
  {
    e.Handled = true;
  }

  private void OnSelectorContextMenuOpening(object sender, ContextMenuEventArgs _)
  {
    if (sender is ListBox listBox && listBox.IsEnabled)
    {
      var result = ListBoxSearchDialog.Show(listBox, listBox.Items);
      if (result != -1)
      {
        listBox.SelectedIndex = result;
        listBox.ScrollIntoView(listBox.SelectedItem);
      }
    }
  }

  private void OnSelectorPreviewMouseWheel(object sender, MouseWheelEventArgs e)
  {
    if ((sender is CheckListBox || sender is Selector) && !e.Handled)
    {
      var selector = sender as UIElement;
      var border = (VisualTreeHelper.GetChild(selector, 0) as Border)!;
      var scrollViewer = (VisualTreeHelper.GetChild(border, 0) as ScrollViewer)!;
      if ((e.Delta > 0 && scrollViewer.VerticalOffset == 0) || (e.Delta < 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight))
      {
        e.Handled = true;

        var parent = (scrollViewer.Parent as UIElement)!;
        parent.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
          RoutedEvent = UIElement.MouseWheelEvent,
          Source = scrollViewer,
        });
      }
    }
  }
}
