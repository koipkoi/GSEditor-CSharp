using GSEditor.Common.Extensions;
using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using GSEditor.UI.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows;

namespace GSEditor.Services;

public class DialogService : IDialogService
{
  private readonly ISettingsService _settings = App.Services.GetRequiredService<ISettingsService>();

  public void ShowMessage(string title, string message)
  {
    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
  }

  public void ShowError(string title, string message)
  {
    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
  }

  public bool ShowQuestion(string title, string message)
  {
    return MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
  }

  public YesNoCancelResult ShowYesNoCancel(string title, string message)
  {
    switch (MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
    {
      case MessageBoxResult.Yes:
        return YesNoCancelResult.Yes;
      case MessageBoxResult.No:
        return YesNoCancelResult.No;
    }
    return YesNoCancelResult.Cancel;
  }

  public string? ShowOpenFile(string title, string filter, string? defaultFileName = null)
  {
    var dialog = new OpenFileDialog
    {
      Title = title,
      Filter = filter,
      FileName = defaultFileName,
    };
    if (dialog.ShowDialog() ?? false)
      return dialog.FileName;
    return null;
  }

  public string? ShowSaveFile(string title, string filter, string? defaultFileName = null)
  {
    var dialog = new SaveFileDialog
    {
      Title = title,
      Filter = filter,
      FileName = defaultFileName,
    };
    if (dialog.ShowDialog() ?? false)
      return dialog.FileName;
    return null;
  }

  public void ShowMain()
  {
    var window = new MainWindow();
    window.SetWindowSetting(_settings.AppSettings.MainWindowSetting);
    window.ShowDialog();

    _settings.AppSettings.MainWindowSetting = window.GetWindowSetting();
    _settings.Apply();
  }

  public void ShowAppInfo()
  {
    var dialog = new AppInfoDialog();
    dialog.ShowDialogCenterOwner();
  }

  public void ShowRomCorruption()
  {
    var dialog = new RomCorruptionsDialog();
    dialog.ShowDialogCenterOwner();
  }

  public Evolution? ShowEvolutionEditor(Evolution? defaultValue)
  {
    var dialog = new EvolutionEditorDialog(defaultValue);
    dialog.ShowDialogCenterOwner();
    return dialog.Result;
  }

  public LearnMove? ShowLearnMoveEditor(LearnMove? defaultValue)
  {
    var dialog = new LearnMoveEditorDialog(defaultValue);
    dialog.ShowDialogCenterOwner();
    return dialog.Result;
  }

  public List<LearnMove> ShowLearnMoveImporter()
  {
    var dialog = new LearnMoveImporterDialog();
    dialog.ShowDialogCenterOwner();
    return dialog.Result;
  }
}
