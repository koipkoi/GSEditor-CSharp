using GSEditor.Models.Pokegold;
using System;

namespace GSEditor.Services.Pokegold;

public sealed class ChecksumConverter : IPokegoldConverter
{
  private static readonly byte[][] _defaultImageBytes = new byte[][]
  {
    /* 0 */ Array.Empty<byte>(),
    /* 1 */ Array.Empty<byte>(),
    /* 2 */ Array.Empty<byte>(),
    /* 3 */ Array.Empty<byte>(),
    /* 4 */ Array.Empty<byte>(),
    /* 5 */ new byte[0x190],
    /* 6 */ new byte[0x240],
    /* 7 */ new byte[0x310],
  };

  private static bool IsNotImageByte(byte[] bytes)
  {
    return !(bytes.Length == 0x190 || bytes.Length == 0x240 || bytes.Length == 0x310);
  }

  public void Read(PokegoldData data)
  {
    // 포켓몬 이미지 : 깨진 데이터 대체
    for (var i = 0; i < 251; i++)
    {
      if (i != 200)
      {
        if (IsNotImageByte(data.Images.Pokemons[i]))
        {
          data.Images.Pokemons[i] = _defaultImageBytes[data.Pokemons[i].ImageTileSize];
          data.Corruptions.Add(new()
          {
            Type = Corruption.ImageCorrupted,
            Index = i,
          });
        }
        if (IsNotImageByte(data.Images.PokemonBacksides[i]))
        {
          data.Images.PokemonBacksides[i] = _defaultImageBytes[6];
          data.Corruptions.Add(new()
          {
            Type = Corruption.ImageBacksideCorrupted,
            Index = i,
          });
        }
      }
    }

    // 트레이너 이미지 : 깨진 데이터 대체
    for (var i = 0; i < 66; i++)
    {
      if (IsNotImageByte(data.Images.Trainers[i]))
      {
        data.Images.Trainers[i] = _defaultImageBytes[7];
        data.Corruptions.Add(new()
        {
          Type = Corruption.TrainerImageCorrupted,
          Index = i,
        });
      }
    }

    // 안농 이미지 : 깨진 데이터 대체
    for (var i = 0; i < 26; i++)
    {
      if (IsNotImageByte(data.Images.Unowns[i]))
      {
        data.Images.Unowns[i] = _defaultImageBytes[5];
        data.Corruptions.Add(new()
        {
          Type = Corruption.UnownImageCorrupted,
          Index = i,
        });
      }
      if (IsNotImageByte(data.Images.UnownBacksides[i]))
      {
        data.Images.UnownBacksides[i] = _defaultImageBytes[6];
        data.Corruptions.Add(new()
        {
          Type = Corruption.UnownImageBacksideCorrupted,
          Index = i,
        });
      }
    }
  }

  public void Write(PokegoldData data)
  {
    // header checksum
    byte headerChecksum = 0;
    for (var i = 0x134; i <= 0x14c; i++)
    {
      var b = data.GetByte(i);
      headerChecksum = (byte)(headerChecksum - b - 1);
    }
    data.SetByte(0x14d, headerChecksum);

    // global checksum
    ushort globalChecksum = 0;
    data.ForEachBytes((i, b) =>
    {
      if (i != 0x14e && i != 0x14f)
        globalChecksum += b;
    });
    data.SetByte(0x14e, (byte)((globalChecksum & 0xff00) >> 8));
    data.SetByte(0x14f, (byte)((globalChecksum & 0x00ff) >> 0));
  }
}
