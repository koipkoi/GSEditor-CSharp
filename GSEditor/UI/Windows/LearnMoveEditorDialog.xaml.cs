﻿using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GSEditor.UI.Windows;

public partial class LearnMoveEditorDialog : Window
{
  private readonly IPokegoldService _pokegold = Program.Services.GetRequiredService<IPokegoldService>();

  public LearnMove? Result { get; set; }

  public LearnMoveEditorDialog(LearnMove? defaultValue)
  {
    Title = defaultValue != null ? "배우는 기술 수정" : "배우는 기술 추가";

    InitializeComponent();

    foreach (var e in _pokegold.Data.Strings.MoveNames)
      MoveComboBox.Items.Add(e);

    if (defaultValue != null)
    {
      LevelUpDown.Value = defaultValue.Level;
      MoveComboBox.SelectedIndex = defaultValue.MoveNo;
    }
    else
    {
      LevelUpDown.Value = 5;
      MoveComboBox.SelectedIndex = 0;
    }
  }

  private void OnConfirmClick(object _, RoutedEventArgs __)
  {
    Result = new()
    {
      Level = (byte)(LevelUpDown.Value ?? 0),
      MoveNo = (byte)(MoveComboBox.SelectedIndex + 1),
    };
    DialogResult = true;
    Close();
  }

  private void OnCancelClick(object _, RoutedEventArgs __)
  {
    Close();
  }
}
