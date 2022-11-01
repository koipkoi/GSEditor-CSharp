using System.Windows;

namespace GSEditor.Core;

public sealed partial class Pokegold
{
    public event EventHandler? RomChanged;
    public event EventHandler? DataChanged;
    public event EventHandler<int>? PokemonChanged;
    public event EventHandler<int>? TypeChanged;
    public event EventHandler<int>? ItemChanged;

    public void RegisterRomChanged(FrameworkElement element, EventHandler e)
    {
        void onUnloaded(object? _, EventArgs __)
        {
            RomChanged -= e;
            element.Unloaded -= onUnloaded;
        }
        element.Loaded += (_, __) =>
        {
            RomChanged += e;
            element.Unloaded += onUnloaded;
        };
    }

    public void NotifyDataChanged(bool changed = true)
    {
        IsChanged = changed;
        DataChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RegisterDataChanged(FrameworkElement element, EventHandler e)
    {
        void onUnloaded(object? _, EventArgs __)
        {
            DataChanged -= e;
            element.Unloaded -= onUnloaded;
        }
        element.Loaded += (_, __) =>
        {
            DataChanged += e;
            element.Unloaded += onUnloaded;
        };
    }

    public void NotifyPokemonChanged(int index)
    {
        PokemonChanged?.Invoke(this, index);
    }

    public void RegisterPokemonChanged(FrameworkElement element, EventHandler<int> e)
    {
        void onUnloaded(object? _, EventArgs __)
        {
            PokemonChanged -= e;
            element.Unloaded -= onUnloaded;
        }
        element.Loaded += (_, __) =>
        {
            PokemonChanged += e;
            element.Unloaded += onUnloaded;
        };
    }

    public void NotifyTypeChanged(int index)
    {
        TypeChanged?.Invoke(this, index);
    }

    public void RegisterTypeChanged(FrameworkElement element, EventHandler<int> e)
    {
        void onUnloaded(object? _, EventArgs __)
        {
            TypeChanged -= e;
            element.Unloaded -= onUnloaded;
        }
        element.Loaded += (_, __) =>
        {
            TypeChanged += e;
            element.Unloaded += onUnloaded;
        };
    }

    public void NotifyItemChanged(int index)
    {
        ItemChanged?.Invoke(this, index);
    }

    public void RegisterItemChanged(FrameworkElement element, EventHandler<int> e)
    {
        void onUnloaded(object? _, EventArgs __)
        {
            ItemChanged -= e;
            element.Unloaded -= onUnloaded;
        }
        element.Loaded += (_, __) =>
        {
            ItemChanged += e;
            element.Unloaded += onUnloaded;
        };
    }
}
