using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public sealed class ChecksumConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    // no-op
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
