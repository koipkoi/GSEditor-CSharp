using GSEditor.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GSEditor.UI.Windows;

public partial class MainWindow : Window
{
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();
  private readonly AppSettings _appSettings = App.Services.GetRequiredService<AppSettings>();
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
      switch (MessageBox.Show("변경 내용을 저장하시겠습니까?", "알림", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
      {
        case MessageBoxResult.Yes:
          e.Cancel = false;
          _pokegold.Write(_pokegold.Filename);
          break;

        case MessageBoxResult.No:
          e.Cancel = false;
          break;

        default:
        case MessageBoxResult.Cancel:
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
    FileNameState.Content = _pokegold.Filename;

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
        var openFileDialog = new OpenFileDialog
        {
          Title = "열기",
          Filter = "지원하는 파일|*.gb;*.gbc;*.bin|모든 파일|*.*",
        };
        if (openFileDialog.ShowDialog() ?? false)
          _pokegold.Read(openFileDialog.FileName);
        break;

      case "Ctrl+S":
        if (_pokegold.IsOpened)
          _pokegold.Write(_pokegold.Filename);
        break;

      case "Alt+F4":
        Close();
        break;

      case "F5":
        if (_pokegold.IsOpened)
        {
          if (!_pokegold.StartTestPlay(_appSettings.EmulatorPath))
            MessageBox.Show("에뮬레이터가 설정되어있지 않아 실패했습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _appSettings.EmulatorPath = emulatorDialog.FileName;
              break;

            case nameof(AppInformationMenuItem):
              AppInformationDialog.Show(this);
              break;
          }
        }
        break;
    }
  }
}
