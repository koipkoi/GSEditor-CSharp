namespace GSEditor.Core.PokegoldCore;

public sealed class ItemsConverter : IPokegoldConverter
{
  public void Read(Pokegold pokegold)
  {
    pokegold.Items.Clear();
    for (var i = 0; i < 256; i++)
    {
      var address = 0x697b + (i * 7);
      var bytes = pokegold.GetBytes(address, 7);
      pokegold.Items.Add(PGItem.FromBytes(bytes));
    }
  }

  public void Write(Pokegold pokegold)
  {
    for (var i = 0; i < 256; i++)
    {
      var address = 0x697b + (i * 7);
      var bytes = pokegold.Items[i].ToBytes();
      pokegold.SetBytes(address, bytes);
    }
  }
}
