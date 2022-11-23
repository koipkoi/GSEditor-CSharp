using GSEditor.Core;
using GSEditor.Core.PokegoldCore;
using GSEditor.UI.Controls;
using GSEditor.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GSEditor.UI.Tabs;

public partial class UnownTab : UserControl
{
  private readonly System.Drawing.Color _pokemonWhiteColor = GBColor.FromBytes(new byte[] { 0xff, 0x7f, }).ToColor();
  private readonly System.Drawing.Color _pokemonBlackColor = GBColor.FromBytes(new byte[] { 0x00, 0x00, }).ToColor();
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();

  public UnownTab()
  {
    InitializeComponent();

    for (var i = 0; i < 26; i++)
      UnownListBox.Items.Add(((char)('A' + i)).ToString());
  }

  private void OnUnownListBoxSelected(object _, SelectionChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = UnownListBox.SelectedIndex;
      if (index != -1)
        UpdatePokemonImages();
      ContentBorder.IsEnabled = index != -1;
    });
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

            var index = UnownListBox.SelectedIndex;

            if (gbImageBox.Name == nameof(FrontImage) || gbImageBox.Name == nameof(FrontShinyImage))
            {
              if (newImage.Columns != _pokegold.Pokemons[200].GetImageTileSize() || newImage.Rows != _pokegold.Pokemons[200].GetImageTileSize())
              {
                MessageBox.Show("이미지 사이즈가 올바르지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
              }

              _pokegold.Images.Unowns[index] = newImage.Source;
            }

            if (gbImageBox.Name == nameof(BackImage) || gbImageBox.Name == nameof(BackShinyImage))
            {
              if (newImage.Columns != 6 || newImage.Rows != 6)
              {
                MessageBox.Show("이미지 사이즈가 올바르지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
              }

              _pokegold.Images.UnownBacksides[index] = newImage.Source;
            }

            _pokegold.Pokemons[index].SetImageTileSize(newImage.Rows);

            if (gbImageBox.Name == nameof(FrontShinyImage) || gbImageBox.Name == nameof(BackShinyImage))
            {
              _pokegold.Colors.ShinyPokemons[200][0] = GBColor.FromColor(newImage.Colors[1]);
              _pokegold.Colors.ShinyPokemons[200][1] = GBColor.FromColor(newImage.Colors[2]);
            }
            else
            {
              _pokegold.Colors.Pokemons[200][0] = GBColor.FromColor(newImage.Colors[1]);
              _pokegold.Colors.Pokemons[200][1] = GBColor.FromColor(newImage.Colors[2]);
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
        var index = UnownListBox.SelectedIndex;
        var dialog = new SaveFileDialog
        {
          Title = "png 저장",
          Filter = "png 파일|*png",
          FileName = $"unown_{button.Tag}_{index + 1}.png",
        };
        if (dialog.ShowDialog() ?? false)
          gbImageBox.GBImage!.WriteFile(dialog.FileName);
      };
      menu.Items.Add(pngMenu);

      var binMenu = new MenuItem { Header = "2bpp 저장..." };
      binMenu.Click += (_, __) =>
      {
        var index = UnownListBox.SelectedIndex;
        var dialog = new SaveFileDialog
        {
          Title = "2bpp 저장",
          Filter = "2bpp 파일|*2bpp|bin 파일|*.bin|모든 파일|*.*",
          FileName = $"unown_{button.Tag}_{index + 1}.2bpp",
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
      var index = UnownListBox.SelectedIndex;
      if (index != -1)
      {
        _pokegold.Colors.Pokemons[200][0] = GBColor.FromWPFColor(Color1.SelectedColor!.Value);
        _pokegold.Colors.Pokemons[200][1] = GBColor.FromWPFColor(Color2.SelectedColor!.Value);
        _pokegold.Colors.ShinyPokemons[200][0] = GBColor.FromWPFColor(ShinyColor1.SelectedColor!.Value);
        _pokegold.Colors.ShinyPokemons[200][1] = GBColor.FromWPFColor(ShinyColor2.SelectedColor!.Value);

        UpdatePokemonImages();
        _pokegold.NotifyDataChanged();
      }
    });
  }

  private void UpdatePokemonImages()
  {
    var index = UnownListBox.SelectedIndex;

    FrontImage.GBImage = new()
    {
      Source = _pokegold.Images.Unowns[index],
      Rows = _pokegold.Pokemons[200].GetImageTileSize(),
      Columns = _pokegold.Pokemons[200].GetImageTileSize(),
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.Pokemons[200][0].ToColor(),
        _pokegold.Colors.Pokemons[200][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    FrontShinyImage.GBImage = new()
    {
      Source = _pokegold.Images.Unowns[index],
      Rows = _pokegold.Pokemons[200].GetImageTileSize(),
      Columns = _pokegold.Pokemons[200].GetImageTileSize(),
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.ShinyPokemons[200][0].ToColor(),
        _pokegold.Colors.ShinyPokemons[200][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    BackImage.GBImage = new()
    {
      Source = _pokegold.Images.UnownBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.Pokemons[200][0].ToColor(),
        _pokegold.Colors.Pokemons[200][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    BackShinyImage.GBImage = new()
    {
      Source = _pokegold.Images.UnownBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new System.Drawing.Color[] {
        _pokemonWhiteColor,
        _pokegold.Colors.ShinyPokemons[200][0].ToColor(),
        _pokegold.Colors.ShinyPokemons[200][1].ToColor(),
        _pokemonBlackColor,
      },
    };

    Color1.SelectedColor = _pokegold.Colors.Pokemons[200][0].ToWPFColor();
    Color2.SelectedColor = _pokegold.Colors.Pokemons[200][1].ToWPFColor();

    ShinyColor1.SelectedColor = _pokegold.Colors.ShinyPokemons[200][0].ToWPFColor();
    ShinyColor2.SelectedColor = _pokegold.Colors.ShinyPokemons[200][1].ToWPFColor();
  }
}
