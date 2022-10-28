using System.ComponentModel;

namespace GSEditor.UI.Controls;

public class CustomListBox : ListBox
{
  [Category("OddBackColor")]
  [Description("홀수 항목의 배경 색상")]
  public Color OddBackColor { get; set; } = Color.White;

  [Category("EvenBackColor")]
  [Description("짝수 항목의 배경 색상")]
  public Color EvenBackColor { get; set; } = Color.AliceBlue;

  [Category("SelectedBackColor")]
  [Description("선택 항목의 배경 색상")]
  public Color SelectedBackColor { get; set; } = SystemColors.Highlight;

  public CustomListBox()
  {
    DrawMode = DrawMode.OwnerDrawVariable;
  }

  protected override void OnMeasureItem(MeasureItemEventArgs e)
  {
    base.OnMeasureItem(e);

    if (e.Index < Items.Count)
    {
      var lines = Items[e.Index].ToString()!.Replace("\r\n", "\n").Split('\n').Length;
      if (lines > 0)
        e.ItemHeight = Font.Height * lines;
    }
  }

  protected override void OnDrawItem(DrawItemEventArgs e)
  {
    base.OnDrawItem(e);

    if (e.Index == -1 || Items.Count == 0)
      return;

    var isSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
    var color = isSelected
        ? SelectedBackColor
        : (e.Index % 2 == 0) ? OddBackColor : EvenBackColor;
    var backgroundBrush = new SolidBrush(color);
    var textBrush = new SolidBrush(e.ForeColor);
    e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
    e.Graphics.DrawString(Items[e.Index].ToString(), e.Font!, textBrush, e.Bounds, StringFormat.GenericDefault);
    backgroundBrush.Dispose();
    textBrush.Dispose();
    e.DrawFocusRectangle();
  }
}
