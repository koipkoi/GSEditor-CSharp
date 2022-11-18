using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GSEditor.UI.Windows;

public partial class LearnMoveDialog : Window
{
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();
  private PGLearnMove? _result = null;

  private LearnMoveDialog(DependencyObject owner, PGLearnMove? defaultValue)
  {
    Owner = GetWindow(owner);
    Title = defaultValue != null ? "배우는 기술 수정" : "배우는 기술 추가";

    InitializeComponent();

    foreach (var e in _pokegold.Strings.MoveNames)
      MoveComboBox.Items.Add(e);

    if (defaultValue != null)
    {
      LevelUpDown.Value = defaultValue.Level;
      MoveComboBox.SelectedIndex = defaultValue.MoveNo;
    }
    else
    {
      LevelUpDown.Value = 5;
      MoveComboBox.SelectedIndex = 0;
    }
  }

  public static PGLearnMove? Show(DependencyObject owner, PGLearnMove? defaultValue = null)
  {
    var dialog = new LearnMoveDialog(owner, defaultValue);
    if (dialog.ShowDialog() ?? false)
      return dialog._result;
    return null;
  }

  private void OnConfirmClick(object _, RoutedEventArgs __)
  {
    _result = new()
    {
      Level = (byte)(LevelUpDown.Value ?? 0),
      MoveNo = (byte)(MoveComboBox.SelectedIndex + 1),
    };
    DialogResult = true;
    Close();
  }

  private void OnCancelClick(object _, RoutedEventArgs __)
  {
    Close();
  }
}
