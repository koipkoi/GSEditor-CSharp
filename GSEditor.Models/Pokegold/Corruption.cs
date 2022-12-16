namespace GSEditor.Models.Pokegold;

public sealed record Corruption
{
  public const int LearnMoveCorrupted = 0;
  public const int EvolutionCorrupted = 1;
  public const int ImageCorrupted = 2;
  public const int ImageBacksideCorrupted = 3;
  public const int UnownImageCorrupted = 4;
  public const int UnownImageBacksideCorrupted = 5;
  public const int TrainerImageCorrupted = 6;

  public int Type { get; set; }
  public int Index { get; set; }
}
