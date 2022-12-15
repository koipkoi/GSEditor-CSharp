using GSEditor.Models.Pokegold;

namespace GSEditor.Services.Pokegold;

public interface IPokegoldConverter
{
  public void Read(PokegoldData data);
  public void Write(PokegoldData data);
}
