namespace GSEditor.Core.PokegoldCore;

public sealed class PGItem
{
  public int Price { get; set; }
  public byte Effect { get; set; }
  public byte Parameter { get; set; }
  public byte Property { get; set; }
  public byte Pocket { get; set; }
  public byte FieldMenu { get; set; }
  public byte BattleMenu { get; set; }

  public static PGItem FromBytes(byte[] bytes)
  {
    return new()
    {
      Price = bytes[0] + (bytes[1] << 8),
      Effect = bytes[2],
      Parameter = bytes[3],
      Property = bytes[4],
      Pocket = bytes[5],
      FieldMenu = (byte)((bytes[6] & 0xf0) >> 4),
      BattleMenu = (byte)((bytes[6] & 0x0f) >> 0),
    };
  }

  public byte[] ToBytes()
  {
    var hiPrice = (byte)(Price & 0x00ff);
    var loPrice = (byte)((Price & 0xff00) >> 8);
    return new byte[] {
      hiPrice, loPrice,
      Effect,
      Parameter,
      Property,
      Pocket,
      (byte)((FieldMenu << 4) | BattleMenu),
    };
  }
}
