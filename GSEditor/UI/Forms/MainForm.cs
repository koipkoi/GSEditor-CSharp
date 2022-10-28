using GSEditor.Core;
using System.ComponentModel;

namespace GSEditor.UI.Forms;

public partial class MainForm : Form
{
  private readonly Pokegold _pokegold = Injector.Get<Pokegold>();

  public MainForm()
  {
    InitializeComponent();
  }

  protected override void OnClosing(CancelEventArgs e)
  {
    base.OnClosing(e);
  }

  protected override void OnFormClosed(FormClosedEventArgs e)
  {
    base.OnFormClosed(e);
  }

  private void OnMenuFileOpenClick(object _, EventArgs __)
  {
    // todo 추가
  }

  private void OnMenuFileSaveClick(object _, EventArgs __)
  {
    // todo 추가
  }

  private void OnMenuFileExitClick(object _, EventArgs __)
  {
    Close();
  }

  private void OnMenuGamePlayClick(object _, EventArgs __)
  {
    // todo 추가
  }

  private void OnMenuGameSettingsClick(object sender, EventArgs e)
  {
    // todo 추가
  }
}
