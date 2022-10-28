using GSEditor.Core;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GSEditor.UI.Windows;

public partial class MainWindow : Window, INotifyPropertyChanged
{
  private readonly List<KeyGesture> _menuKeyGestures = new() {
    new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl+O"),
    new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S"),
    new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt+F4"),

    new KeyGesture(Key.F5, ModifierKeys.None, "F5"),
  };

  private readonly Pokegold _pokegold = Injector.Get<Pokegold>();

  public event PropertyChangedEventHandler? PropertyChanged;

  public bool RomFileOpened { get; set; } = false;
  public string RomFilename { get; set; } = "-";

  public MainWindow()
  {
    InitializeComponent();

    DataContext = this;

    // 메뉴 숏컷 지정
    foreach (var keyGesture in _menuKeyGestures)
    {
      var menuItemShortcutCommand = new RoutedCommand();
      menuItemShortcutCommand.InputGestures.Add(keyGesture);
      CommandBindings.Add(new CommandBinding(menuItemShortcutCommand, (_, __) => OnMenuItemShortcutExecute(keyGesture.DisplayString, null)));
    }

    // 불필요 툴바 오버플로 영역 제거
    MainToolBar.Loaded += (_, __) =>
    {
      if (MainToolBar.Template.FindName("OverflowGrid", MainToolBar) is FrameworkElement overflowGrid)
        overflowGrid.Visibility = Visibility.Collapsed;
    };

    _pokegold.RomChanged += OnRomChanged;
  }

  protected override void OnClosing(CancelEventArgs e)
  {
    base.OnClosing(e);

    // todo 종료 확인 추가
  }

  protected override void OnClosed(EventArgs e)
  {
    base.OnClosed(e);

    _pokegold.RomChanged -= OnRomChanged;
  }

  private void OnRomChanged(object? _, EventArgs __)
  {
    RomFileOpened = _pokegold.IsOpened;
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RomFileOpened)));

    RomFilename = _pokegold.Filename;
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RomFilename)));
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
          // todo 테스트 플레이 추가
        }
        break;

      default:
        if (menuItem != null)
        {
          switch (menuItem.Name)
          {
            case nameof(EmulatorSettingsMenuItem):
              // todo 에뮬레이터 설정 추가
              break;

            case nameof(AppInformationMenuItem):
              // todo 앱 정보 추가
              break;
          }
        }
        break;
    }
  }
}
