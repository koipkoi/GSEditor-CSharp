using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public sealed class TMHMsConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    data.TMHMs.Clear();
    for (var i = 0; i < 57; i++)
      data.TMHMs.Add(data.GetByte(0x119f5 + i));
  }

  public void Write(PokegoldData data)
  {
    for (var i = 0; i < 57; i++)
      data.SetBytes(0x119f5 + i, data.TMHMs[i]);
  }
}
