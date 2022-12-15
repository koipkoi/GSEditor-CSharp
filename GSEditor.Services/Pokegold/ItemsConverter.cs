using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public sealed class ItemsConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    data.Items.Clear();
    for (var i = 0; i < 256; i++)
    {
      var address = 0x697b + (i * 7);
      var bytes = data.GetBytes(address, 7);
      data.Items.Add(Item.FromBytes(bytes));
    }
  }

  public void Write(PokegoldData data)
  {
    for (var i = 0; i < 256; i++)
    {
      var address = 0x697b + (i * 7);
      var bytes = data.Items[i].ToBytes();
      data.SetBytes(address, bytes);
    }
  }
}
