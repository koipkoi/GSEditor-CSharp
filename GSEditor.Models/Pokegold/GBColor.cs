using System.Windows.Media;

namespace GSEditor.Models.Pokegold;

public sealed class GBColor
{
  public static readonly GBColor GBBlack = new(0, 0, 0);
  public static readonly GBColor GBWhite = new(0xff, 0xff, 0xff);

  private byte _hiByte, _loByte;

  public byte R
  {
    get
    {
      var colorValue = (_hiByte << 8) | _loByte;
      return (byte)(((colorValue & 0b000000000011111) >> 0) * 8);
    }
    set
    {
      var colorValue = (_hiByte << 8) | _loByte;
      colorValue = (colorValue & 0b111111111100000) | ((value / 8) << 0);
      _hiByte = (byte)((colorValue & 0xff00) >> 8);
      _loByte = (byte)((colorValue & 0x00ff) >> 0);
    }
  }

  public byte G
  {
    get
    {
      var colorValue = (_hiByte << 8) | _loByte;
      return (byte)(((colorValue & 0b000001111100000) >> 5) * 8);
    }
    set
    {
      var colorValue = (_hiByte << 8) | _loByte;
      colorValue = (colorValue & 0b111110000011111) | ((value / 8) << 5);
      _hiByte = (byte)((colorValue & 0xff00) >> 8);
      _loByte = (byte)((colorValue & 0x00ff) >> 0);
    }
  }

  public byte B
  {
    get
    {
      var colorValue = (_hiByte << 8) | _loByte;
      return (byte)(((colorValue & 0b111110000000000) >> 10) * 8);
    }
    set
    {
      var colorValue = (_hiByte << 8) | _loByte;
      colorValue = (colorValue & 0b000001111111111) | ((value / 8) << 10);
      _hiByte = (byte)((colorValue & 0xff00) >> 8);
      _loByte = (byte)((colorValue & 0x00ff) >> 0);
    }
  }

  public GBColor() { }

  public GBColor(byte[] bytes)
  {
    _hiByte = bytes[1];
    _loByte = bytes[0];
  }

  public GBColor(byte r, byte g, byte b) : this(Color.FromArgb(0xff, r, g, b)) { }

  public GBColor(Color color)
  {
    var r = (color.R / 8) << 0;
    var g = (color.G / 8) << 5;
    var b = (color.B / 8) << 10;
    var colorValue = r | g | b;
    _hiByte = (byte)((colorValue & 0xff00) >> 8);
    _loByte = (byte)((colorValue & 0x00ff) >> 0);
  }

  public byte[] ToBytes()
  {
    return new byte[] { _loByte, _hiByte, };
  }

  public Color ToColor()
  {
    var colorValue = (_hiByte << 8) | _loByte;
    var r = (byte)(((colorValue & 0b000000000011111) >> 0) * 8);
    var g = (byte)(((colorValue & 0b000001111100000) >> 5) * 8);
    var b = (byte)(((colorValue & 0b111110000000000) >> 10) * 8);
    return Color.FromArgb(255, r, g, b);
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(this, obj))
      return true;
    if (obj is null)
      return false;
    if (obj is GBColor gbc)
      return R == gbc.R && G == gbc.G && B == gbc.B;
    if (obj is Color c)
      return R == c.R && G == c.G && B == c.B;
    if (obj is System.Drawing.Color dc)
      return R == dc.R && G == dc.G && B == dc.B;
    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }

  public static bool operator ==(GBColor a, GBColor b)
  {
    return a.R == b.R && a.G == b.G && a.B == b.B;
  }

  public static bool operator !=(GBColor a, GBColor b)
  {
    return !(a.R == b.R && a.G == b.G && a.B == b.B);
  }

  public static bool operator ==(GBColor a, Color b)
  {
    return a.R == b.R && a.G == b.G && a.B == b.B;
  }

  public static bool operator !=(GBColor a, Color b)
  {
    return !(a.R == b.R && a.G == b.G && a.B == b.B);
  }

  public static bool operator ==(Color a, GBColor b)
  {
    return a.R == b.R && a.G == b.G && a.B == b.B;
  }

  public static bool operator !=(Color a, GBColor b)
  {
    return !(a.R == b.R && a.G == b.G && a.B == b.B);
  }

  public static bool operator ==(GBColor a, System.Drawing.Color b)
  {
    return a.R == b.R && a.G == b.G && a.B == b.B;
  }

  public static bool operator !=(GBColor a, System.Drawing.Color b)
  {
    return !(a.R == b.R && a.G == b.G && a.B == b.B);
  }

  public static bool operator ==(System.Drawing.Color a, GBColor b)
  {
    return a.R == b.R && a.G == b.G && a.B == b.B;
  }

  public static bool operator !=(System.Drawing.Color a, GBColor b)
  {
    return !(a.R == b.R && a.G == b.G && a.B == b.B);
  }
}
