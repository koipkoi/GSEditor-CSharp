using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using GSEditor.UI.Controls;
using GSEditor.UI.Windows;
using GSEditor.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
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
  private readonly System.Drawing.Color _pokemonWhiteColor = GBColor.FromBytes(new byte[] { 0xff, 0x7f, }).ToColor();
  private readonly System.Drawing.Color _pokemonBlackColor = GBColor.FromBytes(new byte[] { 0x00, 0x00, }).ToColor();
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();
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
        if (i < _pokegold.Strings.PokemonNames.Count)
        {
          var e = _pokegold.Strings.PokemonNames[i];
          PokemonListBox.Items.Add(e);
        }
      }

      Type1ComboBox.Items.Clear();
      Type2ComboBox.Items.Clear();
      foreach (var e in _pokegold.Strings.TypeNames)
      {
        Type1ComboBox.Items.Add(e);
        Type2ComboBox.Items.Add(e);
      }

      Item1ComboBox.Items.Clear();
      Item2ComboBox.Items.Clear();
      Item1ComboBox.Items.Add("없음");
      Item2ComboBox.Items.Add("없음");
      foreach (var e in _pokegold.Strings.ItemNames)
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
          var name = _pokegold.Strings.MoveNames[_pokegold.TMHMs[i] - 1];
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
      Source = _pokegold.Images.Pokemons[index],
      Rows = _pokegold.Pokemons[index].GetImageTileSize(),
      Columns = _pokegold.Pokemons[index].GetImageTileSize(),
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.Pokemons[index][0].ToColor(),
        _pokegold.Colors.Pokemons[index][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    FrontShinyImage.GBImage = new()
    {
      Source = _pokegold.Images.Pokemons[index],
      Rows = _pokegold.Pokemons[index].GetImageTileSize(),
      Columns = _pokegold.Pokemons[index].GetImageTileSize(),
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.ShinyPokemons[index][0].ToColor(),
        _pokegold.Colors.ShinyPokemons[index][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    BackImage.GBImage = new()
    {
      Source = _pokegold.Images.PokemonBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.Pokemons[index][0].ToColor(),
        _pokegold.Colors.Pokemons[index][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    BackShinyImage.GBImage = new()
    {
      Source = _pokegold.Images.PokemonBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.ShinyPokemons[index][0].ToColor(),
        _pokegold.Colors.ShinyPokemons[index][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    Color1.SelectedColor = _pokegold.Colors.Pokemons[index][0].ToWPFColor();
    Color2.SelectedColor = _pokegold.Colors.Pokemons[index][1].ToWPFColor();

    ShinyColor1.SelectedColor = _pokegold.Colors.ShinyPokemons[index][0].ToWPFColor();
    ShinyColor2.SelectedColor = _pokegold.Colors.ShinyPokemons[index][1].ToWPFColor();
  }

  private void UpdateEvolutionMoves()
  {
    var index = PokemonListBox.SelectedIndex;

    EvolutionListView.Items.Clear();
    foreach (var e in _pokegold.Pokemons[index].Evolutions)
    {
      EvolutionListView.Items.Add(new EvolutionItem
      {
        Pokemon = _pokegold.Strings.PokemonNames[e.PokemonNo - 1],
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
          2 => $"\"{_pokegold.Strings.ItemNames[e.ItemNo - 1]}\" 사용",
          3 => e.ItemNo != 0xff ? $"\"{_pokegold.Strings.ItemNames[e.ItemNo - 1]}\" 지닌 상태" : "-",
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

    _pokegold.Pokemons[index].LearnMoves.Sort((a, b) => a.Level.CompareTo(b.Level));

    LearnMoveListView.Items.Clear();
    foreach (var e in _pokegold.Pokemons[index].LearnMoves)
    {
      LearnMoveListView.Items.Add(new LearnMoveItem
      {
        Level = $"{e.Level}",
        Move = $"{_pokegold.Strings.MoveNames[e.MoveNo - 1]}",
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

  private void NestedScrollImplEvent(object sender, MouseWheelEventArgs e)
  {
    if (sender is ListView listView && !e.Handled)
    {
      var border = (VisualTreeHelper.GetChild(listView, 0) as Border)!;
      var scrollViewer = (VisualTreeHelper.GetChild(border, 0) as ScrollViewer)!;
      if ((e.Delta > 0 && scrollViewer.VerticalOffset == 0) || (e.Delta < 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight))
      {
        e.Handled = true;

        var parent = (scrollViewer.Parent as UIElement)!;
        parent.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
          RoutedEvent = MouseWheelEvent,
          Source = scrollViewer,
        });
      }
    }

    if (sender is CheckListBox listBox && !e.Handled)
    {
      var border = (VisualTreeHelper.GetChild(listBox, 0) as Border)!;
      var scrollViewer = (VisualTreeHelper.GetChild(border, 0) as ScrollViewer)!;
      if ((e.Delta > 0 && scrollViewer.VerticalOffset == 0) || (e.Delta < 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight))
      {
        e.Handled = true;

        var parent = (scrollViewer.Parent as UIElement)!;
        parent.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
          RoutedEvent = MouseWheelEvent,
          Source = scrollViewer,
        });
      }
    }
  }

  private void OnPokemonListBoxSelected(object _, SelectionChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = PokemonListBox.SelectedIndex;
      if (index != -1)
      {
        NumberTextBox.Text = $"{index + 1}";
        NameTextBox.Text = _pokegold.Strings.PokemonNames[index];
        Type1ComboBox.SelectedIndex = _pokegold.Pokemons[index].Type1;
        Type2ComboBox.SelectedIndex = _pokegold.Pokemons[index].Type2;
        Item1ComboBox.SelectedIndex = _pokegold.Pokemons[index].Item1;
        Item2ComboBox.SelectedIndex = _pokegold.Pokemons[index].Item2;
        GenderRateComboBox.SelectedIndex = _pokegold.Pokemons[index].GetGenderRateType();
        GrowthRateComboBox.SelectedIndex = _pokegold.Pokemons[index].GrowthRate;
        EggGroup1ComboBox.SelectedIndex = (_pokegold.Pokemons[index].EggGroup & 0xf0) >> 4;
        EggGroup2ComboBox.SelectedIndex = (_pokegold.Pokemons[index].EggGroup & 0x0f) >> 0;
        EXPUpDown.Value = _pokegold.Pokemons[index].Exp;
        CatchRateUpDown.Value = _pokegold.Pokemons[index].CatchRate;

        HPUpDown.Value = _pokegold.Pokemons[index].HP;
        AttackUpDown.Value = _pokegold.Pokemons[index].Attack;
        DefenceUpDown.Value = _pokegold.Pokemons[index].Defence;
        SpAttackUpDown.Value = _pokegold.Pokemons[index].SpAttack;
        SpDefenceUpDown.Value = _pokegold.Pokemons[index].SpDefence;
        SpeedUpDown.Value = _pokegold.Pokemons[index].Speed;

        SpecificNameTextBox.Text = _pokegold.Pokedex[index].SpecificName;
        HeightUpDown.Value = _pokegold.Pokedex[index].Height / 10.0;
        WeightUpDown.Value = _pokegold.Pokedex[index].Weight / 10.0;
        DexDescriptionTextBox.Text = _pokegold.Pokedex[index].Description.Replace("[59]", "\n");

        foreach (var e in _tmhmList)
          e.SelectedItems.Clear();

        for (var i = 0; i < 57; i++)
        {
          var item = _tmhmList[i / 8].Items[i % 8];
          if (_pokegold.Pokemons[index].TMHMs[i])
            _tmhmList[i / 8].SelectedItems.Add(item);
        }

        UpdateEvolutionMoves();
        UpdatePokemonImages();
      }

      ContentBorder.IsEnabled = index != -1;
      ImagesGrid.Visibility = index != 200 ? Visibility.Visible : Visibility.Collapsed;
      UnownGrid.Visibility = index == 200 ? Visibility.Visible : Visibility.Collapsed;
    });
  }

  private void OnTextChanged(object _, TextChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = PokemonListBox.SelectedIndex;
      if (index != -1)
      {
        if (NameTextBox.Text.TryTextEncode(out var _))
        {
          _pokegold.Strings.PokemonNames[index] = NameTextBox.Text;
          PokemonListBox.Items[index] = _pokegold.Strings.PokemonNames[index];
          PokemonListBox.SelectedIndex = index;
          _pokegold.NotifyDataChanged();
        }

        if (SpecificNameTextBox.Text.TryTextEncode(out var _))
        {
          _pokegold.Pokedex[index].SpecificName = SpecificNameTextBox.Text;
          _pokegold.NotifyDataChanged();
        }

        var realDescription = DexDescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "[59]");
        if (realDescription.TryTextEncode(out var _))
        {
          _pokegold.Pokedex[index].Description = realDescription;
          _pokegold.NotifyDataChanged();
        }
      }
    });

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
    this.RunSafe(() =>
    {
      var index = PokemonListBox.SelectedIndex;
      if (index != -1)
      {
        _pokegold.Pokemons[index].Type1 = (byte)Type1ComboBox.SelectedIndex;
        _pokegold.Pokemons[index].Type2 = (byte)Type2ComboBox.SelectedIndex;
        _pokegold.Pokemons[index].Item1 = (byte)Item1ComboBox.SelectedIndex;
        _pokegold.Pokemons[index].Item2 = (byte)Item2ComboBox.SelectedIndex;
        _pokegold.Pokemons[index].SetGenderRateType(GenderRateComboBox.SelectedIndex);
        _pokegold.Pokemons[index].GrowthRate = (byte)GrowthRateComboBox.SelectedIndex;
        _pokegold.Pokemons[index].EggGroup = (byte)((EggGroup1ComboBox.SelectedIndex << 4) | EggGroup2ComboBox.SelectedIndex);
        _pokegold.NotifyDataChanged();
      }
    });
  }

  private void OnUpDownValueChanged(object _, RoutedPropertyChangedEventArgs<object> __)
  {
    var index = PokemonListBox.SelectedIndex;

    this.RunSafe(() =>
    {
      if (index != -1)
      {
        _pokegold.Pokemons[index].Exp = (byte)(EXPUpDown.Value ?? 0);
        _pokegold.Pokemons[index].CatchRate = (byte)(CatchRateUpDown.Value ?? 0);

        _pokegold.Pokemons[index].HP = (byte)(HPUpDown.Value ?? 0);
        _pokegold.Pokemons[index].Attack = (byte)(AttackUpDown.Value ?? 0);
        _pokegold.Pokemons[index].Defence = (byte)(DefenceUpDown.Value ?? 0);
        _pokegold.Pokemons[index].SpAttack = (byte)(SpAttackUpDown.Value ?? 0);
        _pokegold.Pokemons[index].SpDefence = (byte)(SpDefenceUpDown.Value ?? 0);
        _pokegold.Pokemons[index].Speed = (byte)(SpeedUpDown.Value ?? 0);

        _pokegold.Pokedex[index].Height = (byte)((HeightUpDown.Value ?? 0) * 10);
        _pokegold.Pokedex[index].Weight = (int)((WeightUpDown.Value ?? 0) * 10);

        _pokegold.NotifyDataChanged();
      }
    });

    // 포획률 퍼센티지 변경
    if (index != -1)
    {
      var percentage = string.Format("{0:P2}", (double)_pokegold.Pokemons[index].CatchRate / 0xff);
      CatchRatePercentageLabel.Content = percentage;
    }
  }

  private void OnImageClick(object sender, RoutedEventArgs _)
  {
    if (sender is Button button && button.Content is IgnoreDpiPanel panel && panel.Child is GBImageBox gbImageBox)
    {
      var menu = new ContextMenu();

      var headerMenu = new MenuItem { Header = "이미지 변경..." };
      headerMenu.Click += (_, __) =>
      {
        var dialog = new OpenFileDialog
        {
          Title = "이미지 변경",
          Filter = "png 파일|*png",
        };
        if (dialog.ShowDialog() ?? false)
        {
          if (GBImage.TryLoadFromFile(dialog.FileName, out var newImage))
          {
            if (newImage == null)
              return;

            var index = PokemonListBox.SelectedIndex;

            if (gbImageBox.Name == nameof(FrontImage) || gbImageBox.Name == nameof(FrontShinyImage))
            {
              if (newImage.Columns != newImage.Rows || newImage.Columns < 5 || newImage.Columns > 7 || newImage.Rows < 5 || newImage.Rows > 7)
              {
                System.Windows.MessageBox.Show("이미지 사이즈가 올바르지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
              }

              _pokegold.Images.Pokemons[index] = newImage.Source;
            }

            if (gbImageBox.Name == nameof(BackImage) || gbImageBox.Name == nameof(BackShinyImage))
            {
              if (newImage.Columns != 6 || newImage.Rows != 6)
              {
                System.Windows.MessageBox.Show("이미지 사이즈가 올바르지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
              }

              _pokegold.Images.PokemonBacksides[index] = newImage.Source;
            }

            _pokegold.Pokemons[index].SetImageTileSize(newImage.Rows);

            if (gbImageBox.Name == nameof(FrontShinyImage) || gbImageBox.Name == nameof(BackShinyImage))
            {
              _pokegold.Colors.ShinyPokemons[index][0] = GBColor.FromColor(newImage.Colors[1]);
              _pokegold.Colors.ShinyPokemons[index][1] = GBColor.FromColor(newImage.Colors[2]);
            }
            else
            {
              _pokegold.Colors.Pokemons[index][0] = GBColor.FromColor(newImage.Colors[1]);
              _pokegold.Colors.Pokemons[index][1] = GBColor.FromColor(newImage.Colors[2]);
            }

            UpdatePokemonImages();
            _pokegold.NotifyDataChanged();
          }
        }
      };
      menu.Items.Add(headerMenu);

      menu.Items.Add(new Separator());

      var pngMenu = new MenuItem { Header = "png 저장..." };
      pngMenu.Click += (_, __) =>
      {
        var index = PokemonListBox.SelectedIndex;
        var dialog = new SaveFileDialog
        {
          Title = "png 저장",
          Filter = "png 파일|*png",
          FileName = $"{button.Tag}_{index + 1}.png",
        };
        if (dialog.ShowDialog() ?? false)
          gbImageBox.GBImage!.WriteFile(dialog.FileName);
      };
      menu.Items.Add(pngMenu);

      var binMenu = new MenuItem { Header = "2bpp 저장..." };
      binMenu.Click += (_, __) =>
      {
        var index = PokemonListBox.SelectedIndex;
        var dialog = new SaveFileDialog
        {
          Title = "2bpp 저장",
          Filter = "2bpp 파일|*2bpp|bin 파일|*.bin|모든 파일|*.*",
          FileName = $"{button.Tag}_{index + 1}.2bpp",
        };
        if (dialog.ShowDialog() ?? false)
          File.WriteAllBytes(dialog.FileName, gbImageBox.GBImage!.Source!);
      };
      menu.Items.Add(binMenu);

      menu.IsOpen = true;
    }
  }

  private void OnColorPickerValueChanged(object _, RoutedPropertyChangedEventArgs<Color?> __)
  {
    this.RunSafe(() =>
    {
      var index = PokemonListBox.SelectedIndex;
      if (index != -1)
      {
        _pokegold.Colors.Pokemons[index][0] = GBColor.FromWPFColor(Color1.SelectedColor!.Value);
        _pokegold.Colors.Pokemons[index][1] = GBColor.FromWPFColor(Color2.SelectedColor!.Value);
        _pokegold.Colors.ShinyPokemons[index][0] = GBColor.FromWPFColor(ShinyColor1.SelectedColor!.Value);
        _pokegold.Colors.ShinyPokemons[index][1] = GBColor.FromWPFColor(ShinyColor2.SelectedColor!.Value);

        UpdatePokemonImages();
        _pokegold.NotifyDataChanged();
      }
    });
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
        var result = EvolutionDialog.Show(this);
        if (result != null)
        {
          var index = PokemonListBox.SelectedIndex;
          _pokegold.Pokemons[index].Evolutions.Add(result);
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
          if (System.Windows.MessageBox.Show("정말로 삭제하겠습니까?", "알림", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            return;

          _pokegold.Pokemons[PokemonListBox.SelectedIndex].Evolutions.RemoveAt(index);
          _pokegold.NotifyDataChanged();
          UpdateEvolutionMoves();
        }
        return;
      }

      if (button.Name == nameof(EvolutionClearButton))
      {
        if (System.Windows.MessageBox.Show("정말로 전부 삭제하겠습니까?", "알림", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
          return;

        _pokegold.Pokemons[PokemonListBox.SelectedIndex].Evolutions.Clear();
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
      var result = EvolutionDialog.Show(this, _pokegold.Pokemons[index].Evolutions[evIndex]);
      if (result != null)
      {
        _pokegold.Pokemons[index].Evolutions[evIndex] = result;
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
        var result = LearnMoveDialog.Show(this);
        if (result != null)
        {
          var index = PokemonListBox.SelectedIndex;
          _pokegold.Pokemons[index].LearnMoves.Add(result);
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
          if (System.Windows.MessageBox.Show("정말로 삭제하겠습니까?", "알림", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            return;

          _pokegold.Pokemons[PokemonListBox.SelectedIndex].LearnMoves.RemoveAt(index);
          _pokegold.NotifyDataChanged();
          UpdateEvolutionMoves();
        }
        return;
      }

      if (button.Name == nameof(LearnMoveClearButton))
      {
        if (System.Windows.MessageBox.Show("정말로 전부 삭제하겠습니까?", "알림", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
          return;

        _pokegold.Pokemons[PokemonListBox.SelectedIndex].LearnMoves.Clear();
        _pokegold.NotifyDataChanged();
        UpdateEvolutionMoves();
        return;
      }

      if (button.Name == nameof(LearnMoveImportButton))
      {
        var result = LearnMoveImportDialog.Show(this);
        foreach (var e in result)
        {
          foreach (var e2 in _pokegold.Pokemons[PokemonListBox.SelectedIndex].LearnMoves)
          {
            if (e2.Level == e.Level && e2.MoveNo == e.MoveNo)
              goto ignoreSameItem;
          }

          _pokegold.Pokemons[PokemonListBox.SelectedIndex].LearnMoves.Add(e);
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
      var result = LearnMoveDialog.Show(this, _pokegold.Pokemons[index].LearnMoves[lmIndex]);
      if (result != null)
      {
        _pokegold.Pokemons[index].LearnMoves[lmIndex] = result;
        _pokegold.NotifyDataChanged();
        UpdateEvolutionMoves();
      }
    }
  }

  private void OnTMHMsSelectionChanged(object _, ItemSelectionChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = PokemonListBox.SelectedIndex;
      if (index != -1)
      {
        for (var i = 0; i < 57; i++)
          _pokegold.Pokemons[index].TMHMs[i] = false;

        for (var i = 0; i < _tmhmList.Count; i++)
        {
          foreach (var e in _tmhmList[i].SelectedItems)
            _pokegold.Pokemons[index].TMHMs[(i * 8) + _tmhmList[i].SelectedItems.IndexOf(e)] = true;
        }

        _pokegold.NotifyDataChanged();
      }
    });
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

public sealed class EvolutionItem
{
  public string Pokemon { get; set; } = "";
  public string Method { get; set; } = "";
  public string Parameter1 { get; set; } = "";
  public string Parameter2 { get; set; } = "";
}

public sealed class LearnMoveItem
{
  public string Level { get; set; } = "";
  public string Move { get; set; } = "";
}
