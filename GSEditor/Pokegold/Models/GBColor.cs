namespace GSEditor.Core.PokegoldCore;

public sealed class GBColor
{
  private byte _hiByte, _loByte;

  private GBColor() { }

  public static GBColor FromBytes(byte[] bytes)
  {
    return new()
    {
      _hiByte = bytes[1],
      _loByte = bytes[0]
    };
  }

  public static GBColor FromColor(Color color)
  {
    var r = (color.R / 8) << 0;
    var g = (color.G / 8) << 5;
    var b = (color.B / 8) << 10;
    var colorValue = r | g | b;
    return new()
    {
      _hiByte = (byte)((colorValue & 0xff00) >> 8),
      _loByte = (byte)((colorValue & 0x00ff) >> 0)
    };
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
}
