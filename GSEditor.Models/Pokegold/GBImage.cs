using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GSEditor.Models.Pokegold;

public sealed class GBImage
{
  public int Rows { get; set; }
  public int Columns { get; set; }
  public byte[] Source { get; set; } = Array.Empty<byte>();
  public GBColor[] Colors { get; set; } = Array.Empty<GBColor>();

  public static bool TryLoadFromFile(string fileName, out GBImage? image)
  {
    if (!File.Exists(fileName))
    {
      image = null;
      return false;
    }

    var colors = DecodePalette(fileName);
    if (colors == null)
    {
      image = null;
      return false;
    }

    image = CreateGBImage(fileName, colors);
    return image != null;
  }

  private static GBColor[]? DecodePalette(string fileName)
  {
    var _pngHeader = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };
    var _paletteHeader = "PLTE"u8.ToArray();
    var bytes = File.ReadAllBytes(fileName);

    var headerBytes = new byte[_pngHeader.Length];
    Array.Copy(bytes, 0, headerBytes, 0, _pngHeader.Length);
    if (!headerBytes.SequenceEqual(_pngHeader))
      return null;

    var index = 0;
    var paletteBytes = new byte[_paletteHeader.Length];
    while (true)
    {
      if (index + _paletteHeader.Length >= bytes.Length)
        return null;

      Array.Copy(bytes, index, paletteBytes, 0, _paletteHeader.Length);
      if (paletteBytes.SequenceEqual(_paletteHeader))
      {
        index += 4;
        break;
      }

      index++;
    }

    var result = new GBColor[4];
    for (var i = 0; i < 4; i++)
    {
      result[i] = new()
      {
        R = bytes[index + 0],
        G = bytes[index + 1],
        B = bytes[index + 2],
      };
      index += 3;
    }

    return result;
  }

  private static GBImage? CreateGBImage(string fileName, GBColor[] colors)
  {
    var image = (Bitmap)Image.FromFile(fileName);

    var size = image.Size;
    var rows = size.Height / 8;
    var columns = size.Width / 8;

    var bytes = new List<byte>();
    for (var r = 0; r < rows; r++)
    {
      for (var c = 0; c < columns; c++)
      {
        for (var y = 0; y < 8; y++)
        {
          byte first = 0;
          byte second = 0;

          for (var x = 0; x < 8; x++)
          {
            var indexX = (r * 8) + x;
            var indexY = (c * 8) + y;
            var color = image.GetPixel(indexX, indexY);
            var colorIndex = 0;
            foreach (var e in colors)
            {
              if (e == color)
                break;
              colorIndex++;
            }

            var hi = (colorIndex & 0b00000010) >> 1;
            var lo = (colorIndex & 0b00000001) >> 0;
            first |= (byte)(lo << (7 - x));
            second |= (byte)(hi << (7 - x));
          }

          bytes.Add(first);
          bytes.Add(second);
        }
      }
    }

    return new()
    {
      Rows = rows,
      Columns = columns,
      Source = bytes.ToArray(),
      Colors = colors,
    };
  }

  public bool WriteFile(string fileName)
  {
    try
    {
      var bitmap = new Bitmap(Rows * 8, Columns * 8, PixelFormat.Format32bppArgb);
      var resultBitmap = new Bitmap(Rows * 8, Columns * 8, PixelFormat.Format8bppIndexed);

      for (var i = 0; i < Source.Length; i += 2)
      {
        var first = Source[i + 0];
        var second = Source[i + 1];
        var x = (i / (16 * Columns)) * 8;
        var y = (i % (16 * Columns)) / 2;

        for (var pixelX = 0; pixelX < 8; pixelX++)
        {
          var hi = (byte)((second >> (7 - pixelX)) & 1);
          var lo = (byte)((first >> (7 - pixelX)) & 1);
          var colorIndex = (hi << 1) | lo;
          bitmap.SetPixel(x + pixelX, y, Color.FromArgb(255, Colors[colorIndex].R, Colors[colorIndex].G, Colors[colorIndex].B));
        }
      }

      var newPalette = resultBitmap.Palette;
      for (var i = 0; i < 256; i++)
      {
        if (i < Colors.Length)
        {
          newPalette.Entries[i] = Color.FromArgb(255, Colors[i].R, Colors[i].G, Colors[i].B);
          continue;
        }
        newPalette.Entries[i] = Color.Black;
      }
      resultBitmap.Palette = newPalette;

      var data = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.ReadWrite, resultBitmap.PixelFormat);
      var rawBytes = new byte[data.Stride * data.Height];
      var rgbBytes = GetBitmapBytes(bitmap);
      int stride = rgbBytes.Length / bitmap.Height;
      int colLimit = bitmap.Width * 4;

      for (var row = 0; row < bitmap.Height; row++)
      {
        for (var col = 0; col < colLimit; col += 4)
        {
          int offset = row * stride + col;
          var color = Color.FromArgb(255, rgbBytes[offset + 2], rgbBytes[offset + 1], rgbBytes[offset]);
          int index = row * data.Stride + col / 4;
          int colorIndex = 0;
          foreach (var e in Colors)
          {
            if (e == color)
              break;
            colorIndex++;
          }
          rawBytes[index] = (byte)colorIndex;
        }
      }

      Marshal.Copy(rawBytes, 0, data.Scan0, rawBytes.Length);

      resultBitmap.UnlockBits(data);
      resultBitmap.Save(fileName);

      return true;
    }
    catch { }
    return false;
  }

  private static byte[] GetBitmapBytes(Bitmap src)
  {
    var rect = new Rectangle(0, 0, src.Width, src.Height);
    var bmpData = src.LockBits(rect, ImageLockMode.ReadOnly, src.PixelFormat);
    var ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * src.Height;
    var rgbValues = new byte[bytes];
    Marshal.Copy(ptr, rgbValues, 0, bytes);
    src.UnlockBits(bmpData);
    return rgbValues;
  }
}
