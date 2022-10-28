namespace GSEditor.Core.PokegoldCore;

public sealed class PokedexConverter : IPokegoldConverter
{
  public void Read(Pokegold pokegold)
  {
    pokegold.Pokedex.Clear();
    for (var i = 0; i < 251; i++)
    {
      var item = new PGPokedex();
      var address = (i < 128)
        ? pokegold.GetBytes(0x442ff + (i * 2), 2).ToGBAddress(0x68)
        : pokegold.GetBytes(0x443ff + ((i - 128) * 2), 2).ToGBAddress(0x69);

      address += pokegold.ReadBytes(address, (_, b) => b == 0x50, out byte[] secificNameBytes);
      item.SpecificName = secificNameBytes.TextDecode();

      item.Height = pokegold.GetByte(address++);
      item.Weight = pokegold.GetByte(address++) | (pokegold.GetByte(address++) << 8);

      _ = pokegold.ReadBytes(address, (_, b) => b == 0x50, out byte[] descriptionBytes);
      item.Description = descriptionBytes.TextDecode();

      pokegold.Pokedex.Add(item);
    }
  }

  public void Write(Pokegold pokegold)
  {
    var firstAddress = 0x1a0000;
    var secondAddress = 0x1a4000;

    pokegold.FillBytes(0, 0x1a0000, 0x8000);

    for (var i = 0; i < 251; i++)
    {
      var address = (i < 128) ? firstAddress : secondAddress;
      var pointerAddr = (i < 128) ? 0x442ff + (i * 2) : 0x443ff + ((i - 128) * 2);

      pokegold.SetBytes(pointerAddr, address.ToGBPointer());

      var specificNameBytes = pokegold.Pokedex[i].SpecificName.TextEncode();
      pokegold.SetBytes(address, specificNameBytes);
      address += specificNameBytes.Length;
      pokegold.SetBytes(address++, 0x50);

      pokegold.SetBytes(address++, pokegold.Pokedex[i].Height);

      pokegold.SetBytes(address++, (byte)((pokegold.Pokedex[i].Weight & 0x00ff) >> 0));
      pokegold.SetBytes(address++, (byte)((pokegold.Pokedex[i].Weight & 0xff00) >> 8));

      var descriptionBytes = pokegold.Pokedex[i].Description.TextEncode();
      pokegold.SetBytes(address, descriptionBytes);
      address += descriptionBytes.Length;
      pokegold.SetBytes(address++, 0x50);

      if (i < 128)
        firstAddress = address;
      else
        secondAddress = address;
    }
  }
}
