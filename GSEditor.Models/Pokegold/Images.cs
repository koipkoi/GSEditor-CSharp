using System.Collections.Generic;

namespace GSEditor.Models.Pokegold;

public sealed class Images
{
  public List<byte[]> Pokemons { get; } = new();
  public List<byte[]> PokemonBacksides { get; } = new();
  public List<byte[]> Trainers { get; } = new();
  public List<byte[]> Unowns { get; } = new();
  public List<byte[]> UnownBacksides { get; } = new();
}
