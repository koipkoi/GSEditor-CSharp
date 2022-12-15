using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using GSEditor.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GSEditor.UI.Tabs;

public partial class UnownTab : UserControl
{
  private readonly IPokegoldService _pokegold = App.Services.GetRequiredService<IPokegoldService>();
  private readonly IDialogService _dialogs = App.Services.GetRequiredService<IDialogService>();
  private readonly IPopupMenuService _popupMenus = App.Services.GetRequiredService<IPopupMenuService>();

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
    var index = UnownListBox.SelectedIndex;
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
            if (newImage.Columns != _pokegold.Data.Pokemons[200].ImageTileSize || newImage.Rows != _pokegold.Data.Pokemons[200].ImageTileSize)
            {
              _dialogs.ShowError("오류", "이미지 사이즈가 올바르지 않습니다.\n40x40 사이즈로 맞춰주세요.");
              return;
            }

            _pokegold.Data.Images.Unowns[index] = newImage.Source;
            _pokegold.Data.Pokemons[index].ImageTileSize = (byte)newImage.Rows;
          }

          if (gbImageBox.Name == nameof(BackImage) || gbImageBox.Name == nameof(BackShinyImage))
          {
            if (newImage.Columns != 6 || newImage.Rows != 6)
            {
              _dialogs.ShowError("오류", "이미지 사이즈가 올바르지 않습니다.\n48x48 사이즈로 맞춰주세요.");
              return;
            }

            _pokegold.Data.Images.UnownBacksides[index] = newImage.Source;
          }

          if (gbImageBox.Name == nameof(FrontShinyImage) || gbImageBox.Name == nameof(BackShinyImage))
          {
            _pokegold.Data.Colors.ShinyPokemons[200][0] = newImage.Colors[1];
            _pokegold.Data.Colors.ShinyPokemons[200][1] = newImage.Colors[2];
          }
          else
          {
            _pokegold.Data.Colors.Pokemons[200][0] = newImage.Colors[1];
            _pokegold.Data.Colors.Pokemons[200][1] = newImage.Colors[2];
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
          var fileName = _dialogs.ShowSaveFile("png 저장", "png 파일|*png", $"unown_{button.Tag}_{index + 1}.png");
          if (fileName != null)
            gbImageBox.GBImage!.WriteFile(fileName);
        })
        .Add("2bpp 저장...", () =>
        {
          var fileName = _dialogs.ShowSaveFile("2bpp 저장", "2bpp 파일|*2bpp|bin 파일|*.bin|모든 파일|*.*", $"unown_{button.Tag}_{index + 1}.2bpp");
          if (fileName != null)
            File.WriteAllBytes(fileName, gbImageBox.GBImage!.Source!);
        })
        .Create());
    }
  }

  private void OnColorPickerValueChanged(object _, RoutedPropertyChangedEventArgs<Color?> __)
  {
    this.RunSafe(() =>
    {
      var index = UnownListBox.SelectedIndex;
      if (index != -1)
      {
        _pokegold.Data.Colors.Pokemons[200][0] = new GBColor(Color1.SelectedColor!.Value);
        _pokegold.Data.Colors.Pokemons[200][1] = new GBColor(Color2.SelectedColor!.Value);
        _pokegold.Data.Colors.ShinyPokemons[200][0] = new GBColor(ShinyColor1.SelectedColor!.Value);
        _pokegold.Data.Colors.ShinyPokemons[200][1] = new GBColor(ShinyColor2.SelectedColor!.Value);
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
      Source = _pokegold.Data.Images.Unowns[index],
      Rows = _pokegold.Data.Pokemons[200].ImageTileSize,
      Columns = _pokegold.Data.Pokemons[200].ImageTileSize,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.Pokemons[200][0],
        _pokegold.Data.Colors.Pokemons[200][1],
        GBColor.GBBlack,
      },
    };

    FrontShinyImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.Unowns[index],
      Rows = _pokegold.Data.Pokemons[200].ImageTileSize,
      Columns = _pokegold.Data.Pokemons[200].ImageTileSize,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.ShinyPokemons[200][0],
        _pokegold.Data.Colors.ShinyPokemons[200][1],
        GBColor.GBBlack,
      },
    };

    BackImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.UnownBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.Pokemons[200][0],
        _pokegold.Data.Colors.Pokemons[200][1],
        GBColor.GBBlack,
      },
    };

    BackShinyImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.UnownBacksides[index],
      Rows = 6,
      Columns = 6,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.ShinyPokemons[200][0],
        _pokegold.Data.Colors.ShinyPokemons[200][1],
        GBColor.GBBlack,
      },
    };

    Color1.SelectedColor = _pokegold.Data.Colors.Pokemons[200][0].ToColor();
    Color2.SelectedColor = _pokegold.Data.Colors.Pokemons[200][1].ToColor();
    ShinyColor1.SelectedColor = _pokegold.Data.Colors.ShinyPokemons[200][0].ToColor();
    ShinyColor2.SelectedColor = _pokegold.Data.Colors.ShinyPokemons[200][1].ToColor();
  }
}
