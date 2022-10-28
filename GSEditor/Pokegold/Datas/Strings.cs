namespace GSEditor.Core.PokegoldCore;

public sealed class Strings
{
  public List<string> ItemNames { get; } = new();
  public List<string> TrainerClassNames { get; } = new();
  public List<string> PokemonNames { get; } = new();
  public List<string> MoveNames { get; } = new();

  public List<string> ItemDescriptions { get; } = new();
  public List<string> MoveDescriptions { get; } = new();

  public List<string> TypeNames { get; } = new();
}

public sealed class StringsConverter : IPokegoldConverter
{
  public void Read(Pokegold pokegold)
  {
    pokegold.Strings.ItemNames.Clear();
    var itemNameAddr = pokegold.GetBytes(0x35cc, 3).ToGBAddress();
    for (var i = 0; i < 256; i++)
    {
      itemNameAddr += pokegold.ReadBytes(itemNameAddr, (_, b) => b == 0x50, out byte[] bytes);
      pokegold.Strings.ItemNames.Add(bytes.TextDecode());
    }

    pokegold.Strings.TrainerClassNames.Clear();
    var trainerClassNameAddr = pokegold.GetBytes(0x35d5, 3).ToGBAddress();
    for (var i = 0; i < 67; i++)
    {
      trainerClassNameAddr += pokegold.ReadBytes(trainerClassNameAddr, (_, b) => b == 0x50, out byte[] bytes);
      pokegold.Strings.TrainerClassNames.Add(bytes.TextDecode());
    }

    pokegold.Strings.PokemonNames.Clear();
    for (var i = 0; i < 256; i++)
    {
      var addr = pokegold.GetBytes(0x35c3, 3).ToGBAddress() + (i * 10);
      _ = pokegold.ReadBytes(addr, (i, b) => i == 10 || b == 0x50, out byte[] bytes);
      pokegold.Strings.PokemonNames.Add(bytes.TextDecode());
    }

    pokegold.Strings.MoveNames.Clear();
    var moveNameAddr = pokegold.GetBytes(0x35c6, 3).ToGBAddress();
    for (var i = 0; i < 251; i++)
    {
      moveNameAddr += pokegold.ReadBytes(moveNameAddr, (_, b) => b == 0x50, out byte[] bytes);
      pokegold.Strings.MoveNames.Add(bytes.TextDecode());
    }

    pokegold.Strings.ItemDescriptions.Clear();
    for (var i = 0; i < 256; i++)
    {
      var bank = 0x1b8000.ToGBBank();
      var address = pokegold.GetBytes(0x1b8000 + (i * 2), 2).ToGBAddress(bank);
      _ = pokegold.ReadBytes(address, (_, b) => b == 0x50, out byte[] bytes);
      pokegold.Strings.ItemDescriptions.Add(bytes.TextDecode());
    }

    pokegold.Strings.MoveDescriptions.Clear();
    for (var i = 0; i < 256; i++)
    {
      var bank = 0x1b4000.ToGBBank();
      var address = pokegold.GetBytes(0x1b4000 + (i * 2), 2).ToGBAddress(bank);
      _ = pokegold.ReadBytes(address, (_, b) => b == 0x50, out byte[] bytes);
      pokegold.Strings.MoveDescriptions.Add(bytes.TextDecode());
    }

    pokegold.Strings.TypeNames.Clear();
    for (var i = 0; i < 28; i++)
    {
      var bank = 0x50a57.ToGBBank();
      var address = pokegold.GetBytes(0x50a57 + (i * 2), 2).ToGBAddress(bank);
      _ = pokegold.ReadBytes(address, (_, b) => b == 0x50, out byte[] bytes);
      pokegold.Strings.TypeNames.Add(bytes.TextDecode());
    }
  }

  public void Write(Pokegold pokegold)
  {
    pokegold.FillBytes(0, 0x1b0000, 0x4000);
    pokegold.FillBytes(0, 0x1b4000, 0x4000);
    pokegold.FillBytes(0, 0x1b8000, 0x4000);

    var address = 0x1b0000;

    pokegold.SetBytes(0x35cc, address.ToGBPointerWithBank());
    pokegold.SetBytes(0x515cd, address.ToGBPointer());
    pokegold.SetBytes(0x515d7, address.ToGBPointer());
    for (var i = 0; i < 256; i++)
    {
      var bytes = pokegold.Strings.ItemNames[i].TextEncode();
      pokegold.SetBytes(address, bytes);
      address += bytes.Length;
      pokegold.SetBytes(address++, 0x50);
    }

    pokegold.SetBytes(0x35d5, address.ToGBPointerWithBank());
    for (var i = 0; i < 67; i++)
    {
      var bytes = pokegold.Strings.TrainerClassNames[i].TextEncode();
      pokegold.SetBytes(address, bytes);
      address += bytes.Length;
      pokegold.SetBytes(address++, 0x50);
    }

    pokegold.SetBytes(0x35c3, address.ToGBPointerWithBank());
    pokegold.SetBytes(0x3667, address.ToGBPointer());
    pokegold.SetBytes(0x515bf, address.ToGBPointer());
    for (var i = 0; i < 256; i++)
    {
      var bytes = pokegold.Strings.PokemonNames[i].TextEncode();
      pokegold.SetBytes(address, bytes);
      address += bytes.Length;

      for (var j = 0; j < 10 - bytes.Length; j++)
        pokegold.SetBytes(address++, 0x50);
    }

    pokegold.SetBytes(0x35c6, address.ToGBPointerWithBank());
    for (var i = 0; i < 251; i++)
    {
      var bytes = pokegold.Strings.MoveNames[i].TextEncode();
      pokegold.SetBytes(address, bytes);
      address += bytes.Length;
      pokegold.SetBytes(address++, 0x50);
    }

    var itemDescriptionAddrDict = new Dictionary<string, int>();
    var itemDescriptionAddr = 0x1b8000 + (256 * 2);
    for (var i = 0; i < 256; i++)
    {
      var text = pokegold.Strings.ItemDescriptions[i];
      if (itemDescriptionAddrDict.ContainsKey(text))
      {
        pokegold.SetBytes(0x1b8000 + (i * 2), itemDescriptionAddrDict[text].ToGBPointer());
      }
      else
      {
        pokegold.SetBytes(0x1b8000 + (i * 2), itemDescriptionAddr.ToGBPointer());
        itemDescriptionAddrDict[text] = itemDescriptionAddr;

        var bytes = text.TextEncode();
        pokegold.SetBytes(itemDescriptionAddr, bytes);
        itemDescriptionAddr += bytes.Length;
        pokegold.SetBytes(itemDescriptionAddr++, 0x50);
      }
    }

    var moveDescriptionAddrDict = new Dictionary<string, int>();
    var moveDescriptionAddr = 0x1b4000 + (256 * 2);
    for (var i = 0; i < 256; i++)
    {
      var text = pokegold.Strings.MoveDescriptions[i];
      if (moveDescriptionAddrDict.ContainsKey(text))
      {
        pokegold.SetBytes(0x1b4000 + (i * 2), moveDescriptionAddrDict[text].ToGBPointer());
      }
      else
      {
        pokegold.SetBytes(0x1b4000 + (i * 2), moveDescriptionAddr.ToGBPointer());
        moveDescriptionAddrDict[text] = moveDescriptionAddr;

        var bytes = text.TextEncode();
        pokegold.SetBytes(moveDescriptionAddr, bytes);
        moveDescriptionAddr += bytes.Length;
        pokegold.SetBytes(moveDescriptionAddr++, 0x50);
      }
    }

    // todo 타입명 기록 추가
  }
}
