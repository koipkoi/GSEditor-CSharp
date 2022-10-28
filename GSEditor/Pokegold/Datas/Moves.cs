namespace GSEditor.Core.PokegoldCore;

public sealed class MovesConverter : IPokegoldConverter
{
  public void Read(Pokegold pokegold)
  {
    pokegold.Moves.Clear();
    for (var i = 0; i < 251; i++)
    {
      var address = 0x4172e + (i * 7);
      var bytes = pokegold.GetBytes(address, 7);
      pokegold.Moves.Add(PGMove.FromBytes(bytes));
    }
  }

  public void Write(Pokegold pokegold)
  {
    for (var i = 0; i < 251; i++)
    {
      var address = 0x4172e + (i * 7);
      var bytes = pokegold.Moves[i].ToBytes();
      pokegold.SetBytes(address, bytes);
    }
  }
}
