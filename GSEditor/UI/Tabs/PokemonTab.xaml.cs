using GSEditor.Common.Extensions;
using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using GSEditor.UI.Controls;
using GSEditor.ViewModels.Selectors;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace GSEditor.UI.Tabs;

public partial class PokemonTab : UserControl
{
  private readonly IPokegoldService _pokegold = Program.Services.GetRequiredService<IPokegoldService>();
  private readonly IDialogService _dialogs = Program.Services.GetRequiredService<IDialogService>();
  private readonly IPopupMenuService _popupMenus = Program.Services.GetRequiredService<IPopupMenuService>();

  private readonly List<CheckListBox> _tmhmList;

  public PokemonTab()
  {
    InitializeComponent();

    _tmhmList = new()
    {
      TMHM0,
      TMHM1,
      TMHM2,
      TMHM3,
      TMHM4,
      TMHM5,
      TMHM6,
      TMHM7,
    };

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, _) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    var previousSelection = PokemonListBox.SelectedIndex;

    this.RunSafe(() =>
    {
      PokemonListBox.Items.Clear();
      for (var i = 0; i < 251; i++)
      {
        if (i < _pokegold.Data.Strings.PokemonNames.Count)
        {
          var e = _pokegold.Data.Strings.PokemonNames[i];
          PokemonListBox.Items.Add(e);
        }
      }

      Type1ComboBox.Items.Clear();
      Type2ComboBox.Items.Clear();
      foreach (var e in _pokegold.Data.Strings.MoveTypeNames)
      {
        Type1ComboBox.Items.Add(e);
        Type2ComboBox.Items.Add(e);
      }

      Item1ComboBox.Items.Clear();
      Item2ComboBox.Items.Clear();
      Item1ComboBox.Items.Add("없음");
      Item2ComboBox.Items.Add("없음");
      foreach (var e in _pokegold.Data.Strings.ItemNames)
      {
        Item1ComboBox.Items.Add(e);
        Item2ComboBox.Items.Add(e);
      }

      if (_pokegold.IsOpened)
      {
        foreach (var e in _tmhmList)
          e.Items.Clear();

        for (var i = 0; i < 57; i++)
        {
          var label = (i < 50 ? "기술" : "비전");
          var number = (i < 50 ? i + 1 : i - 49).ToString().PadLeft(2, '0');
          var name = _pokegold.Data.Strings.MoveNames[_pokegold.Data.TMHMs[i] - 1];
          _tmhmList[i / 8].Items.Add($"{label}{number} [{name}]");
        }
      }
    });

