using System;
using System.Collections.Generic;

namespace GSEditor.Models.Pokegold;

public sealed class PokegoldData
{
  public byte[]? Bytes { get; set; }

  public Colors Colors { get; } = new();
  public Images Images { get; } = new();
  public Strings Strings { get; } = new();
  public List<Item> Items { get; } = new();
  public List<Move> Moves { get; } = new();
  public List<Pokedex> Pokedex { get; } = new();
  public List<Pokemon> Pokemons { get; } = new();
  public List<byte> TMHMs { get; } = new();
  public List<Corruption> Corruptions { get; } = new();

  public byte GetByte(int address)
  {
    if (Bytes == null)
      return 0;
    return Bytes[address];
  }

  public byte[] GetBytes(int address, int length)
  {
    if (Bytes == null)
      return Array.Empty<byte>();
    if (address < 0 || length == 0 || address + length > Bytes.Length)
      return Array.Empty<byte>();
    var result = new byte[length];
    for (var i = 0; i < length; i++)
      result[i] = Bytes[address + i];
    return result;
  }

  public byte[] GetBytes(int address, Func<byte, bool> predicate)
  {
    if (Bytes == null)
      return Array.Empty<byte>();
    var bytes = new List<byte>();
    var index = 0;
    while (true)
    {
      var b = Bytes[address + index];
      if (predicate(b))
        break;
      bytes.Add(b);
      index++;
    }
    return bytes.ToArray();
  }

  public void ForEachBytes(Action<int, byte> action)
  {
    if (Bytes != null)
      ForEachBytes(0, Bytes.Length, action);
  }

  public void ForEachBytes(int address, Action<int, byte> action)
  {
    if (Bytes != null)
      ForEachBytes(address, Bytes.Length - address, action);
  }

  public void ForEachBytes(int address, int length, Action<int, byte> action)
  {
    if (Bytes != null)
    {
      for (var i = address; i < length; i++)
        action(i, Bytes[i]);
    }
  }

  public void SetByte(int address, byte b)
  {
    if (Bytes != null)
      Bytes[address] = b;
  }

  public void SetBytes(int address, params byte[] bytes)
  {
    if (Bytes != null)
    {
      for (var i = 0; i < bytes.Length; i++)
        Bytes[address + i] = bytes[i];
    }
  }

  public void FillByte(byte b, int address, int length)
  {
    if (Bytes != null)
    {
      for (var i = 0; i < length; i++)
        Bytes[address + i] = b;
    }
  }
}
