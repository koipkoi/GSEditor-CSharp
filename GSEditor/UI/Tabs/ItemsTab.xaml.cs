using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Tabs;

public partial class ItemsTab : UserControl, INotifyPropertyChanged
{
  private readonly Pokegold _pokegold = Injector.Get<Pokegold>();
  private bool _selfChanged = false;

  public event PropertyChangedEventHandler? PropertyChanged;

  public bool IsListSelected { get; set; } = false;

  public ItemsTab()
  {
    InitializeComponent();

    DataContext = this;

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, __) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    _selfChanged = true;

    var previousItemsSelection = ItemsListBox.SelectedIndex;
    ItemsListBox.Items.Clear();
    foreach (var e in _pokegold.Strings.ItemNames)
      ItemsListBox.Items.Add(e);
    ItemsListBox.SelectedIndex = previousItemsSelection;

    var previousgiveEffectSelection = GiveEffectComboBox.SelectedIndex;
    GiveEffectComboBox.Items.Clear();
    foreach (var e in Properties.Resources.GiveItemEffects.Replace("\r\n", "\n").Split("\n"))
      GiveEffectComboBox.Items.Add(e);
    GiveEffectComboBox.SelectedIndex = previousgiveEffectSelection;

    _selfChanged = false;
  }

  private void OnItemsSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = ItemsListBox.SelectedIndex;
    if (!_selfChanged)
    {
      _selfChanged = true;

      NameTextBox.Text = _pokegold.Strings.ItemNames[index];
      PriceUpDown.Value = _pokegold.Items[index].Price;
      GroupComboBox.SelectedIndex = _pokegold.Items[index].Pocket;
      DescriptionTextBox.Text = _pokegold.Strings.ItemDescriptions[index].Replace("[59]", "\n");

      FieldMenuComboBox.SelectedIndex = _pokegold.Items[index].FieldMenu;
      BattleMenuComboBox.SelectedIndex = _pokegold.Items[index].BattleMenu;

      GiveEffectComboBox.SelectedIndex = _pokegold.Items[index].Effect;
      RegisterAndSellComboBox.SelectedIndex = _pokegold.Items[index].Property >> 6;
      ItemParameterUpDown.Value = _pokegold.Items[index].Parameter;

      IsListSelected = true;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListSelected)));

      _selfChanged = false;
    }
  }

  private void OnTextBoxTextChanged(object _, TextChangedEventArgs __)
  {
    var index = ItemsListBox.SelectedIndex;

    if (!_selfChanged && NameTextBox.Text.TryTextEncode(out var _))
    {
      _selfChanged = true;
      var previousSelection = ItemsListBox.SelectedIndex;
      _pokegold.Strings.ItemNames[index] = NameTextBox.Text;
      ItemsListBox.Items[index] = _pokegold.Strings.ItemNames[index];
      ItemsListBox.SelectedIndex = previousSelection;
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
      _pokegold.Strings.ItemDescriptions[index] = realDescription;
      _pokegold.NotifyDataChanged();
    }
  }

  private void OnUpDownValueChaged(object _, RoutedPropertyChangedEventArgs<object> __)
  {
    var index = ItemsListBox.SelectedIndex;
    if (!_selfChanged && index != -1)
    {
      _pokegold.Items[index].Price = PriceUpDown.Value ?? 0;
      _pokegold.Items[index].Parameter = ItemParameterUpDown.Value ?? 0;

      _pokegold.NotifyDataChanged();
    }
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = ItemsListBox.SelectedIndex;
    if (!_selfChanged && index != -1)
    {
      _pokegold.Items[index].FieldMenu = (byte)FieldMenuComboBox.SelectedIndex;
      _pokegold.Items[index].BattleMenu = (byte)BattleMenuComboBox.SelectedIndex;

      _pokegold.Items[index].Effect = (byte)GiveEffectComboBox.SelectedIndex;
      _pokegold.Items[index].Pocket = (byte)GroupComboBox.SelectedIndex;
      _pokegold.Items[index].Property = (byte)(RegisterAndSellComboBox.SelectedIndex << 6);

      _pokegold.NotifyDataChanged();
    }
  }
}
