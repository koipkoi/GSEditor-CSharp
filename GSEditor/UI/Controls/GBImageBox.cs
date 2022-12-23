using GSEditor.Models.Pokegold;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GSEditor.UI.Controls;

public sealed class GBImageBox : Image
{
  private GBImage? _gbImage;
  public GBImage? GBImage
  {
    get => _gbImage;
    set
    {
      _gbImage = value;
      InvalidateGBImage();
    }
  }

  private void InvalidateGBImage()
  {
    if (GBImage == null)
      return;

    var width = GBImage.Columns * 8;
    var height = GBImage.Rows * 8;
    var pixels = new byte[width * height * 4];
    var writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

    for (int i = 0; i < GBImage.Source.Length; i += 2)
    {
      byte firstByte = GBImage.Source[i];
      byte secondByte = GBImage.Source[i + 1];
      int cursorX = i / (16 * GBImage.Columns);
      int y = i % (16 * GBImage.Columns) / 2;
      for (int x = 0; x < 8; x++)
      {
        byte highBit = (byte)((secondByte >> (7 - x)) & 1);
        byte lowBit = (byte)((firstByte >> (7 - x)) & 1);
        int palette = (highBit << 1) | lowBit;
        int index = (cursorX * 4 * 8) + (y * GBImage.Columns * 8 * 4) + (x * 4);
        pixels[Math.Min(index + 0, pixels.Length - 1)] = GBImage.Colors[palette].B;
        pixels[Math.Min(index + 1, pixels.Length - 1)] = GBImage.Colors[palette].G;
        pixels[Math.Min(index + 2, pixels.Length - 1)] = GBImage.Colors[palette].R;
        pixels[Math.Min(index + 3, pixels.Length - 1)] = 255;
      }
    }

    writeableBitmap.Lock();
    writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, 4 * GBImage.Columns * 8, 0);
    writeableBitmap.Unlock();

    Stretch = Stretch.None;
    Source = writeableBitmap;

    RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
  }
}
