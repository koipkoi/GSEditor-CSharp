namespace GSEditor.Core.PokegoldCore;

public sealed class PGPokedex
{
  public string SpecificName { get; set; } = string.Empty;
  public byte Height { get; set; }
  public int Weight { get; set; }
  public string Description { get; set; } = string.Empty;
}
