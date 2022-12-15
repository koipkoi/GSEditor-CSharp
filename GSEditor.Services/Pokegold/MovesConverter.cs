using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public sealed class MovesConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    data.Moves.Clear();
    for (var i = 0; i < 251; i++)
    {
      var address = 0x4172e + (i * 7);
      var bytes = data.GetBytes(address, 7);
      data.Moves.Add(Move.FromBytes(bytes));
    }
  }

  public void Write(PokegoldData data)
  {
    for (var i = 0; i < 251; i++)
    {
      var address = 0x4172e + (i * 7);
      var bytes = data.Moves[i].ToBytes();
      data.SetBytes(address, bytes);
    }
  }
}
