namespace GSEditor.Core;

public sealed partial class Pokegold
{
  public event EventHandler? RomChanged;
  public event EventHandler<int>? PokemonChanged;
  public event EventHandler<int>? TypeChanged;
  public event EventHandler<int>? ItemChanged;

  public void NotifyDataChanged()
  {
    IsChanged = true;
  }

  public void NotifyPokemonChanged(int index)
  {
    PokemonChanged?.Invoke(this, index);
  }

  public void NotifyTypeChanged(int index)
  {
    TypeChanged?.Invoke(this, index);
  }

  public void NotifyItemChanged(int index)
  {
    ItemChanged?.Invoke(this, index);
  }
}
