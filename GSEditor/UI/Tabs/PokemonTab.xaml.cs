using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using GSEditor.UI.Controls;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GSEditor.UI.Tabs;

public partial class PokemonTab : UserControl, INotifyPropertyChanged
{
  private readonly System.Drawing.Color _pokemonWhiteColor = GBColor.FromBytes(new byte[] { 0xff, 0x7f, }).ToColor();
  private readonly System.Drawing.Color _pokemonBlackColor = GBColor.FromBytes(new byte[] { 0x00, 0x00, }).ToColor();

  private readonly Pokegold _pokegold = Injector.Get<Pokegold>();
  private bool _selfChanged = false;

  public event PropertyChangedEventHandler? PropertyChanged;

  public bool IsPokemonListSelected { get; set; } = false;
  public bool IsUnown { get; set; } = false;
  public BindingList<EvolutionItem> Evolutions { get; set; } = new();
  public BindingList<LearnMoveItem> Moves { get; set; } = new();

  public PokemonTab()
  {
    InitializeComponent();

    DataContext = this;

    // 롬파일 로딩 갱신
    _pokegold.RegisterRomChanged(this, (_, _) =>
    {
      _selfChanged = true;

      var previousSelection = PokemonListBox.SelectedIndex;

      PokemonListBox.Items.Clear();
      for (var i = 0; i < 251; i++)
      {
        var e = _pokegold.Strings.PokemonNames[i];
        PokemonListBox.Items.Add(e);
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

      _selfChanged = false;

      PokemonListBox.SelectedIndex = previousSelection;
    });

    // 포켓몬 이름 변경 갱신
    _pokegold.RegisterPokemonChanged(this, (_, index) =>
    {
      _selfChanged = true;

      PokemonListBox.Items[index] = _pokegold.Strings.PokemonNames[index];
      PokemonListBox.SelectedIndex = index;

      _selfChanged = false;
    });

    // 타입명 변경 갱신
    _pokegold.RegisterTypeChanged(this, (_, index) =>
    {
      Type1ComboBox.Items[index] = _pokegold.Strings.TypeNames[index];
      Type2ComboBox.Items[index] = _pokegold.Strings.TypeNames[index];
    });

    // 아이템 변경 갱신
    _pokegold.RegisterItemChanged(this, (_, index) =>
    {
      Item1ComboBox.Items[index + 1] = _pokegold.Strings.ItemNames[index];
      Item2ComboBox.Items[index + 1] = _pokegold.Strings.ItemNames[index];
    });
  }

  private void OnSizeChanged(object _, SizeChangedEventArgs __)
  {
    if (EvolutionListView.View is GridView evolutionGridView)
    {
      var totallyWidth = EvolutionListView.ActualWidth - 72;
      evolutionGridView.Columns[0].Width = totallyWidth * 0.2;
      evolutionGridView.Columns[1].Width = totallyWidth * 0.2;
      evolutionGridView.Columns[2].Width = totallyWidth * 0.35;
      evolutionGridView.Columns[3].Width = totallyWidth * 0.35;
    }

    if (LearnMoveListView.View is GridView learnMoveGridView)
    {
      var totallyWidth = LearnMoveListView.ActualWidth - 72;
      learnMoveGridView.Columns[0].Width = totallyWidth * 0.3;
      learnMoveGridView.Columns[1].Width = totallyWidth * 0.7;
    }
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
  }

  private void OnPokemonListBoxSelected(object _, SelectionChangedEventArgs __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (!_selfChanged)
    {
      _selfChanged = true;

      NumberTextBox.Text = $"{index + 1}";
      NameTextBox.Text = _pokegold.Strings.PokemonNames[index];
      Type1ComboBox.SelectedIndex = _pokegold.Pokemons[index].Type1;
      Type2ComboBox.SelectedIndex = _pokegold.Pokemons[index].Type2;
      Item1ComboBox.SelectedIndex = _pokegold.Pokemons[index].Item1;
      Item2ComboBox.SelectedIndex = _pokegold.Pokemons[index].Item2;
      GenderRateComboBox.SelectedIndex = _pokegold.Pokemons[index].GetGenderRateType();
      GrowthRateComboBox.SelectedIndex = _pokegold.Pokemons[index].GetGrowthRateType();
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

      Evolutions.Clear();
      foreach (var e in _pokegold.Pokemons[index].Evolutions)
      {
        Evolutions.Add(new()
        {
          Pokemon = _pokegold.Strings.PokemonNames[e.PokemonNo - 1],
          Method = e.Type switch
          {
            1 => "레벨업",
            2 => "도구",
            3 => "통신교환",
            4 => "친밀도",
            5 => "능력치",
            _ => "?",
          },
          Parameter1 = e.Type switch
          {
            1 => $"레벨 {e.Level} 달성",
            2 => $"\"{_pokegold.Strings.ItemNames[e.ItemNo - 1]}\" 사용",
            3 => e.ItemNo != 0xff ? $"\"{_pokegold.Strings.ItemNames[e.ItemNo - 1]}\" 지닌 상태" : "-",
            4 => $"친밀도 MAX",
            5 => $"레벨 {e.Level - 1} 달성",
            _ => "-",
          },
          Parameter2 = e.Type switch
          {
            4 => e.Affection switch
            {
              1 => "레벨업",
              2 => "낮에 레벨업",
              3 => "밤에 레벨업",
              _ => "?",
            },
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

      Moves.Clear();
      foreach (var e in _pokegold.Pokemons[index].LearnMoves)
      {
        Moves.Add(new()
        {
          Level = $"{e.Level}",
          Move = $"{_pokegold.Strings.MoveNames[e.MoveNo]}",
        });
      }

      IsPokemonListSelected = true;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPokemonListSelected)));

      IsUnown = index == 200;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUnown)));

      UpdatePokemonImages();
      _selfChanged = false;
    }
  }

  private void UpdatePokemonImages()
  {
    _selfChanged = true;

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

    _selfChanged = false;
  }

  private void OnTextChanged(object _, TextChangedEventArgs __)
  {
    var changed = false;
    var index = PokemonListBox.SelectedIndex;

    if (!_selfChanged && NameTextBox.Text.TryTextEncode(out var _))
    {
      _pokegold.Strings.PokemonNames[index] = NameTextBox.Text;
      _pokegold.NotifyPokemonChanged(index);
      changed = true;
    }

    if (!_selfChanged && SpecificNameTextBox.Text.TryTextEncode(out var _))
    {
      _pokegold.Pokedex[index].SpecificName = SpecificNameTextBox.Text;
      changed = true;
    }

    var maxLength = 0;
    foreach (var line in DexDescriptionTextBox.Text.Split("\n"))
    {
      if (line.Length > maxLength)
        maxLength = line.Length;
    }
    DexDescriptionLabel.Content = $"설명 ({maxLength}/18)：";

    if (!_selfChanged && DexDescriptionTextBox.Text.TryTextEncode(out var _))
    {
      _pokegold.Pokedex[index].Description = DexDescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "[59]");
      changed = true;
    }

    if (changed)
      _pokegold.NotifyDataChanged();
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (!_selfChanged)
    {
      _pokegold.Pokemons[index].Type1 = (byte)Type1ComboBox.SelectedIndex;
      _pokegold.Pokemons[index].Type2 = (byte)Type2ComboBox.SelectedIndex;
      _pokegold.Pokemons[index].Item1 = (byte)Item1ComboBox.SelectedIndex;
      _pokegold.Pokemons[index].Item2 = (byte)Item2ComboBox.SelectedIndex;
      _pokegold.Pokemons[index].SetGenderRateType(GenderRateComboBox.SelectedIndex);
      _pokegold.Pokemons[index].SetGrowthRateType(GrowthRateComboBox.SelectedIndex);
      _pokegold.Pokemons[index].EggGroup = (byte)((EggGroup1ComboBox.SelectedIndex << 4) | EggGroup2ComboBox.SelectedIndex);
      _pokegold.NotifyDataChanged();
    }
  }

  private void OnUpDownValueChanged(object _, RoutedPropertyChangedEventArgs<object> __)
  {
    var index = PokemonListBox.SelectedIndex;
    if (!_selfChanged)
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
  }

  private void OnImageClick(object sender, RoutedEventArgs _)
  {
    if (sender is Button button && button.Content is GBImageBox gbImageBox)
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
                MessageBox.Show("이미지 사이즈가 올바르지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
              }

              _pokegold.Images.Pokemons[index] = newImage.Source;
            }

            if (gbImageBox.Name == nameof(BackImage) || gbImageBox.Name == nameof(BackShinyImage))
            {
              if (newImage.Columns != 6 || newImage.Rows != 6)
              {
                MessageBox.Show("이미지 사이즈가 올바르지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
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
          FileName = $"{index + 1}.png",
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
          FileName = $"{index + 1}.2bpp",
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
    if (!_selfChanged)
    {
      var index = PokemonListBox.SelectedIndex;
      _pokegold.Colors.Pokemons[index][0] = GBColor.FromWPFColor(Color1.SelectedColor!.Value);
      _pokegold.Colors.Pokemons[index][1] = GBColor.FromWPFColor(Color2.SelectedColor!.Value);
      _pokegold.Colors.ShinyPokemons[index][0] = GBColor.FromWPFColor(ShinyColor1.SelectedColor!.Value);
      _pokegold.Colors.ShinyPokemons[index][1] = GBColor.FromWPFColor(ShinyColor2.SelectedColor!.Value);

      UpdatePokemonImages();
      _pokegold.NotifyDataChanged();
    }
  }

  private void OnEvolutionListViewDoubleClick(object _, MouseButtonEventArgs e)
  {
    if (e.OriginalSource is FrameworkElement element && element.DataContext is EvolutionItem item)
    {
      // todo 진화 항목 편집 추가
    }
  }

  private void OnEvolutionButtonClick(object sender, RoutedEventArgs __)
  {
    if (sender is Button button)
    {
      if (button.Name == nameof(EvolutionAddButton))
      {
        // todo '항목 추가' 추가
      }

      if (button.Name == nameof(EvolutionRemoveButton))
      {
        // todo '삭제' 추가
      }
    }
  }

  private void OnLearnMoveListViewDoubleClick(object _, MouseButtonEventArgs e)
  {
    if (e.OriginalSource is FrameworkElement element && element.DataContext is LearnMoveItem item)
    {
      // todo 진화 항목 편집 추가
    }
  }

  private void OnLearnMoveButtonClick(object sender, RoutedEventArgs __)
  {
    if (sender is Button button)
    {
      if (button.Name == nameof(LearnMoveAddButton))
      {
        // todo '항목 추가' 추가
      }

      if (button.Name == nameof(LearnMoveRemoveButton))
      {
        // todo '삭제' 추가
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
