using GSEditor.Contract.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GSEditor.UI.Windows;

public partial class MainWindow : Window
{
  private readonly IPokegoldService _pokegold = App.Services.GetRequiredService<IPokegoldService>();
  private readonly ISettingsService _appSettings = App.Services.GetRequiredService<ISettingsService>();
  private readonly IDialogService _dialogs = App.Services.GetRequiredService<IDialogService>();

  private readonly List<KeyGesture> _menuKeyGestures = new()
  {
    new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl+O"),
    new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S"),
    new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt+F4"),
    new KeyGesture(Key.F5, ModifierKeys.None, "F5"),
  };

  public MainWindow()
  {
    InitializeComponent();

    // 메뉴 숏컷 지정
    foreach (var keyGesture in _menuKeyGestures)
    {
      var menuItemShortcutCommand = new RoutedCommand();
      menuItemShortcutCommand.InputGestures.Add(keyGesture);
      CommandBindings.Add(new CommandBinding(menuItemShortcutCommand, (_, __) => OnMenuItemShortcutExecute(keyGesture.DisplayString, null)));
    }

    _pokegold.RomChanged += OnRomChanged;
    _pokegold.DataChanged += OnDataChanged;
  }

  protected override void OnClosing(CancelEventArgs e)
  {
    base.OnClosing(e);

    if (_pokegold.IsChanged)
    {
      switch (_dialogs.ShowYesNoCancel("알림", "변경 내용을 저장하시겠습니까?"))
      {
        case YesNoCancelResult.Yes:
          e.Cancel = false;
          _pokegold.Write();
          break;

        case YesNoCancelResult.No:
          e.Cancel = false;
          break;

        default:
        case YesNoCancelResult.Cancel:
          e.Cancel = true;
          break;
      }
    }
  }

  protected override void OnClosed(EventArgs e)
  {
    base.OnClosed(e);

    _pokegold.RomChanged -= OnRomChanged;
    _pokegold.DataChanged -= OnDataChanged;
  }

  private void OnDataChanged(object? _, EventArgs __)
  {
    FileModifyState.Visibility = _pokegold.IsChanged ? Visibility.Visible : Visibility.Collapsed;
  }

  private void OnRomChanged(object? _, EventArgs __)
  {
    FileNameState.Content = _pokegold.FileName;
    SaveMenuItem.IsEnabled = _pokegold.IsOpened;
    TestPlayMenuItem.IsEnabled = _pokegold.IsOpened;
    SaveToolBarButton.IsEnabled = _pokegold.IsOpened;
    TestPlayToolBarButton.IsEnabled = _pokegold.IsOpened;
    MainTab.IsEnabled = _pokegold.IsOpened;
  }

  private void OnMenuItemClick(object? sender, RoutedEventArgs _)
  {
    if (sender is MenuItem menuItem)
      OnMenuItemShortcutExecute(menuItem.InputGestureText, menuItem);
    else if (sender is Button button)
      OnMenuItemShortcutExecute((button.Tag as string)!, null);
  }

  private void OnMenuItemShortcutExecute(string shortcut, MenuItem? menuItem)
  {
    switch (shortcut)
    {
      case "Ctrl+O":
        if (_pokegold.IsOpened && _pokegold.IsChanged)
        {
          if (!_dialogs.ShowQuestion("알림", "이미 작업중인 파일이 열려있습니다.\n다른 파일로 열겠습니까?"))
            return;
        }

        var fileName = _dialogs.ShowOpenFile("열기", "지원하는 파일|*.gb;*.gbc;*.bin|모든 파일|*.*");
        if (fileName != null)
        {
          if (!_pokegold.Open(fileName))
            _dialogs.ShowError("알림", "오류가 발생하여 롬 파일을 불러올 수 없습니다.");
        }
        break;

      case "Ctrl+S":
        if (_pokegold.IsOpened)
          _pokegold.Write();
        break;

      case "Alt+F4":
        Close();
        break;

      case "F5":
        if (_pokegold.IsOpened)
        {
          var emulatorPath = _appSettings.AppSettings.EmulatorPath;
          if (emulatorPath == null || !File.Exists(emulatorPath))
          {
            _dialogs.ShowError("알림", "에뮬레이터가 설정되어있지 않아 실패했습니다.");
            return;
          }

          try
          {
            _pokegold.Run(emulatorPath!);
          }
          catch
          {
            _dialogs.ShowError("알림", "지원하지 않는 에뮬레이터로 실행하였습니다.\n다른 에뮬레이터로 실행해주세요.");
          }
        }
        break;

      default:
        if (menuItem != null)
        {
          switch (menuItem.Name)
          {
            case nameof(EmulatorSettingsMenuItem):
              var emulatorDialog = new OpenFileDialog
              {
                Title = "열기",
                Filter = "실행 가능한 파일|*.exe|모든 파일|*.*",
              };
              if (emulatorDialog.ShowDialog() ?? false)
                _appSettings.AppSettings.EmulatorPath = emulatorDialog.FileName;
              _appSettings.Apply();
              break;

            case nameof(AppInformationMenuItem):
              _dialogs.ShowAppInfo();
              break;
          }
        }
        break;
    }
  }
}
