using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using GSEditor.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Tabs;

public partial class MovesTab : UserControl
{
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();

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
    var previousEffect = EffectComboBox.SelectedIndex;

    this.RunSafe(() =>
    {
      MovesListBox.Items.Clear();
      foreach (var e in _pokegold.Strings.MoveNames)
        MovesListBox.Items.Add(e);

      TypeComboBox.Items.Clear();
      foreach (var e in _pokegold.Strings.TypeNames)
        TypeComboBox.Items.Add(e);

      EffectComboBox.Items.Clear();
      foreach (var e in Properties.Resources.MoveEffects.Replace("\r\n", "\n").Split("\n"))
        EffectComboBox.Items.Add(e);
    });

    MovesListBox.SelectedIndex = previousMovesSelection;
    TypeComboBox.SelectedIndex = previousTypeSelection;
    EffectComboBox.SelectedIndex = previousEffect;
  }

  private void OnMovesListBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = MovesListBox.SelectedIndex;
      if (index != -1)
      {
        NoTextBox.Text = $"{_pokegold.Moves[index].No}";
        NameTextBox.Text = _pokegold.Strings.MoveNames[index];
        DescriptionTextBox.Text = _pokegold.Strings.MoveDescriptions[index].Replace("[59]", "\n");

        TypeComboBox.SelectedIndex = _pokegold.Moves[index].MoveType;
        PPUpDown.Value = _pokegold.Moves[index].PP;
        PowerUpDown.Value = _pokegold.Moves[index].Power;
        AccuracyUpDown.Value = _pokegold.Moves[index].Accuracy;

        EffectComboBox.SelectedIndex = _pokegold.Moves[index].Effect;
        EffectUpDown.Value = _pokegold.Moves[index].EffectChance;
      }

      ContentPanel.IsEnabled = index != -1;
    });
  }

  private void OnTextBoxTextChanged(object _, TextChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = MovesListBox.SelectedIndex;

      if (NameTextBox.Text.TryTextEncode(out var _))
      {
        var previousSelection = MovesListBox.SelectedIndex;
        _pokegold.Strings.MoveNames[index] = NameTextBox.Text;
        MovesListBox.Items[index] = _pokegold.Strings.MoveNames[index];
        MovesListBox.SelectedIndex = previousSelection;
        _pokegold.NotifyDataChanged();
      }

      var maxLength = 0;
      foreach (var line in DescriptionTextBox.Text.Split("\n"))
      {
        if (line.Length > maxLength)
          maxLength = line.Length;
      }
      DescriptionLabel.Content = $"설명 ({maxLength}/18)：";

      var realDescription = DescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "[59]");
      if (realDescription.TryTextEncode(out var _))
      {
        _pokegold.Strings.MoveDescriptions[index] = realDescription;
        _pokegold.NotifyDataChanged();
      }
    });
  }

  private void OnUpDownValueChaged(object _, RoutedPropertyChangedEventArgs<object> __)
  {
    var index = MovesListBox.SelectedIndex;

    this.RunSafe(() =>
    {
      if (index != -1)
      {
        _pokegold.Moves[index].PP = (byte)(PPUpDown.Value ?? 0);
        _pokegold.Moves[index].Power = (byte)(PowerUpDown.Value ?? 0);
        _pokegold.Moves[index].Accuracy = (byte)(AccuracyUpDown.Value ?? 0);
        _pokegold.Moves[index].EffectChance = (byte)(EffectUpDown.Value ?? 0);
        _pokegold.NotifyDataChanged();
      }
    });

    // 명중률 퍼센티지 변경
    if (index != -1)
    {
      var accuracyPercentage = string.Format("{0:P2}", (double)_pokegold.Moves[index].Accuracy / 0xff);
      AccuracyPercentageLabel.Content = accuracyPercentage;
    }
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = MovesListBox.SelectedIndex;
      if (index != -1)
      {
        _pokegold.Moves[index].MoveType = (byte)TypeComboBox.SelectedIndex;
        _pokegold.Moves[index].Effect = (byte)EffectComboBox.SelectedIndex;
        _pokegold.NotifyDataChanged();
      }
    });
  }
}
