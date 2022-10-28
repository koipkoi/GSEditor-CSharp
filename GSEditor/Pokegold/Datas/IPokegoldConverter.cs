namespace GSEditor.Core.PokegoldCore;

public interface IPokegoldConverter
{
  void Read(Pokegold pokegold);
  void Write(Pokegold pokegold);
}
