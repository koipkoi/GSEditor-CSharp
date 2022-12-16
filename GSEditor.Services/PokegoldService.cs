using GSEditor.Common.Utilities;
using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using GSEditor.Services.Pokegold;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GSEditor.Services;

public sealed class PokegoldService : IPokegoldService
{
  private readonly MD5 _md5 = MD5.Create();
  private readonly List<IPokegoldConverter> _converters = new()
  {
    new ColorsConverter(),
    new ImagesConverter(),
    new ItemsConverter(),
    new MovesConverter(),
    new PokedexConverter(),
    new PokemonsConverter(),
    new StringsConverter(),
    new TMHMsConverter(),
    new ChecksumConverter(),
  };

  public event EventHandler? RomChanged;
  public event EventHandler? DataChanged;

  public bool IsOpened { get; private set; } = false;
  public bool IsChanged { get; private set; } = false;
  public string FileName { get; private set; } = "";
  public PokegoldData Data { get; private set; } = new();

  public void Dispose()
  {
    Unsafe.ClearAllEventHandler(this);
    _md5.Dispose();
  }

  public bool Open(string fileName)
  {
    try
    {
      Data.Bytes = File.ReadAllBytes(fileName);
      Data.Corruptions.Clear();

      foreach (var converter in _converters)
        converter.Read(Data);

      IsOpened = true;
      IsChanged = Data.Corruptions.Count > 0;
      FileName = fileName;
      RomChanged?.Invoke(this, EventArgs.Empty);

      return true;
    }
    catch (Exception)
    {
      // ignored
    }

    IsOpened = false;
    IsChanged = false;
    Data.Corruptions.Clear();
    FileName = "";
    RomChanged?.Invoke(this, EventArgs.Empty);

    return false;
  }

  public bool Write()
  {
    try
    {
      foreach (var converter in _converters)
        converter.Write(Data);

      File.WriteAllBytes(FileName, Data.Bytes!);

      IsChanged = false;
      Data.Corruptions.Clear();
      DataChanged?.Invoke(this, EventArgs.Empty);

      return true;
    }
    catch (Exception)
    {
      // ignored
    }
    return false;
  }

  public void NotifyDataChanged()
  {
    IsChanged = true;
    DataChanged?.Invoke(this, EventArgs.Empty);
  }

  public void Run(string emulatorPath)
  {
    var path = Path.GetDirectoryName(FileName)!;
    var withoutExtFilename = Path.GetFileNameWithoutExtension(FileName);
    var saveFilename = Path.Combine(path, $"{withoutExtFilename}.sav");

    var encodedFileName = Convert.ToHexString(_md5.ComputeHash(Encoding.UTF8.GetBytes("test_play_temp"))).ToLower();
    var newFilename = Path.Combine(Platforms.AppDataDir, $"{encodedFileName}.gbc");
    var newSaveFilename = Path.Combine(Platforms.AppDataDir, $"{encodedFileName}.sav");
    File.Copy(FileName, newFilename, true);
    if (File.Exists(saveFilename))
      File.Copy(saveFilename, newSaveFilename, true);

    var process = new Process();
    process.StartInfo.FileName = emulatorPath;
    process.StartInfo.Arguments = $"\"{newFilename}\"";
    process.StartInfo.UseShellExecute = true;
    process.StartInfo.CreateNoWindow = true;
    process.Start();
    process.WaitForExit();

    File.Delete(newFilename);
    File.Delete(newSaveFilename);
  }
}
