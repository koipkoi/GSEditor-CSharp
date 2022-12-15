using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public sealed class ColorsConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    data.Colors.Pokemons.Clear();
    data.Colors.ShinyPokemons.Clear();
    for (var i = 0; i < 251; i++)
    {
      var address = 0xad15 + (i * 8);

      data.Colors.Pokemons.Add(new GBColor[]
      {
        new (data.GetBytes(address + 0, 2)),
        new (data.GetBytes(address + 2, 2)),
      });

      data.Colors.ShinyPokemons.Add(new GBColor[]
      {
        new(data.GetBytes(address + 4, 2)),
        new(data.GetBytes(address + 6, 2)),
      });
    }

    data.Colors.Trainers.Clear();
    for (var i = 0; i < 66; i++)
    {
      var address = 0xb511 + (i * 4);
      data.Colors.Trainers.Add(new GBColor[]
      {
        new(data.GetBytes(address + 0, 2)),
        new(data.GetBytes(address + 2, 2)),
      });
    }
  }

  public void Write(PokegoldData data)
  {
    for (var i = 0; i < 251; i++)
    {
      var address = 0xad15 + (i * 8);
      data.SetBytes(address + 0, data.Colors.Pokemons[i][0].ToBytes());
      data.SetBytes(address + 2, data.Colors.Pokemons[i][1].ToBytes());
      data.SetBytes(address + 4, data.Colors.ShinyPokemons[i][0].ToBytes());
      data.SetBytes(address + 6, data.Colors.ShinyPokemons[i][1].ToBytes());
    }

    for (var i = 0; i < 66; i++)
    {
      var address = 0xb511 + (i * 4);
      data.SetBytes(address + 0, data.Colors.Trainers[i][0].ToBytes());
      data.SetBytes(address + 2, data.Colors.Trainers[i][1].ToBytes());
    }
  }
}
