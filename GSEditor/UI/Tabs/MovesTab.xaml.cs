using GSEditor.Common.Extensions;
using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Tabs;

public partial class MovesTab : UserControl
{
  private readonly IPokegoldService _pokegold = App.Services.GetRequiredService<IPokegoldService>();

  public MovesTab()
  {
    InitializeComponent();

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, __) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    var previousMovesSelection = MovesListBox.SelectedIndex;
    var previousTypeSelection = TypeComboBox.SelectedIndex;

    this.RunSafe(() =>
    {
      MovesListBox.Items.Clear();
      foreach (var e in _pokegold.Data.Strings.MoveNames)
        MovesListBox.Items.Add(e);

      TypeComboBox.Items.Clear();
      foreach (var e in _pokegold.Data.Strings.MoveTypeNames)
        TypeComboBox.Items.Add(e);
    });

    MovesListBox.SelectedIndex = previousMovesSelection;
    TypeComboBox.SelectedIndex = previousTypeSelection;
  }

  private void OnMovesListBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = MovesListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        NoTextBox.Text = $"{_pokegold.Data.Moves[index].No}";
        NameTextBox.Text = _pokegold.Data.Strings.MoveNames[index];
        DescriptionTextBox.Text = _pokegold.Data.Strings.MoveDescriptions[index].Replace("[59]", "\n");

        TypeComboBox.SelectedIndex = _pokegold.Data.Moves[index].MoveType;
        PPUpDown.Value = _pokegold.Data.Moves[index].PP;
        PowerUpDown.Value = _pokegold.Data.Moves[index].Power;
        AccuracyUpDown.Value = _pokegold.Data.Moves[index].Accuracy;

        EffectComboBox.SelectedIndex = _pokegold.Data.Moves[index].Effect;
        EffectUpDown.Value = _pokegold.Data.Moves[index].EffectChance;
      });
    }

    ContentPanel.IsEnabled = index != -1;
  }

  private void OnTextBoxTextChanged(object _, TextChangedEventArgs __)
  {
    var index = MovesListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        if (NameTextBox.Text.TryTextEncode(out var _))
        {
          var previousSelection = MovesListBox.SelectedIndex;
          _pokegold.Data.Strings.MoveNames[index] = NameTextBox.Text;
          MovesListBox.Items[index] = _pokegold.Data.Strings.MoveNames[index];
          MovesListBox.SelectedIndex = previousSelection;
          _pokegold.NotifyDataChanged();
        }

        var realDescription = DescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "[59]");
        if (realDescription.TryTextEncode(out var _))
        {
          _pokegold.Data.Strings.MoveDescriptions[index] = realDescription;
          _pokegold.NotifyDataChanged();
        }
      });
    }

    var maxLength = 0;
    foreach (var line in DescriptionTextBox.Text.Split("\n"))
    {
      if (line.Length > maxLength)
        maxLength = line.Length;
    }
    DescriptionLabel.Content = $"설명 ({maxLength}/18)：";
  }

  private void OnUpDownValueChaged(object _, RoutedPropertyChangedEventArgs<object> __)
  {
    var index = MovesListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.Data.Moves[index].PP = (byte)(PPUpDown.Value ?? 0);
        _pokegold.Data.Moves[index].Power = (byte)(PowerUpDown.Value ?? 0);
        _pokegold.Data.Moves[index].Accuracy = (byte)(AccuracyUpDown.Value ?? 0);
        _pokegold.Data.Moves[index].EffectChance = (byte)(EffectUpDown.Value ?? 0);
        _pokegold.NotifyDataChanged();
      });

      // 명중률 퍼센티지 변경
      var accuracyPercentage = string.Format("{0:P2}", (double)_pokegold.Data.Moves[index].Accuracy / 0xff);
      AccuracyPercentageLabel.Content = accuracyPercentage;
    }
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = MovesListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.Data.Moves[index].MoveType = (byte)TypeComboBox.SelectedIndex;
        _pokegold.Data.Moves[index].Effect = (byte)EffectComboBox.SelectedIndex;
        _pokegold.NotifyDataChanged();
      });
    }
  }
}
