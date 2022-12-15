using System;
using System.Collections.Generic;
using System.Text;

namespace GSEditor.Common.Extensions;

public static class TextConverter
{
  private static readonly Dictionary<int, string> _charmap = new();
  private static readonly Dictionary<string, int> _charmapReverse = new();

  static TextConverter()
  {
    var lines = Common.Properties.Resources.Charmap.Replace("\r\n", "\n").Split("\n");
    foreach (var line in lines)
    {
      var keyValue = line.Split('=');
      if (keyValue.Length == 2)
      {
        try
        {
          var key = Convert.ToInt32(keyValue[0], 16);
          _charmap[key] = keyValue[1];
          _charmapReverse[keyValue[1]] = key;
        }
        catch { }
      }
    }
  }

  public static string TextDecode(this byte[] bytes)
  {
    var sb = new StringBuilder();
    for (var i = 0; i < bytes.Length; i++)
    {
      var b = bytes[i];
      if (b >= 0x1 && b <= 0xb)
      {
        var charmapIndex = (b << 8) | bytes[i + 1];
        sb.Append(_charmap[charmapIndex]);
        i++;
      }
      else
      {
        if (_charmap.TryGetValue(b, out var value) && value != null)
          sb.Append(value);
        else
          sb.Append($"[{b.ToString("x2")}]");
      }
    }
    return sb.ToString();
  }

  public static bool TryTextEncode(this string str, out byte[] outBytes)
  {
    var bytes = new List<byte>();

    str = str.Replace("\r\n", "\n");

    for (var i = 0; i < str.Length;)
    {
      var seek = 0;

      for (var j = 0; j < 16; j++)
      {
        seek++;

        if (i + j >= str.Length)
        {
          outBytes = Array.Empty<byte>();
          return false;
        }

        var sub = str.Substring(i, j + 1);

        // 등록된 문자 존재
        if (_charmapReverse.TryGetValue(sub, out int value))
        {
          var n = value;
          if (n > 0xff)
          {
            bytes.Add((byte)((n & 0xff00) >> 8));
            bytes.Add((byte)(n & 0xff));
          }
          else
          {
            bytes.Add((byte)n);
          }
          break;
        }

        // 숫자 처리
        if (sub.StartsWith("[") && sub.EndsWith("]"))
        {
          var numStr = sub[1..^1];
          try
          {
            var num = Convert.ToInt32(numStr, 16);
            if (num <= 0xff)
            {
              bytes.Add((byte)num);
              break;
            }
            else
            {
              outBytes = Array.Empty<byte>();
              return false;
            }
          }
          catch
          {
            outBytes = Array.Empty<byte>();
            return false;
          }
        }
      }

      i += seek;
    }

    outBytes = bytes.ToArray();
    return true;
  }

  public static byte[] TextEncode(this string str)
  {
    TryTextEncode(str, out byte[] result);
    return result;
  }
}
