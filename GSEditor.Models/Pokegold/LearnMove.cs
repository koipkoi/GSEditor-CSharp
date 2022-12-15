using System.Collections.Generic;

namespace GSEditor.Models.Pokegold;

public sealed class LearnMove
{
  public byte Level { get; set; }
  public byte MoveNo { get; set; }

  public static List<LearnMove> FromBytes(byte[] bytes)
  {
    var result = new List<LearnMove>();
    for (var i = 0; i < bytes.Length; i += 2)
    {
      var newItem = new LearnMove
      {
        Level = bytes[i],
        MoveNo = bytes[i + 1],
      };
      result.Add(newItem);
    }
    return result;
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
