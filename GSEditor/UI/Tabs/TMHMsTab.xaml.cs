using GSEditor.Core;
using GSEditor.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GSEditor.UI.Tabs;

public partial class TMHMsTab : UserControl
{
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();

  public TMHMsTab()
  {
    InitializeComponent();

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, _) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    if (_pokegold.IsOpened)
    {
      var previousSelection = TMHMsListBox.SelectedIndex;

      this.RunSafe(() =>
      {
        TMHMsListBox.Items.Clear();
        for (var i = 0; i < 57; i++)
        {
          var label = (i < 50 ? "기술" : "비전");
          var number = (i < 50 ? i + 1 : i - 49).ToString().PadLeft(2, '0');
          var name = _pokegold.Strings.MoveNames[_pokegold.TMHMs[i] - 1];
          TMHMsListBox.Items.Add($"{label}{number} [{name}]");
        }

        MoveComboBox.Items.Clear();
        foreach (var e in _pokegold.Strings.MoveNames)
          MoveComboBox.Items.Add(e);
      });

      TMHMsListBox.SelectedIndex = previousSelection;
    }
  }

  private void NestedScrollImplEvent(object sender, MouseWheelEventArgs e)
  {
    if (sender is ScrollViewer scrollViewer)
    {
      if ((e.Delta > 0 && scrollViewer.VerticalOffset == 0) || (e.Delta < 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight))
      {
        e.Handled = true;

        var parent = (scrollViewer.Parent as UIElement)!;
        parent.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
          RoutedEvent = MouseWheelEvent,
          Source = scrollViewer,
        });
      }
    }
  }

  private void OnTMHMsSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = TMHMsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        MoveComboBox.SelectedIndex = _pokegold.TMHMs[index] - 1;
      });
    }

    ContentBorder.IsEnabled = index != -1;
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var previousSelection = TMHMsListBox.SelectedIndex;

    var index = TMHMsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.TMHMs[index] = (byte)(MoveComboBox.SelectedIndex + 1);

        var label = (index < 50 ? "기술" : "비전");
        var number = (index < 50 ? index + 1 : index - 49).ToString().PadLeft(2, '0');
        var name = _pokegold.Strings.MoveNames[_pokegold.TMHMs[index] - 1];
        TMHMsListBox.Items[index] = $"{label}{number} [{name}]";

        _pokegold.NotifyDataChanged();
      });
    }

    TMHMsListBox.SelectedIndex = previousSelection;
  }
}
