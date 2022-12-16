using System.Collections.Generic;

namespace GSEditor.Models.Pokegold;

public sealed class LearnMove
{
  public byte Level { get; set; }
  public byte MoveNo { get; set; }

  public static bool TryParseFromBytes(byte[] bytes, out List<LearnMove> result)
  {
    result = new List<LearnMove>();
    for (var i = 0; i < bytes.Length; i += 2)
    {
      try
      {
        var newItem = new LearnMove
        {
          Level = bytes[i],
          MoveNo = bytes[i + 1],
        };
        result.Add(newItem);
      }
      catch
      {
        return false;
      }
    }
    return true;
  }

  public byte[] ToBytes()
  {
    return new byte[]
    {
      Level,
      MoveNo,
    };
  }
}
