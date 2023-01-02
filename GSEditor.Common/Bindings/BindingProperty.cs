using System.ComponentModel;

namespace GSEditor.Common.Bindings;

public sealed class BindingProperty<T> : INotifyPropertyChanged
{
  private T? _value;

  public T? Value
  {
    get => _value;
    set
    {
      _value = value;
      NotifyPropertyChanged();
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  public BindingProperty(T? value = default)
  {
    _value = value;
  }

  public void NotifyPropertyChanged()
  {
    PropertyChanged?.Invoke(this, new(nameof(Value)));
  }
}
