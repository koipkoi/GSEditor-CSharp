namespace GSEditor.Core.PokegoldCore;

public sealed class PGMove
{
  public byte Animation { get; set; }
  public byte Effect { get; set; }
  public byte Power { get; set; }
  public byte MoveType { get; set; }
  public byte Accuracy { get; set; }
  public byte PP { get; set; }
  public byte EffectChance { get; set; }

  public static PGMove FromBytes(byte[] bytes)
  {
    return new()
    {
      Animation = bytes[0],
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
    return new byte[] { Animation, Effect, Power, MoveType, Accuracy, PP, EffectChance, };
  }
}
