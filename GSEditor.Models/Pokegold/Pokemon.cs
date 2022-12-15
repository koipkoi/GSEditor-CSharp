using System.Collections.Generic;

namespace GSEditor.Models.Pokegold;

public sealed class Pokemon
{
  private static readonly byte[] bits = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
  private static readonly Dictionary<byte, byte> _genderRates = new()
  {
    [0x00] = 0,
    [0x1f] = 1,
    [0x3f] = 2,
    [0x5f] = 3,
    [0x7f] = 4,
    [0x9f] = 5,
    [0xbf] = 6,
    [0xdf] = 7,
    [0xfe] = 8,
    [0xff] = 9,
  };

  public byte No { get; set; }
  public byte HP { get; set; }
  public byte Attack { get; set; }
  public byte Defence { get; set; }
  public byte Speed { get; set; }
  public byte SpAttack { get; set; }
  public byte SpDefence { get; set; }
  public byte Type1 { get; set; }
  public byte Type2 { get; set; }
  public byte CatchRate { get; set; }
  public byte EXP { get; set; }
  public byte Item1 { get; set; }
  public byte Item2 { get; set; }
  public byte GenderRate { get; set; }
  public byte Unk1 { get; set; }
  public byte EggType { get; set; }
  public byte Unk2 { get; set; }
  public byte ImageTileSize { get; set; }
  public byte Padding1 { get; set; }
  public byte Padding2 { get; set; }
  public byte Padding3 { get; set; }
  public byte Padding4 { get; set; }
  public byte GrowthRate { get; set; }
  public byte EggGroup1 { get; set; }
  public byte EggGroup2 { get; set; }

  public bool[] TMHMs { get; } = new bool[64];

  public List<Evolution> Evolutions { get; } = new();
  public List<LearnMove> LearnMoves { get; } = new();

  private Pokemon() { }

  public static Pokemon FromBytes(byte[] bytes)
  {
    var newItem = new Pokemon
    {
      No = bytes[0],
      HP = bytes[1],
      Attack = bytes[2],
      Defence = bytes[3],
      Speed = bytes[4],
      SpAttack = bytes[5],
      SpDefence = bytes[6],
      Type1 = bytes[7],
      Type2 = bytes[8],
      CatchRate = bytes[9],
      EXP = bytes[10],
      Item1 = bytes[11],
      Item2 = bytes[12],
      GenderRate = _genderRates[bytes[13]],
      Unk1 = bytes[14],
      EggType = bytes[15],
      Unk2 = bytes[16],
      ImageTileSize = bytes[17] switch
      {
        0x55 => 5,
        0x66 => 6,
        0x77 => 7,
        _ => 5,
      },
      Padding1 = bytes[18],
      Padding2 = bytes[19],
      Padding3 = bytes[20],
      Padding4 = bytes[21],
      GrowthRate = bytes[22],
      EggGroup1 = (byte)((bytes[23] & 0xf0) >> 4),
      EggGroup2 = (byte)((bytes[23] & 0x0f) >> 0),
    };

    for (var i = 0; i < 8; i++)
    {
      for (var j = 0; j < 8; j++)
      {
        var index = (i * 8) + j;
        newItem.TMHMs[index] = (bytes[24 + i] & bits[j]) != 0;
      }
    }

    return newItem;
  }

  public byte[] ToBytes()
  {
    byte _genderRate = 0;
    foreach (var key in _genderRates.Keys)
    {
      var e = _genderRates[key];
      if (e == GenderRate)
      {
        _genderRate = key;
        break;
      }
    }

    var bytes = new byte[]
    {
      No,
      HP,
      Attack,
      Defence,
      Speed,
      SpAttack,
      SpDefence,
      Type1,
      Type2,
      CatchRate,
      EXP,
      Item1,
      Item2,
      _genderRate,
      Unk1,
      EggType,
      Unk2,
      ImageTileSize switch
      {
        5 => 0x55,
        6 => 0x66,
        7 => 0x77,
        _ => 0x55,
      },
      Padding1,
      Padding2,
      Padding3,
      Padding4,
      GrowthRate,
      (byte)((EggGroup1 << 4)| EggGroup2),
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
    };

    for (var i = 0; i < 8; i++)
    {
      for (var j = 0; j < 8; j++)
      {
        if (TMHMs[(i * 8) + j])
          bytes[24 + i] = (byte)(bytes[24 + i] | bits[j]);
      }
    }

    return bytes;
  }
}
