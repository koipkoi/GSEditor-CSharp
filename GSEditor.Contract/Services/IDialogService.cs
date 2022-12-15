using GSEditor.Models.Pokegold;
using System.Collections.Generic;

namespace GSEditor.Contract.Services;

public interface IDialogService
{
  public void ShowMessage(string title, string message);
  public void ShowError(string title, string message);
  public bool ShowQuestion(string title, string message);
  public YesNoCancelResult ShowYesNoCancel(string title, string message);

  public string? ShowOpenFile(string title, string filter, string? defaultFileName = null);
  public string? ShowSaveFile(string title, string filter, string? defaultFileName = null);

  public void ShowMain();
  public void ShowAppInfo();

  public Evolution? ShowEvolutionEditor(Evolution? defaultValue);
  public LearnMove? ShowLearnMoveEditor(LearnMove? defaultValue);
  public List<LearnMove> ShowLearnMoveImporter();
}

public enum YesNoCancelResult
{
  Yes, No, Cancel,
}
