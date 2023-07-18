using GSEditor.Common.Bindings;
using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace GSEditor.ViewModels.Windows;

public sealed class MainWindowViewModel
{
  private readonly IPokegoldService _pokegold = Program.Services.GetRequiredService<IPokegoldService>();
  private readonly ISettingsService _appSettings = Program.Services.GetRequiredService<ISettingsService>();
  private readonly IDialogService _dialogs = Program.Services.GetRequiredService<IDialogService>();
  private readonly IUpdateService _updates = Program.Services.GetRequiredService<IUpdateService>();

  public BindingProperty<string> Title { get; } = new("");
  public BindingProperty<bool> IsRomOpened { get; } = new(false);
  public BindingProperty<bool> IsRomChanged { get; } = new(false);
  public BindingProperty<string> RomFileName { get; } = new("-");

  public MainWindowViewModel()
  {
    Title.Value = $"GS 에디터 - v{Platforms.AppVersion}";

    _pokegold.RomChanged += OnDataChanged;
    _pokegold.DataChanged += OnDataChanged;
  }

  public void RequestClose(CancelEventArgs e)
  {
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

  public void Cleanup()
  {
    _pokegold.RomChanged -= OnDataChanged;
    _pokegold.DataChanged -= OnDataChanged;
  }

  private void OnDataChanged(object? _, EventArgs __)
  {
    IsRomOpened.Value = _pokegold.IsOpened;
    IsRomChanged.Value = _pokegold.IsChanged;
    RomFileName.Value = _pokegold.FileName;
  }

  public void OpenRom()
  {
    if (_pokegold.IsOpened && _pokegold.IsChanged)
    {
      if (!_dialogs.ShowQuestion("알림", "이미 작업중인 파일이 열려있습니다.\n다른 파일로 열겠습니까?"))
        return;
    }

    var fileName = _dialogs.ShowOpenFile("열기", "지원하는 파일|*.gb;*.gbc;*.bin|모든 파일|*.*");
    if (fileName != null)
    {
      if (!_pokegold.Open(fileName))
      {
        _dialogs.ShowError("알림", "오류가 발생하여 롬 파일을 불러올 수 없습니다.");
      }
      else
      {
        if (_pokegold.Data.Corruptions.Count > 0)
        {
          if (_dialogs.ShowQuestion("알림", "롬파일에서 손상된 데이터가 존재하여 데이터의 일부 손실 또는 대체되었습니다.\n자세한 내역을 확인하겠습니까?"))
            _dialogs.ShowRomCorruption();
        }
      }
    }
  }

  public void SaveRom()
  {
    if (_pokegold.IsOpened)
      _pokegold.Write();
  }

  public void Run()
  {
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
  }

  public void ShowEmulatorSetting()
  {
    var fileName = _dialogs.ShowOpenFile("열기", "실행 가능한 파일|*.exe|모든 파일|*.*");
    if (fileName != null)
    {
      _appSettings.AppSettings.EmulatorPath = fileName;
      _appSettings.Apply();
    }
  }

  public void ShowAppInfo()
  {
    _dialogs.ShowAppInfo();
  }

  public async void CheckUpdate(bool ignoreNoHasUpdate)
  {
    var update = await _updates.GetAppUpdate();
    if (!update.HasUpdate)
    {
      if (!ignoreNoHasUpdate)
        _dialogs.ShowMessage("알림", $"현재 최신 버전입니다.\n(현재 버전 : {Platforms.AppVersion})");
      return;
    }

    var isConfirm = _dialogs.ShowQuestion("알림", $"버전 업데이트가 있습니다.\n· 현재 버전 : {Platforms.AppVersion}\n· 최신 버전 : {update.LatestVersion}\n\n'확인'을 눌러주시면 다운로드 페이지가 열립니다.");
    if (isConfirm)
    {
      var url = "https://github.com/koipkoi/GSEditor/releases";
      Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
  }
}
