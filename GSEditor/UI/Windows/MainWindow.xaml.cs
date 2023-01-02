using System.Windows;

namespace GSEditor.UI.Windows;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
  }

  private void OnCloseClick(object _, RoutedEventArgs __)
  {
    Close();
  }
}
