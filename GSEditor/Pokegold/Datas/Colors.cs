namespace GSEditor.Core.PokegoldCore;

public sealed class Colors
{
  public List<GBColor[]> Pokemons = new();
  public List<GBColor[]> ShinyPokemons = new();
  public List<GBColor[]> Trainers = new();
}

internal sealed class ColorsConverter : IPokegoldConverter
{
  public void Read(Pokegold pokegold)
  {
    pokegold.Colors.Pokemons.Clear();
    pokegold.Colors.ShinyPokemons.Clear();
    for (var i = 0; i < 251; i++)
    {
      var address = 0xad15 + (i * 8);

      pokegold.Colors.Pokemons.Add(new GBColor[]
      {
        GBColor.FromBytes(pokegold.GetBytes(address + 0, 2)),
        GBColor.FromBytes(pokegold.GetBytes(address + 2, 2)),
      });

      pokegold.Colors.ShinyPokemons.Add(new GBColor[]
      {
        GBColor.FromBytes(pokegold.GetBytes(address + 4, 2)),
        GBColor.FromBytes(pokegold.GetBytes(address + 6, 2)),
      });
    }

    pokegold.Colors.Trainers.Clear();
    for (var i = 0; i < 66; i++)
    {
      var address = 0xb511 + (i * 4);
      pokegold.Colors.Trainers.Add(new GBColor[]
      {
        GBColor.FromBytes(pokegold.GetBytes(address + 0, 2)),
        GBColor.FromBytes(pokegold.GetBytes(address + 2, 2)),
      });
    }
  }

  public void Write(Pokegold pokegold)
  {
    for (var i = 0; i < 251; i++)
    {
      var address = 0xad15 + (i * 8);
      pokegold.SetBytes(address + 0, pokegold.Colors.Pokemons[i][0].ToBytes());
      pokegold.SetBytes(address + 2, pokegold.Colors.Pokemons[i][1].ToBytes());
      pokegold.SetBytes(address + 4, pokegold.Colors.ShinyPokemons[i][0].ToBytes());
      pokegold.SetBytes(address + 6, pokegold.Colors.ShinyPokemons[i][1].ToBytes());
    }

    for (var i = 0; i < 66; i++)
    {
      var address = 0xb511 + (i * 4);
      pokegold.SetBytes(address + 0, pokegold.Colors.Trainers[i][0].ToBytes());
      pokegold.SetBytes(address + 2, pokegold.Colors.Trainers[i][1].ToBytes());
    }
  }
}
