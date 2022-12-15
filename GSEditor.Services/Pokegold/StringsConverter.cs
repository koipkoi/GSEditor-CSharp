using GSEditor.Common.Extensions;
using GSEditor.Models.Pokegold;
using System.Collections.Generic;

namespace GSEditor.Services.Pokegold;

public sealed class StringsConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    data.Strings.ItemNames.Clear();
    var itemNameAddr = data.GetBytes(0x35cc, 3).ToGBAddress();
    for (var i = 0; i < 256; i++)
    {
      var bytes = data.GetBytes(itemNameAddr, b => b == 0x50);
      itemNameAddr += bytes.Length + 1;
      data.Strings.ItemNames.Add(bytes.TextDecode());
    }

    data.Strings.TrainerClassNames.Clear();
    var trainerClassNameAddr = data.GetBytes(0x35d5, 3).ToGBAddress();
    for (var i = 0; i < 67; i++)
    {
      var bytes = data.GetBytes(trainerClassNameAddr, b => b == 0x50);
      trainerClassNameAddr += bytes.Length + 1;
      data.Strings.TrainerClassNames.Add(bytes.TextDecode());
    }

    data.Strings.PokemonNames.Clear();
    for (var i = 0; i < 256; i++)
    {
      var addr = data.GetBytes(0x35c3, 3).ToGBAddress() + (i * 10);
      var cnt = 0;
      var bytes = data.GetBytes(addr, b =>
      {
        cnt++;
        return cnt == 11 || b == 0x50;
      });
      data.Strings.PokemonNames.Add(bytes.TextDecode());
    }

    data.Strings.MoveNames.Clear();
    var moveNameAddr = data.GetBytes(0x35c6, 3).ToGBAddress();
    for (var i = 0; i < 251; i++)
    {
      var bytes = data.GetBytes(moveNameAddr, b => b == 0x50);
      moveNameAddr += bytes.Length + 1;
      data.Strings.MoveNames.Add(bytes.TextDecode());
    }

    data.Strings.ItemDescriptions.Clear();
    for (var i = 0; i < 256; i++)
    {
      var bank = 0x1b8000.ToGBBank();
      var address = data.GetBytes(0x1b8000 + (i * 2), 2).ToGBAddress(bank);
      var bytes = data.GetBytes(address, b => b == 0x50);
      data.Strings.ItemDescriptions.Add(bytes.TextDecode());
    }

    data.Strings.MoveDescriptions.Clear();
    for (var i = 0; i < 256; i++)
    {
      var bank = 0x1b4000.ToGBBank();
      var address = data.GetBytes(0x1b4000 + (i * 2), 2).ToGBAddress(bank);
      var bytes = data.GetBytes(address, b => b == 0x50);
      data.Strings.MoveDescriptions.Add(bytes.TextDecode());
    }

    data.Strings.MoveTypeNames.Clear();
    for (var i = 0; i < 28; i++)
    {
      var bank = 0x50a57.ToGBBank();
      var address = data.GetBytes(0x50a57 + (i * 2), 2).ToGBAddress(bank);
      var bytes = data.GetBytes(address, b => b == 0x50);
      data.Strings.MoveTypeNames.Add(bytes.TextDecode());
    }
  }

#pragma warning disable IDE0230
  public void Write(PokegoldData data)
  {
    data.FillByte(0, 0x1b0000, 0x4000);
    data.FillByte(0, 0x1b4000, 0x4000);
    data.FillByte(0, 0x1b8000, 0x4000);

    var address = 0x1b0000;

    data.SetBytes(0x35cc, address.ToGBPointerWithBank());
    data.SetBytes(0x515cd, address.ToGBPointer());
    data.SetBytes(0x515d7, address.ToGBPointer());
    for (var i = 0; i < 256; i++)
    {
      var bytes = data.Strings.ItemNames[i].TextEncode();
      data.SetBytes(address, bytes);
      address += bytes.Length;
      data.SetBytes(address++, 0x50);
    }

    data.SetBytes(0x35d5, address.ToGBPointerWithBank());
    for (var i = 0; i < 67; i++)
    {
      var bytes = data.Strings.TrainerClassNames[i].TextEncode();
      data.SetBytes(address, bytes);
      address += bytes.Length;
      data.SetBytes(address++, 0x50);
    }

    data.SetBytes(0x35c3, address.ToGBPointerWithBank());
    data.SetBytes(0x3667, address.ToGBPointer());
    data.SetBytes(0x515bf, address.ToGBPointer());
    for (var i = 0; i < 256; i++)
    {
      var bytes = data.Strings.PokemonNames[i].TextEncode();
      data.SetBytes(address, bytes);
      address += bytes.Length;

      for (var j = 0; j < 10 - bytes.Length; j++)
        data.SetBytes(address++, 0x50);
    }

    data.SetBytes(0x35c6, address.ToGBPointerWithBank());
    for (var i = 0; i < 251; i++)
    {
      var bytes = data.Strings.MoveNames[i].TextEncode();
      data.SetBytes(address, bytes);
      address += bytes.Length;
      data.SetBytes(address++, 0x50);
    }

    var itemDescriptionAddrDict = new Dictionary<string, int>();
    var itemDescriptionAddr = 0x1b8000 + (256 * 2);
    for (var i = 0; i < 256; i++)
    {
      var text = data.Strings.ItemDescriptions[i];
      if (itemDescriptionAddrDict.TryGetValue(text.ToString(), out int value))
      {
        data.SetBytes(0x1b8000 + (i * 2), value.ToGBPointer());
      }
      else
      {
        data.SetBytes(0x1b8000 + (i * 2), itemDescriptionAddr.ToGBPointer());
        itemDescriptionAddrDict[text.ToString()] = itemDescriptionAddr;

        var bytes = text.TextEncode();
        data.SetBytes(itemDescriptionAddr, bytes);
        itemDescriptionAddr += bytes.Length;
        data.SetBytes(itemDescriptionAddr++, 0x50);
      }
    }

    var moveDescriptionAddrDict = new Dictionary<string, int>();
    var moveDescriptionAddr = 0x1b4000 + (256 * 2);
    for (var i = 0; i < 256; i++)
    {
      var text = data.Strings.MoveDescriptions[i];
      if (moveDescriptionAddrDict.TryGetValue(text.ToString(), out int value))
      {
        data.SetBytes(0x1b4000 + (i * 2), value.ToGBPointer());
      }
      else
      {
        data.SetBytes(0x1b4000 + (i * 2), moveDescriptionAddr.ToGBPointer());
        moveDescriptionAddrDict[text.ToString()] = moveDescriptionAddr;

        var bytes = text.TextEncode();
        data.SetBytes(moveDescriptionAddr, bytes);
        moveDescriptionAddr += bytes.Length;
        data.SetBytes(moveDescriptionAddr++, 0x50);
      }
    }

    // todo 타입명 기록 추가
  }
}

#pragma warning restore IDE0230
