using GSEditor.Core.PokegoldCore;

namespace GSEditor.Core;

public sealed partial class Pokegold
{
  public bool IsChanged { get; private set; } = false;
  public bool IsOpened { get; private set; } = false;
  public string Filename { get; private set; } = "-";

  public Colors Colors { get; } = new();
  public Images Images { get; } = new();
  public List<PGItem> Items { get; } = new();
  public List<PGMove> Moves { get; } = new();
  public List<PGPokedex> Pokedex { get; } = new();
  public List<PGPokemon> Pokemons { get; } = new();
  public Strings Strings { get; } = new();
}
