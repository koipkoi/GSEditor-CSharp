namespace GSEditor.Core.PokegoldCore;

public sealed class PGLearnMove
{
  public byte Level { get; set; }
  public byte MoveNo { get; set; }

  public static List<PGLearnMove> FromBytes(byte[] bytes)
  {
    var result = new List<PGLearnMove>();
    for (var i = 0; i < bytes.Length; i += 2)
    {
      if (i + 1 < bytes.Length)
      {
        var newItem = new PGLearnMove
        {
          Level = bytes[i],
          MoveNo = bytes[i + 1],
        };
        result.Add(newItem);
      }
    }
    return result;
  }

  public byte[] ToBytes()
  {
    return new byte[] { Level, MoveNo, };
  }
}
