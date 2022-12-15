using System.Collections.Generic;

namespace GSEditor.Models.Pokegold;

public sealed class Strings
{
  public List<string> ItemNames { get; } = new();
  public List<string> TrainerClassNames { get; } = new();
  public List<string> PokemonNames { get; } = new();
  public List<string> MoveNames { get; } = new();
  public List<string> MoveTypeNames { get; } = new();

  public List<string> ItemDescriptions { get; } = new();
  public List<string> MoveDescriptions { get; } = new();
}
