using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GSEditor.UI.Controls;

public sealed class GBImageBox : Panel
{
  private readonly uint[] _paletteBGR = new uint[256];
  private int _rows = 0, _columns = 0;
  private byte[] _source = Array.Empty<byte>();
  private Color[] _palette = Array.Empty<Color>();

  [Category("Rows")]
  [Description("이미지 타일의 행 개수")]
  public int Rows
  {
    get => _rows;
    set
    {
      _rows = value;
      Invalidate();
    }
  }

  [Category("Columns")]
  [Description("이미지 타일의 열 개수")]
  public int Columns
  {
    get => _columns;
    set
    {
      _columns = value;
      Invalidate();
    }
  }

  [Category("Source")]
  [Description("이미지 데이터")]
  public byte[] Source
  {
    get => _source;
    set
    {
      _source = value;
      Invalidate();
    }
  }

  [Category("Palette")]
  [Description("팔레트")]
  public Color[] Palette
  {
    get => _palette;
    set
    {
      _palette = value;
      for (var i = 0; i < value.Length; i++)
        _paletteBGR[i] = (uint)((value[i].B << 16) | (value[i].G << 8) | (value[i].R << 0));
      Invalidate();
    }
  }

  protected override void OnPaintBackground(PaintEventArgs e)
  {
    base.OnPaintBackground(e);

    var hdc = e.Graphics.GetHdc();
    for (var i = 0; i < Source.Length; i += 2)
    {
      var first = Source[i + 0];
      var second = Source[i + 1];
      var x = (i / (16 * Columns)) * 8;
      var y = (i % (16 * Columns)) / 2;

      for (var pixelX = 0; pixelX < 8; pixelX++)
      {
        var hi = (byte)((second >> (7 - pixelX)) & 1);
        var lo = (byte)((first >> (7 - pixelX)) & 1);
        var colorIndex = (hi << 1) | lo;
        _ = SetPixel(hdc, x + pixelX, y, _paletteBGR[colorIndex]);
      }
    }
  }

  [DllImport("gdi32.dll")]
  private static extern bool SetPixel(IntPtr hdc, int x, int y, uint color);
}
