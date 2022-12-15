using GSEditor.Common.Extensions;
using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public sealed class PokedexConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    data.Pokedex.Clear();
    for (var i = 0; i < 251; i++)
    {
      var item = new Pokedex();
      var address = (i < 128)
        ? data.GetBytes(0x442ff + (i * 2), 2).ToGBAddress(0x68)
        : data.GetBytes(0x443ff + ((i - 128) * 2), 2).ToGBAddress(0x69);

      var secificNameBytes = data.GetBytes(address, b => b == 0x50);
      address += secificNameBytes.Length + 1;
      item.SpecificName = secificNameBytes.TextDecode();

      item.Height = data.GetByte(address++);
      item.Weight = data.GetByte(address++) | (data.GetByte(address++) << 8);

      var descriptionBytes = data.GetBytes(address, b => b == 0x50);
      item.Description = descriptionBytes.TextDecode();

      data.Pokedex.Add(item);
    }
  }

  public void Write(PokegoldData data)
  {
    var firstAddress = 0x1a0000;
    var secondAddress = 0x1a4000;

    data.FillByte(0, 0x1a0000, 0x8000);

    for (var i = 0; i < 251; i++)
    {
      var address = (i < 128) ? firstAddress : secondAddress;
      var pointerAddr = (i < 128) ? 0x442ff + (i * 2) : 0x443ff + ((i - 128) * 2);

      data.SetBytes(pointerAddr, address.ToGBPointer());

      var specificNameBytes = data.Pokedex[i].SpecificName.TextEncode();
      data.SetBytes(address, specificNameBytes);
      address += specificNameBytes.Length;
      data.SetBytes(address++, 0x50);

      data.SetBytes(address++, data.Pokedex[i].Height);

      data.SetBytes(address++, (byte)((data.Pokedex[i].Weight & 0x00ff) >> 0));
      data.SetBytes(address++, (byte)((data.Pokedex[i].Weight & 0xff00) >> 8));

      var descriptionBytes = data.Pokedex[i].Description.TextEncode();
      data.SetBytes(address, descriptionBytes);
      address += descriptionBytes.Length;
      data.SetBytes(address++, 0x50);

      if (i < 128)
        firstAddress = address;
      else
        secondAddress = address;
    }
  }
}
