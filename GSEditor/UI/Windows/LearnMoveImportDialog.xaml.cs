using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Windows;

public partial class LearnMoveImportDialog : Window
{
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();
  private readonly List<PGLearnMove> _result = new();

  private LearnMoveImportDialog()
  {
    InitializeComponent();

    for (var i = 0; i < 251; i++)
      PokemonListBox.Items.Add(_pokegold.Strings.PokemonNames[i]);
  }

  public static List<PGLearnMove> Show(DependencyObject owner)
  {
    var window = new LearnMoveImportDialog { Owner = GetWindow(owner), };
    window.ShowDialog();
    return window._result;
  }

  private void OnWindowSizeChanged(object _, SizeChangedEventArgs __)
  {
    if (LearnMoveListView.View is GridView learnMoveGridView)
    {
      var totallyWidth = LearnMoveListView.ActualWidth - 32;
      learnMoveGridView.Columns[0].Width = totallyWidth * 0.3;
      learnMoveGridView.Columns[1].Width = totallyWidth * 0.7;
    }
  }

  private void OnPokemonListBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index != -1)
    {
      LearnMoveListView.SelectedIndex = -1;

      _result.Clear();
      LearnMoveListView.Items.Clear();

      foreach (var e in _pokegold.Pokemons[index].LearnMoves)
      {
        _result.Add(e);
        LearnMoveListView.Items.Add(new LearnMoveItem
        {
          Level = $"{e.Level}",
          Move = $"{_pokegold.Strings.MoveNames[e.MoveNo - 1]}",
        });
      }
    }
  }

  private void OnRemoveItemClick(object sender, RoutedEventArgs e)
  {
    var index = LearnMoveListView.SelectedIndex;
    if (index != -1)
    {
      LearnMoveListView.Items.RemoveAt(index);
      _result.RemoveAt(index);
    }
  }

  private void OnConfirmClick(object _, RoutedEventArgs __)
  {
    Close();
  }

  private void OnCancelClick(object _, RoutedEventArgs __)
  {
    _result.Clear();
    Close();
  }
}

public sealed class LearnMoveItem
{
  public string Level { get; set; } = "";
  public string Move { get; set; } = "";
}
