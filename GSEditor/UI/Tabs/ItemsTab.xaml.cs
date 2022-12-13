using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using GSEditor.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Tabs;

public partial class ItemsTab : UserControl
{
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();

  public ItemsTab()
  {
    InitializeComponent();

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, __) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    var previousItemsSelection = ItemsListBox.SelectedIndex;

    this.RunSafe(() =>
    {
      ItemsListBox.Items.Clear();
      foreach (var e in _pokegold.Strings.ItemNames)
        ItemsListBox.Items.Add(e);
    });

    ItemsListBox.SelectedIndex = previousItemsSelection;
  }

  private void OnItemsSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = ItemsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        NameTextBox.Text = _pokegold.Strings.ItemNames[index];
        PriceUpDown.Value = _pokegold.Items[index].Price;
        GroupComboBox.SelectedIndex = _pokegold.Items[index].Pocket;
        DescriptionTextBox.Text = _pokegold.Strings.ItemDescriptions[index].Replace("[59]", "\n");

        FieldMenuComboBox.SelectedIndex = _pokegold.Items[index].FieldMenu;
        BattleMenuComboBox.SelectedIndex = _pokegold.Items[index].BattleMenu;

        GiveEffectComboBox.SelectedIndex = _pokegold.Items[index].Effect;
        RegisterAndSellComboBox.SelectedIndex = _pokegold.Items[index].Property >> 6;
        ItemParameterUpDown.Value = _pokegold.Items[index].Parameter;
      });
    }

    ContentBorder.IsEnabled = index != -1;
  }

  private void OnTextBoxTextChanged(object _, TextChangedEventArgs __)
  {
    var index = ItemsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        if (NameTextBox.Text.TryTextEncode(out var _))
        {
          var previousSelection = ItemsListBox.SelectedIndex;
          _pokegold.Strings.ItemNames[index] = NameTextBox.Text;
          ItemsListBox.Items[index] = _pokegold.Strings.ItemNames[index];
          ItemsListBox.SelectedIndex = previousSelection;
          _pokegold.NotifyDataChanged();
        }

        var realDescription = DescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "[59]");
        if (realDescription.TryTextEncode(out var _))
        {
          _pokegold.Strings.ItemDescriptions[index] = realDescription;
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
    var index = ItemsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.Items[index].Price = PriceUpDown.Value ?? 0;
        _pokegold.Items[index].Parameter = ItemParameterUpDown.Value ?? 0;

        _pokegold.NotifyDataChanged();
      });
    }
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = ItemsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
    {
      _pokegold.Items[index].FieldMenu = (byte)FieldMenuComboBox.SelectedIndex;
      _pokegold.Items[index].BattleMenu = (byte)BattleMenuComboBox.SelectedIndex;

      _pokegold.Items[index].Effect = (byte)GiveEffectComboBox.SelectedIndex;
      _pokegold.Items[index].Pocket = (byte)GroupComboBox.SelectedIndex;
      _pokegold.Items[index].Property = (byte)(RegisterAndSellComboBox.SelectedIndex << 6);

      _pokegold.NotifyDataChanged();
    });
    }
  }
}
