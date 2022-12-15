using GSEditor.Common.Extensions;
using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Tabs;

public partial class ItemsTab : UserControl
{
  private readonly IPokegoldService _pokegold = App.Services.GetRequiredService<IPokegoldService>();

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
      foreach (var e in _pokegold.Data.Strings.ItemNames)
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
        NameTextBox.Text = _pokegold.Data.Strings.ItemNames[index];
        PriceUpDown.Value = _pokegold.Data.Items[index].Price;
        GroupComboBox.SelectedIndex = _pokegold.Data.Items[index].Pocket;
        DescriptionTextBox.Text = _pokegold.Data.Strings.ItemDescriptions[index].Replace("[59]", "\n");

        FieldMenuComboBox.SelectedIndex = _pokegold.Data.Items[index].FieldMenu;
        BattleMenuComboBox.SelectedIndex = _pokegold.Data.Items[index].BattleMenu;

        GiveEffectComboBox.SelectedIndex = _pokegold.Data.Items[index].Effect;
        RegisterAndSellComboBox.SelectedIndex = _pokegold.Data.Items[index].Property >> 6;
        ItemParameterUpDown.Value = _pokegold.Data.Items[index].Parameter;
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
          _pokegold.Data.Strings.ItemNames[index] = NameTextBox.Text;
          ItemsListBox.Items[index] = _pokegold.Data.Strings.ItemNames[index];
          ItemsListBox.SelectedIndex = previousSelection;
          _pokegold.NotifyDataChanged();
        }

        var realDescription = DescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "[59]");
        if (realDescription.TryTextEncode(out var _))
        {
          _pokegold.Data.Strings.ItemDescriptions[index] = realDescription;
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
        _pokegold.Data.Items[index].Price = PriceUpDown.Value ?? 0;
        _pokegold.Data.Items[index].Parameter = ItemParameterUpDown.Value ?? 0;
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
        _pokegold.Data.Items[index].FieldMenu = (byte)FieldMenuComboBox.SelectedIndex;
        _pokegold.Data.Items[index].BattleMenu = (byte)BattleMenuComboBox.SelectedIndex;
        _pokegold.Data.Items[index].Effect = (byte)GiveEffectComboBox.SelectedIndex;
        _pokegold.Data.Items[index].Pocket = (byte)GroupComboBox.SelectedIndex;
        _pokegold.Data.Items[index].Property = (byte)(RegisterAndSellComboBox.SelectedIndex << 6);
        _pokegold.NotifyDataChanged();
      });
    }
  }
}
