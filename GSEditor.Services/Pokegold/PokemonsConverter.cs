using GSEditor.Common.Extensions;
using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public sealed class PokemonsConverter : IPokegoldConverter
{
  public void Read(PokegoldData data)
  {
    data.Pokemons.Clear();
    for (var i = 0; i < 251; i++)
    {
      var newPokemon = Pokemon.FromBytes(data.GetBytes(0x51bdf + (i * 32), 32));
      data.Pokemons.Add(newPokemon);

      var bank = 0x423ed.ToGBBank();
      var address = data.GetBytes(0x423ed + (i * 2), 2).ToGBAddress(bank);

      var evolutionsBytes = data.GetBytes(address, b => b == 0);
      address += evolutionsBytes.Length + 1;
      newPokemon.Evolutions.AddRange(Evolution.FromBytes(evolutionsBytes));

      var learnMovesBytes = data.GetBytes(address, b => b == 0);
      newPokemon.LearnMoves.AddRange(LearnMove.FromBytes(learnMovesBytes));
    }
  }

  public void Write(PokegoldData data)
  {
    data.FillByte(0, 0x425e3, 6685);

    var addr = 0x425e3;
    for (var i = 0; i < 251; i++)
    {
      data.SetBytes(0x51bdf + (i * 32), data.Pokemons[i].ToBytes());
      data.SetBytes(0x423ed + (i * 2), addr.ToGBPointer());

      foreach (var e in data.Pokemons[i].Evolutions)
      {
        var bytes = e.ToBytes();
        data.SetBytes(addr, bytes);
        addr += bytes.Length;
      }

      data.SetBytes(addr++, 0);

      foreach (var e in data.Pokemons[i].LearnMoves)
      {
        var bytes = e.ToBytes();
        data.SetBytes(addr, bytes);
        addr += bytes.Length;
      }

      data.SetBytes(addr++, 0);
    }
  }
}
