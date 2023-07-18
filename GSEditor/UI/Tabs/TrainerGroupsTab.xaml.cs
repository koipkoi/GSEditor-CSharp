using GSEditor.Common.Extensions;
using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using GSEditor.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GSEditor.UI.Tabs;

public partial class TrainerGroupsTab : UserControl
{
  private readonly IPokegoldService _pokegold = Program.Services.GetRequiredService<IPokegoldService>();
  private readonly IDialogService _dialogs = Program.Services.GetRequiredService<IDialogService>();
  private readonly IPopupMenuService _popupMenus = Program.Services.GetRequiredService<IPopupMenuService>();

  public TrainerGroupsTab()
  {
    InitializeComponent();

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, _) => OnNeedTabUpdate());
    _pokegold.RegisterDataChanged(this, (_, _) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    var previousSelection = TrainersListBox.SelectedIndex;

    this.RunSafe(() =>
    {
      TrainersListBox.Items.Clear();
      for (var i = 0; i < 67; i++)
      {
        var number = (i + 1).ToString().PadLeft(2, '0');
        var className = _pokegold.Data.Strings.TrainerClassNames.ElementAtOrDefault(i);
        TrainersListBox.Items.Add($"{number} [{className}]");
      }
    });

    TrainersListBox.SelectedIndex = previousSelection;
  }

  private void OnListBoxSelected(object _, SelectionChangedEventArgs __)
  {
    this.RunSafe(() =>
    {
      var index = TrainersListBox.SelectedIndex;
      if (index != -1)
      {
        var previousSelection = TrainersListBox.SelectedIndex;
        NameTextBox.Text = _pokegold.Data.Strings.TrainerClassNames[index];

        UpdatePokemonImages();
      }

      ContentBorder.IsEnabled = index != -1;
      ImagePanel.Visibility = index != 66 ? Visibility.Visible : Visibility.Collapsed;
      NotEditableImageLabel.Visibility = index == 66 ? Visibility.Visible : Visibility.Collapsed;
    });
  }

  private void OnTextBoxTextChanged(object _, TextChangedEventArgs __)
  {
    var index = TrainersListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        if (NameTextBox.Text.TryTextEncode(out var _))
        {
          var previousSelection = TrainersListBox.SelectedIndex;
          _pokegold.Data.Strings.TrainerClassNames[index] = NameTextBox.Text;
          var number = (index + 1).ToString().PadLeft(2, '0');
          var className = _pokegold.Data.Strings.TrainerClassNames[index];
          TrainersListBox.Items[index] = $"{number} [{className}]";
          TrainersListBox.SelectedIndex = previousSelection;
          _pokegold.NotifyDataChanged();
        }
      });
    }
  }

  private void OnImageClick(object sender, RoutedEventArgs _)
  {
    var index = TrainersListBox.SelectedIndex;
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

          if (newImage.Columns != 7 || newImage.Rows != 7)
          {
            _dialogs.ShowError("오류", "이미지 사이즈가 올바르지 않습니다.\n56x56 사이즈로 맞춰주세요.");
            return;
          }

          _pokegold.Data.Images.Trainers[index] = newImage.Source;
          _pokegold.Data.Colors.Trainers[index][0] = newImage.Colors[1];
          _pokegold.Data.Colors.Trainers[index][1] = newImage.Colors[2];

          this.RunSafe(() =>
          {
            UpdatePokemonImages();
            _pokegold.NotifyDataChanged();
          });
        })
        .Add("-")
        .Add("png 저장...", () =>
        {
          var fileName = _dialogs.ShowSaveFile("png 저장", "png 파일|*png", $"trainer_{index + 1}.png");
          if (fileName != null)
            gbImageBox.GBImage!.WriteFile(fileName);
        })
        .Add("2bpp 저장...", () =>
        {
          var fileName = _dialogs.ShowSaveFile("2bpp 저장", "2bpp 파일|*2bpp|bin 파일|*.bin|모든 파일|*.*", $"trainer_{index + 1}.2bpp");
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
      var index = TrainersListBox.SelectedIndex;
      if (index != -1 || index != 66)
      {
        _pokegold.Data.Colors.Trainers[index][0] = new GBColor(Color1.SelectedColor!.Value);
        _pokegold.Data.Colors.Trainers[index][1] = new GBColor(Color2.SelectedColor!.Value);
        UpdatePokemonImages();
        _pokegold.NotifyDataChanged();
      }
    });
  }

  private void UpdatePokemonImages()
  {
    var index = TrainersListBox.SelectedIndex;
    if (index == -1 || index >= 66)
      return;

    FrontImage.GBImage = new()
    {
      Source = _pokegold.Data.Images.Trainers[index],
      Rows = 7,
      Columns = 7,
      Colors = new GBColor[] {
        GBColor.GBWhite,
        _pokegold.Data.Colors.Trainers[index][0],
        _pokegold.Data.Colors.Trainers[index][1],
        GBColor.GBBlack,
      },
    };

    Color1.SelectedColor = _pokegold.Data.Colors.Trainers[index][0].ToColor();
    Color2.SelectedColor = _pokegold.Data.Colors.Trainers[index][1].ToColor();
  }
}
