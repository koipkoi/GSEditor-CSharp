using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace GSEditor.UI.Windows;

public partial class EvolutionEditorDialog : Window
{
  private readonly IPokegoldService _pokegold = App.Services.GetRequiredService<IPokegoldService>();

  public Evolution? Result { get; set; }

  public EvolutionEditorDialog(Evolution? currentItem)
  {
    Title = currentItem != null ? "진화 수정" : "진화 추가";

    InitializeComponent();

    for (var i = 0; i < 251; i++)
    {
      var e = _pokegold.Data.Strings.PokemonNames[i];
      StatsPokemonComboBox.Items.Add(e);
      ItemPokemonComboBox.Items.Add(e);
      ExchangePokemonComboBox.Items.Add(e);
      AffectionPokemonComboBox.Items.Add(e);
    }

    foreach (var e in _pokegold.Data.Strings.ItemNames)
    {
      ItemItemComboBox.Items.Add(e);
      ExchangeItemComboBox.Items.Add(e);
    }

    StatsPokemonComboBox.SelectedIndex = 0;
    ItemItemComboBox.SelectedIndex = 0;
    ItemPokemonComboBox.SelectedIndex = 0;
    ExchangeItemComboBox.SelectedIndex = 0;
    ExchangePokemonComboBox.SelectedIndex = 0;
    AffectionPokemonComboBox.SelectedIndex = 0;

    Stats0.IsChecked = true;
    Stats1.IsChecked = false;
    Stats2.IsChecked = false;
    Stats3.IsChecked = false;

    StatsLevel.Value = 5;

    if (currentItem != null)
    {
      ItemPokemonComboBox.SelectedIndex = currentItem.PokemonNo - 1;
      ExchangePokemonComboBox.SelectedIndex = currentItem.PokemonNo - 1;
      AffectionPokemonComboBox.SelectedIndex = currentItem.PokemonNo - 1;
      StatsPokemonComboBox.SelectedIndex = currentItem.PokemonNo - 1;

      switch (currentItem.Type)
      {
        case 1:
          StatsLevel.Value = currentItem.Level;
          MainTab.SelectedIndex = 0;
          break;

        case 2:
          ItemItemComboBox.SelectedIndex = currentItem.ItemNo - 1;
          MainTab.SelectedIndex = 1;
          break;

        case 3:
          ExchangeItemComboBox.SelectedIndex = currentItem.ItemNo - 1;
          ExchangeNeedItemCheckBox.IsChecked = currentItem.ItemNo != 0xff;
          MainTab.SelectedIndex = 2;
          break;

        case 4:
          Affection1.IsChecked = currentItem.Affection == 1;
          Affection2.IsChecked = currentItem.Affection == 2;
          Affection3.IsChecked = currentItem.Affection == 3;
          MainTab.SelectedIndex = 3;
          break;

        case 5:
          StatsLevel.Value = currentItem.Level;
          Stats0.IsChecked = false;
          Stats1.IsChecked = currentItem.BaseStats == 1;
          Stats2.IsChecked = currentItem.BaseStats == 2;
          Stats3.IsChecked = currentItem.BaseStats == 3;
          MainTab.SelectedIndex = 0;
          break;
      }
    }
  }

  private void OnPokemonComboBoxSelectionChanged(object sender, SelectionChangedEventArgs _)
  {
    // 선택한 항목이 다른 탭에서도 적용되게끔 함
    if (sender is ComboBox comboBox)
    {
      var selected = comboBox.SelectedIndex;
      ItemPokemonComboBox.SelectedIndex = selected;
      ExchangePokemonComboBox.SelectedIndex = selected;
      AffectionPokemonComboBox.SelectedIndex = selected;
      StatsPokemonComboBox.SelectedIndex = selected;
    }
  }

  private void OnCancelClick(object _, RoutedEventArgs __)
  {
    Close();
  }

  private void OnConfirmClick(object _, RoutedEventArgs __)
  {
    switch (MainTab.SelectedIndex)
    {
      case 0:
        if (Stats0.IsChecked ?? true)
        {
          Result = new()
          {
            Type = 1,
            Level = (byte)(StatsLevel.Value ?? 0),
            PokemonNo = (byte)(StatsPokemonComboBox.SelectedIndex + 1),
          };
        }
        else
        {
          Result = new()
          {
            Type = 5,
            Level = (byte)(StatsLevel.Value ?? 0),
            BaseStats = (byte)((Stats1.IsChecked ?? true) ? 1 : ((Stats2.IsChecked ?? true) ? 2 : 3)),
            PokemonNo = (byte)(StatsPokemonComboBox.SelectedIndex + 1),
          };
        }
        DialogResult = true;
        Close();
        break;

      case 1:
        Result = new()
        {
          Type = 2,
          ItemNo = (byte)(ItemItemComboBox.SelectedIndex + 1),
          PokemonNo = (byte)(ItemPokemonComboBox.SelectedIndex + 1),
        };
        DialogResult = true;
        Close();
        break;

      case 2:
        Result = new()
        {
          Type = 3,
          ItemNo = (byte)((ExchangeNeedItemCheckBox.IsChecked ?? true) ? ExchangeItemComboBox.SelectedIndex + 1 : 0xff),
          PokemonNo = (byte)(ExchangePokemonComboBox.SelectedIndex + 1),
        };
        DialogResult = true;
        Close();
        break;

      case 3:
        Result = new()
        {
          Type = 4,
          Affection = (byte)((Affection1.IsChecked ?? true) ? 1 : ((Affection2.IsChecked ?? true) ? 2 : 3)),
          PokemonNo = (byte)(AffectionPokemonComboBox.SelectedIndex + 1),
        };
        DialogResult = true;
        Close();
        break;
    }
  }
}
