using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Tabs;

public partial class MovesTab : UserControl, INotifyPropertyChanged
{
  private readonly Pokegold _pokegold = Injector.Get<Pokegold>();
  private bool _selfChanged = false;

  public event PropertyChangedEventHandler? PropertyChanged;

  public bool IsListSelected { get; set; } = false;

  public MovesTab()
  {
    InitializeComponent();

    DataContext = this;

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, __) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    _selfChanged = true;

    var previousMovesSelection = MovesListBox.SelectedIndex;
    MovesListBox.Items.Clear();
    foreach (var e in _pokegold.Strings.MoveNames)
      MovesListBox.Items.Add(e);
    MovesListBox.SelectedIndex = previousMovesSelection;

    var previousTypeSelection = TypeComboBox.SelectedIndex;
    TypeComboBox.Items.Clear();
    foreach (var e in _pokegold.Strings.TypeNames)
      TypeComboBox.Items.Add(e);
    TypeComboBox.SelectedIndex = previousTypeSelection;

    var previousEffect = EffectComboBox.SelectedIndex;
    EffectComboBox.Items.Clear();
    foreach (var e in Properties.Resources.MoveEffects.Replace("\r\n", "\n").Split("\n"))
      EffectComboBox.Items.Add(e);
    EffectComboBox.SelectedIndex = previousEffect;

    _selfChanged = false;
  }

  private void OnMovesListBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = MovesListBox.SelectedIndex;
    if (!_selfChanged)
    {
      _selfChanged = true;

      NoTextBox.Text = $"{_pokegold.Moves[index].No}";
      NameTextBox.Text = _pokegold.Strings.MoveNames[index];
      DescriptionTextBox.Text = _pokegold.Strings.MoveDescriptions[index].Replace("[59]", "\n");

      TypeComboBox.SelectedIndex = _pokegold.Moves[index].MoveType;
      PPUpDown.Value = _pokegold.Moves[index].PP;
      PowerUpDown.Value = _pokegold.Moves[index].Power;
      AccuracyUpDown.Value = _pokegold.Moves[index].Accuracy;

      EffectComboBox.SelectedIndex = _pokegold.Moves[index].Effect;
      EffectUpDown.Value = _pokegold.Moves[index].EffectChance;

      IsListSelected = true;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListSelected)));

      _selfChanged = false;
    }
  }

  private void OnTextBoxTextChanged(object _, TextChangedEventArgs __)
  {
    var index = MovesListBox.SelectedIndex;

    if (!_selfChanged && NameTextBox.Text.TryTextEncode(out var _))
    {
      _selfChanged = true;
      var previousSelection = MovesListBox.SelectedIndex;
      _pokegold.Strings.MoveNames[index] = NameTextBox.Text;
      MovesListBox.Items[index] = _pokegold.Strings.MoveNames[index];
      MovesListBox.SelectedIndex = previousSelection;
      _selfChanged = false;
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
    if (!_selfChanged && realDescription.TryTextEncode(out var _))
    {
      _pokegold.Strings.MoveDescriptions[index] = realDescription;
      _pokegold.NotifyDataChanged();
    }
  }

  private void OnUpDownValueChaged(object _, RoutedPropertyChangedEventArgs<object> __)
  {
    var index = MovesListBox.SelectedIndex;
    if (!_selfChanged && index != -1)
    {
      _pokegold.Moves[index].PP = (byte)(PPUpDown.Value ?? 0);
      _pokegold.Moves[index].Power = (byte)(PowerUpDown.Value ?? 0);
      _pokegold.Moves[index].Accuracy = (byte)(AccuracyUpDown.Value ?? 0);
      _pokegold.Moves[index].EffectChance = (byte)(EffectUpDown.Value ?? 0);
      _pokegold.NotifyDataChanged();
    }

    // 명중률 퍼센티지 변경
    if (index != -1)
    {
      var accuracyPercentage = string.Format("{0:P2}", (double)_pokegold.Moves[index].Accuracy / 0xff);
      AccuracyPercentageLabel.Content = accuracyPercentage;
    }
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = MovesListBox.SelectedIndex;
    if (!_selfChanged && index != -1)
    {
      _pokegold.Moves[index].MoveType = (byte)TypeComboBox.SelectedIndex;
      _pokegold.Moves[index].Effect = (byte)EffectComboBox.SelectedIndex;
      _pokegold.NotifyDataChanged();
    }
  }
}
