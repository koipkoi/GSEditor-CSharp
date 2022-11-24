using GSEditor.Core.PokegoldCore;
using System.IO;

namespace GSEditor.Core;

public sealed partial class Pokegold
{
  private byte[] _data = Array.Empty<byte>();
  private readonly IPokegoldConverter[] _converters = new IPokegoldConverter[] {
    new ColorsConverter(),
    new ImagesConverter(),
    new ItemsConverter(),
    new MovesConverter(),
    new PokedexConverter(),
    new PokemonsConverter(),
    new StringsConverter(),
    new TMHMsConverter(),
  };

  public bool Read(string filename)
  {
    try
    {
      if (!File.Exists(filename))
        return false;

      _data = File.ReadAllBytes(filename);

      foreach (var converter in _converters)
        converter.Read(this);

      IsOpened = true;
      Filename = filename;
      RomChanged?.Invoke(this, EventArgs.Empty);
      NotifyDataChanged(false);

      return true;
    }
    catch { }
    return false;
  }

  public bool Write(string filename)
  {
    try
    {
      foreach (var converter in _converters)
        converter.Write(this);

      // header checksum
      byte headerChecksum = 0;
      for (int i = 0x134; i <= 0x14c; i++)
        headerChecksum = (byte)(headerChecksum - _data[i] - 1);
      _data[0x14d] = headerChecksum;

      // global checksum
      ushort globalChecksum = 0;
      for (int i = 0; i < _data.Length; i++)
      {
        if (i != 0x14e && i != 0x14f)
          globalChecksum += _data[i];
      }
      _data[0x14e] = (byte)((globalChecksum & 0xff00) >> 8);
      _data[0x14f] = (byte)(globalChecksum & 0x00ff >> 0);

      File.WriteAllBytes(filename, _data);
      NotifyDataChanged(false);

      return true;
    }
    catch { }
    return false;
  }

  public byte GetByte(int address)
  {
    return _data[address];
  }

  public byte[] GetBytes(int address, int length)
  {
    var result = new byte[length];
    for (var i = 0; i < length; i++)
      result[i] = _data[address + i];
    return result;
  }

  public int ReadBytes(int address, Func<int, byte, bool> predicate, out byte[] output)
  {
    var bytes = new List<byte>();
    var index = 0;
    while (true)
    {
      var b = _data[address + index];
      if (predicate(index, b))
        break;

      bytes.Add(b);
      index++;
    }

    output = bytes.ToArray();
    return index + 1;
  }

  public void SetBytes(int address, params byte[] bytes)
  {
    for (var i = 0; i < bytes.Length; i++)
      _data[address + i] = bytes[i];
  }

  public void FillBytes(byte data, int address, int length)
  {
    for (var i = 0; i < length; i++)
      _data[address + i] = data;
  }
}
