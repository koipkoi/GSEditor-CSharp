namespace GSEditor.Core.PokegoldCore;

public sealed class PokemonsConverter : IPokegoldConverter
{
  public void Read(Pokegold pokegold)
  {
    pokegold.Pokemons.Clear();
    for (var i = 0; i < 251; i++)
    {
      var newPokemon = PGPokemon.FromBytes(pokegold.GetBytes(0x51bdf + (i * 32), 32));
      pokegold.Pokemons.Add(newPokemon);

      var bank = 0x423ed.ToGBBank();
      var address = pokegold.GetBytes(0x423ed + (i * 2), 2).ToGBAddress(bank);

      address += pokegold.ReadBytes(address, (_, b) => b == 0, out byte[] evolutionsBytes);
      newPokemon.Evolutions.AddRange(PGEvolution.FromBytes(evolutionsBytes));

      address += pokegold.ReadBytes(address, (_, b) => b == 0, out byte[] learnMovesBytes);
      newPokemon.LearnMoves.AddRange(PGLearnMove.FromBytes(learnMovesBytes));
    }
  }

  public void Write(Pokegold pokegold)
  {
    pokegold.FillBytes(0, 0x425e3, 6685);

    var addr = 0x425e3;
    for (var i = 0; i < 251; i++)
    {
      pokegold.SetBytes(0x51bdf + (i * 32), pokegold.Pokemons[i].ToBytes());
      pokegold.SetBytes(0x423ed + (i * 2), addr.ToGBPointer());

      foreach (var e in pokegold.Pokemons[i].Evolutions)
      {
        var bytes = e.ToBytes();
        pokegold.SetBytes(addr, bytes);
        addr += bytes.Length;
      }

      pokegold.SetBytes(addr++, 0);

      foreach (var e in pokegold.Pokemons[i].LearnMoves)
      {
        var bytes = e.ToBytes();
        pokegold.SetBytes(addr, bytes);
        addr += bytes.Length;
      }

      pokegold.SetBytes(addr++, 0);
    }
  }
}
