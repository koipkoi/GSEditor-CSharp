namespace GSEditor.Models.Pokegold;

public sealed class Move
{
  public byte No { get; set; }
  public byte Effect { get; set; }
  public byte Power { get; set; }
  public byte MoveType { get; set; }
  public byte Accuracy { get; set; }
  public byte PP { get; set; }
  public byte EffectChance { get; set; }

  public static Move FromBytes(byte[] bytes)
  {
    return new()
    {
      No = bytes[0],
      Effect = bytes[1],
      Power = bytes[2],
      MoveType = bytes[3],
      Accuracy = bytes[4],
      PP = bytes[5],
      EffectChance = bytes[6],
    };
  }

  public byte[] ToBytes()
  {
    return new byte[]
    {
      No,
      Effect,
      Power,
      MoveType,
      Accuracy,
      PP,
      EffectChance,
    };
  }
}
