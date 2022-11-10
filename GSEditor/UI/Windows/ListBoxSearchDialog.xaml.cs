using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GSEditor.UI.Windows;

public partial class ListBoxSearchDialog : Window
{
  private List<ListBoxSearchItem> _items = new();
  private int _result = -1;

  private ListBoxSearchDialog()
  {
    InitializeComponent();
    Loaded += (_, __) => UpdateResult();
  }

  public static int Show(DependencyObject owner, ItemCollection list)
  {
    var window = new ListBoxSearchDialog { Owner = GetWindow(owner) };

    var i = 0;
    foreach (var e in list)
    {
      window._items.Add(new()
      {
        Number = i++.ToString(),
        Content = e.ToString()!,
      });
    }

    window.ShowDialog();

    return window._result;
  }

  private void OnWindwSizeChanged(object sender, SizeChangedEventArgs e)
  {
    if (ResultListView.View is GridView learnMoveGridView)
    {
      var totallyWidth = ResultListView.ActualWidth - 32;
      learnMoveGridView.Columns[0].Width = totallyWidth * 0.2;
      learnMoveGridView.Columns[1].Width = totallyWidth * 0.7;
    }
  }

  private void OnWindowPreviewKeyDown(object _, KeyEventArgs e)
  {
    if (e.Key == Key.Escape)
    {
      _result = -1;
      Close();
    }
  }

  private void OnKeywordTextChanged(object _, TextChangedEventArgs __)
  {
    UpdateResult();
  }

  private void OnListViewSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var number = (ResultListView.SelectedItem as ListBoxSearchItem)!.Number;
    _items.ForEach(e =>
    {
      if (e.Number == number)
        _result = int.Parse(number);
    });
  }

  private void UpdateResult()
  {
    ResultListView.Items.Clear();

    var keyword = KeywordTextBox.Text;

    // 빈 검색어는 모든 항목 추가
    if (keyword.Trim() == "")
    {
      _items.ForEach(e => ResultListView.Items.Add(e));
      return;
    }

    // 검색어 결과 추가
    _items.ForEach(e =>
    {
      if (e.Content.Contains(keyword))
        ResultListView.Items.Add(e);
    });
  }

  private void OnListViewDoubleClick(object _, System.Windows.Input.MouseButtonEventArgs __)
  {
    if (_result != -1)
      Close();
  }

  private void OnConfirmClick(object _, RoutedEventArgs __)
  {
    Close();
  }

  private void OnCancelClick(object _, RoutedEventArgs __)
  {
    _result = -1;
    Close();
  }
}

public sealed class ListBoxSearchItem
{
  public string Number { get; set; } = "";
  public string Content { get; set; } = "";
}