    PokemonListBox.SelectedIndex = previousSelection;
  }

  private void UpdatePokemonImages()
  {
    var index = PokemonListBox.SelectedIndex;

    FrontImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.Pokemons[index],
      Rows = _pokegold.Data.Pokemons[index].ImageTileSize,
      Columns = _pokegold.Data.Pokemons[index].ImageTileSize,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.Pokemons[index][0],
        _pokegold.Data.Colors.Pokemons[index][1],
        GBColor.GBBlack,
      },
    };

    FrontShinyImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.Pokemons[index],
      Rows = _pokegold.Data.Pokemons[index].ImageTileSize,
      Columns = _pokegold.Data.Pokemons[index].ImageTileSize,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.ShinyPokemons[index][0],
        _pokegold.Data.Colors.ShinyPokemons[index][1],
        GBColor.GBBlack,
      },
    };

    BackImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.PokemonBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.Pokemons[index][0],
        _pokegold.Data.Colors.Pokemons[index][1],
        GBColor.GBBlack,
      },
    };

    BackShinyImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.PokemonBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.ShinyPokemons[index][0],
        _pokegold.Data.Colors.ShinyPokemons[index][1],
        GBColor.GBBlack,
      },
    };

    Color1.SelectedColor = _pokegold.Data.Colors.Pokemons[index][0].ToColor();
    Color2.SelectedColor = _pokegold.Data.Colors.Pokemons[index][1].ToColor();
    ShinyColor1.SelectedColor = _pokegold.Data.Colors.ShinyPokemons[index][0].ToColor();
    ShinyColor2.SelectedColor = _pokegold.Data.Colors.ShinyPokemons[index][1].ToColor();
  }

  private void UpdateEvolutionMoves()
  {
    var index = PokemonListBox.SelectedIndex;

    EvolutionListView.Items.Clear();
    foreach (var e in _pokegold.Data.Pokemons[index].Evolutions)
    {
      EvolutionListView.Items.Add(new EvolutionViewModel
      {
        Pokemon = _pokegold.Data.Strings.PokemonNames[e.PokemonNo - 1],
        Method = e.Type switch
        {
          1 or 5 => "레벨업",
          2 => "도구",
          3 => "통신교환",
          4 => "친밀도 MAX",
          _ => "?",
        },
        Parameter1 = e.Type switch
        {
          1 or 5 => $"레벨 {e.Level} 달성",
          2 => $"\"{_pokegold.Data.Strings.ItemNames[e.ItemNo - 1]}\" 사용",
          3 => e.ItemNo != 0xff ? $"\"{_pokegold.Data.Strings.ItemNames[e.ItemNo - 1]}\" 지닌 상태" : "-",
          4 => e.Affection switch
          {
            1 => "레벨업",
            2 => "낮에 레벨업",
            3 => "밤에 레벨업",
            _ => "?",
          },
          _ => "-",
        },
        Parameter2 = e.Type switch
        {
          5 => e.BaseStats switch
          {
            1 => "공격이 방어보다 높음",
            2 => "방어가 공격보다 높음",
            3 => "공격과 방어가 같음",
            _ => "?",
          },
          _ => "-",
        },
      });
    }

    _pokegold.Data.Pokemons[index].LearnMoves.Sort((a, b) => a.Level.CompareTo(b.Level));

    LearnMoveListView.Items.Clear();
    foreach (var e in _pokegold.Data.Pokemons[index].LearnMoves)
    {
      LearnMoveListView.Items.Add(new LearnMoveViewModel
      {
        Level = $"{e.Level}",
        Move = $"{_pokegold.Data.Strings.MoveNames[e.MoveNo - 1]}",
      });
    }
  }

  private void OnSizeChanged(object _, SizeChangedEventArgs __)
  {
    if (EvolutionListView.View is GridView evolutionGridView)
    {
      var totallyWidth = EvolutionListView.ActualWidth - 32;
      evolutionGridView.Columns[0].Width = totallyWidth * 0.2;
      evolutionGridView.Columns[1].Width = totallyWidth * 0.2;
      evolutionGridView.Columns[2].Width = totallyWidth * 0.3;
      evolutionGridView.Columns[3].Width = totallyWidth * 0.3;
    }

    if (LearnMoveListView.View is GridView learnMoveGridView)
    {
      var totallyWidth = LearnMoveListView.ActualWidth - 32;
      learnMoveGridView.Columns[0].Width = totallyWidth * 0.3;
      learnMoveGridView.Columns[1].Width = totallyWidth * 0.7;
    }

    TMHMContainer.Columns = (int)(ActualWidth / 220);
  }

  private void OnPokemonListBoxSelected(object _, SelectionChangedEventArgs __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        NumberTextBox.Text = $"{index + 1}";
        NameTextBox.Text = _pokegold.Data.Strings.PokemonNames[index];
        Type1ComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].Type1;
        Type2ComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].Type2;
        Item1ComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].Item1;
        Item2ComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].Item2;
        GenderRateComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].GenderRate;
        GrowthRateComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].GrowthRate;
        EggGroup1ComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].EggGroup1;
        EggGroup2ComboBox.SelectedIndex = _pokegold.Data.Pokemons[index].EggGroup2;
        EXPUpDown.Value = _pokegold.Data.Pokemons[index].EXP;
        CatchRateUpDown.Value = _pokegold.Data.Pokemons[index].CatchRate;

        HPUpDown.Value = _pokegold.Data.Pokemons[index].HP;
        AttackUpDown.Value = _pokegold.Data.Pokemons[index].Attack;
        DefenceUpDown.Value = _pokegold.Data.Pokemons[index].Defence;
        SpAttackUpDown.Value = _pokegold.Data.Pokemons[index].SpAttack;
        SpDefenceUpDown.Value = _pokegold.Data.Pokemons[index].SpDefence;
        SpeedUpDown.Value = _pokegold.Data.Pokemons[index].Speed;

        SpecificNameTextBox.Text = _pokegold.Data.Pokedex[index].SpecificName;
        HeightUpDown.Value = _pokegold.Data.Pokedex[index].Height / 10.0;
        WeightUpDown.Value = _pokegold.Data.Pokedex[index].Weight / 10.0;
        DexDescriptionTextBox.Text = _pokegold.Data.Pokedex[index].Description.Replace("[59]", "\n");

        foreach (var e in _tmhmList)
          e.SelectedItems.Clear();

        for (var i = 0; i < 57; i++)
        {
          var item = _tmhmList[i / 8].Items[i % 8];
          if (_pokegold.Data.Pokemons[index].TMHMs[i])
            _tmhmList[i / 8].SelectedItems.Add(item);
        }

        UpdateEvolutionMoves();
        UpdatePokemonImages();
      });
    }

    ContentBorder.IsEnabled = index != -1;
    ImagesGrid.Visibility = index != 200 ? Visibility.Visible : Visibility.Collapsed;
    UnownGrid.Visibility = index == 200 ? Visibility.Visible : Visibility.Collapsed;
  }

  private void OnTextChanged(object _, TextChangedEventArgs __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        if (NameTextBox.Text.TryTextEncode(out var _))
        {
          _pokegold.Data.Strings.PokemonNames[index] = NameTextBox.Text;
          PokemonListBox.Items[index] = _pokegold.Data.Strings.PokemonNames[index];
          PokemonListBox.SelectedIndex = index;
          _pokegold.NotifyDataChanged();
        }

        if (SpecificNameTextBox.Text.TryTextEncode(out var _))
        {
          _pokegold.Data.Pokedex[index].SpecificName = SpecificNameTextBox.Text;
          _pokegold.NotifyDataChanged();
        }

        var realDescription = DexDescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "[59]");
        if (realDescription.TryTextEncode(out var _))
        {
          _pokegold.Data.Pokedex[index].Description = realDescription;
          _pokegold.NotifyDataChanged();
        }
      });
    }

    var maxLength = 0;
    foreach (var line in DexDescriptionTextBox.Text.Split("\n"))
    {
      if (line.Length > maxLength)
        maxLength = line.Length;
    }
    DexDescriptionLabel.Content = $"설명 ({maxLength}/18)：";
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.Data.Pokemons[index].Type1 = (byte)Type1ComboBox.SelectedIndex;
        _pokegold.Data.Pokemons[index].Type2 = (byte)Type2ComboBox.SelectedIndex;
        _pokegold.Data.Pokemons[index].Item1 = (byte)Item1ComboBox.SelectedIndex;
        _pokegold.Data.Pokemons[index].Item2 = (byte)Item2ComboBox.SelectedIndex;
        _pokegold.Data.Pokemons[index].GenderRate = (byte)GenderRateComboBox.SelectedIndex;
        _pokegold.Data.Pokemons[index].GrowthRate = (byte)GrowthRateComboBox.SelectedIndex;
        _pokegold.Data.Pokemons[index].EggGroup1 = (byte)EggGroup1ComboBox.SelectedIndex;
        _pokegold.Data.Pokemons[index].EggGroup2 = (byte)EggGroup2ComboBox.SelectedIndex;
        _pokegold.NotifyDataChanged();
      });
    }
  }

  private void OnUpDownValueChanged(object _, RoutedPropertyChangedEventArgs<object> __)
  {
    var index = PokemonListBox.SelectedIndex;

    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.Data.Pokemons[index].HP = (byte)(HPUpDown.Value ?? 0);
        _pokegold.Data.Pokemons[index].Attack = (byte)(AttackUpDown.Value ?? 0);
        _pokegold.Data.Pokemons[index].Defence = (byte)(DefenceUpDown.Value ?? 0);
        _pokegold.Data.Pokemons[index].SpAttack = (byte)(SpAttackUpDown.Value ?? 0);
        _pokegold.Data.Pokemons[index].SpDefence = (byte)(SpDefenceUpDown.Value ?? 0);
        _pokegold.Data.Pokemons[index].Speed = (byte)(SpeedUpDown.Value ?? 0);

        _pokegold.Data.Pokemons[index].EXP = (byte)(EXPUpDown.Value ?? 0);
        _pokegold.Data.Pokemons[index].CatchRate = (byte)(CatchRateUpDown.Value ?? 0);

        _pokegold.Data.Pokedex[index].Height = (byte)((HeightUpDown.Value ?? 0) * 10);
        _pokegold.Data.Pokedex[index].Weight = (int)((WeightUpDown.Value ?? 0) * 10);

        _pokegold.NotifyDataChanged();
      });

      // 포획률 퍼센티지 변경
      var percentage = string.Format("{0:P2}", (double)_pokegold.Data.Pokemons[index].CatchRate / 0xff);
      CatchRatePercentageLabel.Content = percentage;
    }
  }

  private void OnImageClick(object sender, RoutedEventArgs _)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index == -1)
    {
      _dialogs.ShowError("오류", "잘못된 접근입니다.");
      return;
    }

    if (sender is Button button && button.Content is IgnoreDpiPanel panel && panel.Child is GBImageBox gbImageBox)
    {
      _popupMenus.Show(new PopupMenuItem.Builder()
        .Add("이미지 변경...", () =>
        {
          var fileName = _dialogs.ShowOpenFile("이미지 변경", "png 파일|*png");
          if (fileName == null)
            return;

          if (!GBImage.TryLoadFromFile(fileName, out var newImage) || newImage == null)
          {
            _dialogs.ShowError("오류", "이미지 파일의 형식이 올바르지 않습니다.");
            return;
          }

          if (gbImageBox.Name == nameof(FrontImage) || gbImageBox.Name == nameof(FrontShinyImage))
          {
            if (newImage.Columns != newImage.Rows || newImage.Columns < 5 || newImage.Columns > 7 || newImage.Rows < 5 || newImage.Rows > 7)
            {
              _dialogs.ShowError("오류", "이미지 사이즈가 올바르지 않습니다.\n40x40, 48x48, 56x56 중 하나의 사이즈로 맞춰주세요.");
              return;
            }

            _pokegold.Data.Images.Pokemons[index] = newImage.Source;
            _pokegold.Data.Pokemons[index].ImageTileSize = (byte)newImage.Rows;
          }

          if (gbImageBox.Name == nameof(BackImage) || gbImageBox.Name == nameof(BackShinyImage))
          {
            if (newImage.Columns != 6 || newImage.Rows != 6)
            {
              _dialogs.ShowError("오류", "이미지 사이즈가 올바르지 않습니다.\n48x48 사이즈로 맞춰주세요.");
              return;
            }

            _pokegold.Data.Images.PokemonBacksides[index] = newImage.Source;
          }

          if (gbImageBox.Name == nameof(FrontShinyImage) || gbImageBox.Name == nameof(BackShinyImage))
          {
            _pokegold.Data.Colors.ShinyPokemons[index][0] = newImage.Colors[1];
            _pokegold.Data.Colors.ShinyPokemons[index][1] = newImage.Colors[2];
          }
          else
          {
            _pokegold.Data.Colors.Pokemons[index][0] = newImage.Colors[1];
            _pokegold.Data.Colors.Pokemons[index][1] = newImage.Colors[2];
          }

          this.RunSafe(() =>
          {
            UpdatePokemonImages();
            _pokegold.NotifyDataChanged();
          });
        })
        .Add("-")
        .Add("png 저장...", () =>
        {
          var fileName = _dialogs.ShowSaveFile("png 저장", "png 파일|*png", $"{button.Tag}_{index + 1}.png");
          if (fileName != null)
            gbImageBox.GBImage!.WriteFile(fileName);
        })
        .Add("2bpp 저장...", () =>
        {
          var fileName = _dialogs.ShowSaveFile("2bpp 저장", "2bpp 파일|*2bpp|bin 파일|*.bin|모든 파일|*.*", $"{button.Tag}_{index + 1}.2bpp");
          if (fileName != null)
            File.WriteAllBytes(fileName, gbImageBox.GBImage!.Source!);
        })
        .Create());
    }
  }

  private void OnColorPickerValueChanged(object _, RoutedPropertyChangedEventArgs<Color?> __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.Data.Colors.Pokemons[index][0] = new GBColor(Color1.SelectedColor!.Value);
        _pokegold.Data.Colors.Pokemons[index][1] = new GBColor(Color2.SelectedColor!.Value);
        _pokegold.Data.Colors.ShinyPokemons[index][0] = new GBColor(ShinyColor1.SelectedColor!.Value);
        _pokegold.Data.Colors.ShinyPokemons[index][1] = new GBColor(ShinyColor2.SelectedColor!.Value);
        UpdatePokemonImages();
        _pokegold.NotifyDataChanged();
      });
    }
  }

  private void OnEvolutionLearnMoveSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    EvolutionEditButton.IsEnabled = EvolutionListView.SelectedIndex != -1;
    EvolutionRemoveButton.IsEnabled = EvolutionListView.SelectedIndex != -1;
    LearnMoveEditButton.IsEnabled = LearnMoveListView.SelectedIndex != -1;
    LearnMoveRemoveButton.IsEnabled = LearnMoveListView.SelectedIndex != -1;
  }

  private void OnEvolutionListViewDoubleClick(object _, MouseButtonEventArgs e)
  {
    OnEvolutionEdit();
  }

  private void OnEvolutionButtonClick(object sender, RoutedEventArgs __)
  {
    if (sender is Button button)
    {
      if (button.Name == nameof(EvolutionAddButton))
      {
        var result = _dialogs.ShowEvolutionEditor(null);
        if (result != null)
        {
          var index = PokemonListBox.SelectedIndex;
          _pokegold.Data.Pokemons[index].Evolutions.Add(result);
          _pokegold.NotifyDataChanged();
          UpdateEvolutionMoves();
        }
        return;
      }

      if (button.Name == nameof(EvolutionEditButton))
      {
        OnEvolutionEdit();
        return;
      }

      if (button.Name == nameof(EvolutionRemoveButton))
      {
        var index = EvolutionListView.SelectedIndex;
        if (index != -1)
        {
          if (!_dialogs.ShowQuestion("알림", "정말로 삭제하겠습니까?"))
            return;

          _pokegold.Data.Pokemons[PokemonListBox.SelectedIndex].Evolutions.RemoveAt(index);
          _pokegold.NotifyDataChanged();
          UpdateEvolutionMoves();
        }
        return;
      }

      if (button.Name == nameof(EvolutionClearButton))
      {
        if (!_dialogs.ShowQuestion("알림", "정말로 전부 삭제하겠습니까?"))
          return;

        _pokegold.Data.Pokemons[PokemonListBox.SelectedIndex].Evolutions.Clear();
        _pokegold.NotifyDataChanged();
        UpdateEvolutionMoves();
        return;
      }
    }
  }

  private void OnEvolutionEdit()
  {
    var index = PokemonListBox.SelectedIndex;
    var evIndex = EvolutionListView.SelectedIndex;

    if (evIndex != -1)
    {
      var result = _dialogs.ShowEvolutionEditor(_pokegold.Data.Pokemons[index].Evolutions[evIndex]);
      if (result != null)
      {
        _pokegold.Data.Pokemons[index].Evolutions[evIndex] = result;
        _pokegold.NotifyDataChanged();
        UpdateEvolutionMoves();
      }
    }
  }

  private void OnLearnMoveListViewDoubleClick(object _, MouseButtonEventArgs e)
  {
    OnLearnMoveEdit();
  }

  private void OnLearnMoveButtonClick(object sender, RoutedEventArgs __)
  {
    if (sender is Button button)
    {
      if (button.Name == nameof(LearnMoveAddButton))
      {
        var result = _dialogs.ShowLearnMoveEditor(null);
        if (result != null)
        {
          var index = PokemonListBox.SelectedIndex;
          _pokegold.Data.Pokemons[index].LearnMoves.Add(result);
          _pokegold.NotifyDataChanged();
          UpdateEvolutionMoves();
        }
        return;
      }

      if (button.Name == nameof(LearnMoveEditButton))
      {
        OnLearnMoveEdit();
        return;
      }

      if (button.Name == nameof(LearnMoveRemoveButton))
      {
        var index = LearnMoveListView.SelectedIndex;
        if (index != -1)
        {
          if (!_dialogs.ShowQuestion("알림", "정말로 삭제하겠습니까?"))
            return;

          _pokegold.Data.Pokemons[PokemonListBox.SelectedIndex].LearnMoves.RemoveAt(index);
          _pokegold.NotifyDataChanged();
          UpdateEvolutionMoves();
        }
        return;
      }

      if (button.Name == nameof(LearnMoveClearButton))
      {
        if (!_dialogs.ShowQuestion("알림", "정말로 전부 삭제하겠습니까?"))
          return;

        _pokegold.Data.Pokemons[PokemonListBox.SelectedIndex].LearnMoves.Clear();
        _pokegold.NotifyDataChanged();
        UpdateEvolutionMoves();
        return;
      }

      if (button.Name == nameof(LearnMoveImportButton))
      {
        var result = _dialogs.ShowLearnMoveImporter();
        foreach (var e in result)
        {
          foreach (var e2 in _pokegold.Data.Pokemons[PokemonListBox.SelectedIndex].LearnMoves)
          {
            if (e2.Level == e.Level && e2.MoveNo == e.MoveNo)
              goto ignoreSameItem;
          }

          _pokegold.Data.Pokemons[PokemonListBox.SelectedIndex].LearnMoves.Add(e);
        ignoreSameItem:;
        }

        _pokegold.NotifyDataChanged();
        UpdateEvolutionMoves();
        return;
      }
    }
  }

  private void OnLearnMoveEdit()
  {
    var index = PokemonListBox.SelectedIndex;
    var lmIndex = LearnMoveListView.SelectedIndex;

    if (lmIndex != -1)
    {
      var result = _dialogs.ShowLearnMoveEditor(_pokegold.Data.Pokemons[index].LearnMoves[lmIndex]);
      if (result != null)
      {
        _pokegold.Data.Pokemons[index].LearnMoves[lmIndex] = result;
        _pokegold.NotifyDataChanged();
        UpdateEvolutionMoves();
      }
    }
  }

  private void OnTMHMsSelectionChanged(object _, ItemSelectionChangedEventArgs __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        for (var i = 0; i < 57; i++)
          _pokegold.Data.Pokemons[index].TMHMs[i] = false;

        for (var i = 0; i < _tmhmList.Count; i++)
        {
          foreach (var e in _tmhmList[i].SelectedItems)
            _pokegold.Data.Pokemons[index].TMHMs[(i * 8) + _tmhmList[i].Items.IndexOf(e)] = true;
        }

        _pokegold.NotifyDataChanged();
      });
    }
  }

  private void OnTMHMButtonClick(object sender, RoutedEventArgs _)
  {
    var index = PokemonListBox.SelectedIndex;
    if (index != -1)
    {
      if (sender is Button button)
      {
        if (button.Name == nameof(TMHMCheckAllButton))
        {
          foreach (var e in _tmhmList)
            e.SelectedItems.Clear();

          for (var i = 0; i < 57; i++)
          {
            var item = _tmhmList[i / 8].Items[i % 8];
            _tmhmList[i / 8].SelectedItems.Add(item);
          }
        }

        if (button.Name == nameof(TMHMClearButton))
        {
          foreach (var e in _tmhmList)
            e.SelectedItems.Clear();
        }
      }
    }
  }
}
