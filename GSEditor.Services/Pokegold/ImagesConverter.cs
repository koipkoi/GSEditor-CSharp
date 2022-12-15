using GSEditor.Common.Extensions;
using GSEditor.Models.Pokegold;
using System;
using System.Collections.Generic;

namespace GSEditor.Services.Pokegold;

public sealed class ImagesConverter : IPokegoldConverter
{
  private static readonly int[][] _imageAddrs = new int[][] {
    new int[] { 0x0485e2, 0x04bfff, },
    new int[] { 0x054000, 0x057fff, },
    new int[] { 0x058000, 0x05bfff, },
    new int[] { 0x05c000, 0x05ffff, },
    new int[] { 0x060000, 0x063fff, },
    new int[] { 0x064000, 0x067fff, },
    new int[] { 0x068000, 0x06bfff, },
    new int[] { 0x06c000, 0x06ffff, },
    new int[] { 0x070000, 0x073fff, },
    new int[] { 0x074000, 0x077fff, },
    new int[] { 0x078000, 0x07bfff, },
    new int[] { 0x07c09c, 0x07ffff, },
    new int[] { 0x0800c6, 0x083fff, },

		// 추가 공간
		new int[] { 0x088000, 0x08bfff, },
    new int[] { 0x09c000, 0x09ffff, },
    new int[] { 0x0a0000, 0x0a3fff, },
    new int[] { 0x0a4000, 0x0a7fff, },
    new int[] { 0x0b0000, 0x0b3fff, },
    new int[] { 0x0b4000, 0x0b7fff, },
    new int[] { 0x0bc000, 0x0bffff, },
    new int[] { 0x0d0000, 0x0d3fff, },
    new int[] { 0x0d4000, 0x0d7fff, },
    new int[] { 0x160000, 0x163fff, },
    new int[] { 0x18c000, 0x18ffff, },
    new int[] { 0x1a8000, 0x1abfff, },
    new int[] { 0x1ac000, 0x1affff, },
    new int[] { 0x1bc000, 0x1bffff, },
    new int[] { 0x1cc000, 0x1cffff, },
    new int[] { 0x1d0000, 0x1d3fff, },
    new int[] { 0x1d4000, 0x1d7fff, },
    new int[] { 0x1f0000, 0x1f3fff, },
    new int[] { 0x1f4000, 0x1f7fff, },
    new int[] { 0x1f8000, 0x1fbfff, },
  };

  public void Read(PokegoldData data)
  {
    data.Images.Pokemons.Clear();
    data.Images.PokemonBacksides.Clear();
    for (var i = 0; i < 251; i++)
    {
      if (i != 200)
      {
        var bank = data.GetByte(0x48000 + (i * 6)).ToDecodedBank();
        var address = data.GetBytes(0x48001 + (i * 6), 2).ToGBAddress(bank);
        data.Images.Pokemons.Add(data.GetBytes(address, 4096).ToLZDecompressedBytes());

        var backsideBank = data.GetByte(0x48000 + (i * 6) + 3).ToDecodedBank();
        var backsideAddress = data.GetBytes(0x48001 + (i * 6) + 3, 2).ToGBAddress(backsideBank);
        data.Images.PokemonBacksides.Add(data.GetBytes(backsideAddress, 4096).ToLZDecompressedBytes());
      }
      else
      {
        data.Images.Pokemons.Add(Array.Empty<byte>());
        data.Images.PokemonBacksides.Add(Array.Empty<byte>());
      }
    }

    data.Images.Trainers.Clear();
    for (var i = 0; i < 66; i++)
    {
      var bank = data.GetByte(0x80000 + (i * 3)).ToDecodedBank();
      var address = data.GetBytes(0x80001 + (i * 3), 2).ToGBAddress(bank);
      data.Images.Trainers.Add(data.GetBytes(address, 4096).ToLZDecompressedBytes());
    }

    data.Images.Unowns.Clear();
    data.Images.UnownBacksides.Clear();
    for (var i = 0; i < 26; i++)
    {
      var bank = data.GetByte(0x7c000 + (i * 6)).ToDecodedBank();
      var address = data.GetBytes(0x7c001 + (i * 6), 2).ToGBAddress(bank);
      data.Images.Unowns.Add(data.GetBytes(address, 4096).ToLZDecompressedBytes());

      var backsideBank = data.GetByte(0x7c000 + (i * 6) + 3).ToDecodedBank();
      var backsideAddress = data.GetBytes(0x7c001 + (i * 6) + 3, 2).ToGBAddress(backsideBank);
      data.Images.UnownBacksides.Add(data.GetBytes(backsideAddress, 4096).ToLZDecompressedBytes());
    }
  }

  public void Write(PokegoldData data)
  {
    var indexes = new List<int>();

    foreach (var e in _imageAddrs)
    {
      indexes.Add(0);

      var size = e[1] - e[0] + 1;
      data.FillByte(0, e[0], size);
    }

    for (var i = 0; i < 251; i++)
    {
      if (i != 200)
      {
        WriteImage(data, 0x48000 + (i * 6), data.Images.Pokemons[i], indexes);
        WriteImage(data, 0x48003 + (i * 6), data.Images.PokemonBacksides[i], indexes);
      }
    }

    for (var i = 0; i < 66; i++)
      WriteImage(data, 0x80000 + (i * 3), data.Images.Trainers[i], indexes);

    for (var i = 0; i < 26; i++)
    {
      WriteImage(data, 0x7c000 + (i * 6), data.Images.Unowns[i], indexes);
      WriteImage(data, 0x7c003 + (i * 6), data.Images.UnownBacksides[i], indexes);
    }
  }

  private static void WriteImage(PokegoldData data, int pointerAddr, byte[] bytes, List<int> indexes)
  {
    var compressed = bytes.ToLZCompressedBytes();
    var size = compressed.Length;

    for (var i = 0; i < _imageAddrs.Length; i++)
    {
      var e = _imageAddrs[i];
      var newAddr = e[0] + indexes[i];
      if (newAddr + size < e[1])
      {
        var pointer = newAddr.ToGBPointerWithBank();
        data.SetBytes(pointerAddr, pointer[0].ToEncodedBank(), pointer[1], pointer[2]);
        data.SetBytes(newAddr, compressed);
        indexes[i] += size;
        break;
      }
    }
  }
}
