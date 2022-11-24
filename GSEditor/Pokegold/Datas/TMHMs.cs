namespace GSEditor.Core.PokegoldCore;

public sealed class TMHMsConverter : IPokegoldConverter
{
  public void Read(Pokegold pokegold)
  {
    pokegold.TMHMs.Clear();
    for (var i = 0; i < 57; i++)
      pokegold.TMHMs.Add(pokegold.GetByte(0x119f5 + i));
  }

  public void Write(Pokegold pokegold)
  {
    for (var i = 0; i < 57; i++)
      pokegold.SetBytes(0x119f5 + i, pokegold.TMHMs[i]);
  }
}
