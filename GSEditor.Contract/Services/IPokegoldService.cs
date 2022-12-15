using GSEditor.Models.Pokegold;
using System;

namespace GSEditor.Contract.Services;

public interface IPokegoldService : IDisposable
{
  public event EventHandler? RomChanged;
  public event EventHandler? DataChanged;

  public bool IsOpened { get; }
  public bool IsChanged { get; }
  public string FileName { get; }
  public PokegoldData Data { get; }

  public bool Open(string fileName);
  public bool Write();

  public void NotifyDataChanged();

  public void Run(string emulatorPath);
}
