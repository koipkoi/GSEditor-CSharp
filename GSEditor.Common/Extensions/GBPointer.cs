namespace GSEditor.Common.Extensions;

public static class GBPointer
{
  public static byte ToGBBank(this int address)
  {
    return (byte)(address / 0x4000);
  }

  public static byte[] ToGBPointer(this int address)
  {
    var pointer = (address % 0x4000) + (address >= 0x4000 ? 0x4000 : 0);
    return new byte[] {
      (byte)((pointer & 0x00ff) >> 0),
      (byte)((pointer & 0xff00) >> 8)
    };
  }

  public static byte[] ToGBPointerWithBank(this int address)
  {
    var pointer = (address % 0x4000) + (address >= 0x4000 ? 0x4000 : 0);
    return new byte[] {
      (byte)(address / 0x4000),
      (byte)((pointer & 0x00ff) >> 0),
      (byte)((pointer & 0xff00) >> 8)
    };
  }

  public static int ToGBAddress(this byte[] bytes)
  {
    var address = bytes[1] | ((bytes[2]) << 8);
    return (bytes[0] * 0x4000) | (address - 0x4000);
  }

  public static int ToGBAddress(this byte[] bytes, byte bank)
  {
    var address = bytes[0] | ((bytes[1]) << 8);
    return (bank * 0x4000) | (address - 0x4000);
  }

  public static byte ToDecodedBank(this byte bank)
  {
    return bank switch
    {
      0x13 => 0x1f,
      0x14 => 0x20,
      0x1f => 0x2e,
      _ => bank,
    };
  }

  public static byte ToEncodedBank(this byte bank)
  {
    return bank switch
    {
      0x1f => 0x13,
      0x20 => 0x14,
      0x2e => 0x1f,
      _ => bank,
    };
  }
}
