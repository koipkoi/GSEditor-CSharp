using System.Collections;
using System.Windows;

namespace GSEditor.UI.Extensions;

public sealed class BooleanVisibilityExtension : ReadonlyConverterBindingExtensionBase<bool, Visibility>
{
  public Visibility True { get; set; } = Visibility.Visible;
  public Visibility False { get; set; } = Visibility.Collapsed;

  public BooleanVisibilityExtension() { }
  public BooleanVisibilityExtension(string name) { Name = name; }

  protected override Visibility GetDefaultValue()
  {
    return Visibility.Visible;
  }

  protected override Visibility GetConvertValue(bool value)
  {
    return value ? True : False;
  }
}

public sealed class CollectionEmptyVilibilityExtension : ReadonlyConverterBindingExtensionBase<ICollection, Visibility>
{
  public Visibility Empty { get; set; } = Visibility.Collapsed;
  public Visibility NotEmpty { get; set; } = Visibility.Visible;

  public CollectionEmptyVilibilityExtension() { }
  public CollectionEmptyVilibilityExtension(string name) { Name = name; }

  protected override Visibility GetDefaultValue()
  {
    return Visibility.Visible;
  }

  protected override Visibility GetConvertValue(ICollection value)
  {
    return value.Count > 0 ? NotEmpty : Empty;
  }
}

public sealed class StringEmptyVilibilityExtension : ReadonlyConverterBindingExtensionBase<string, Visibility>
{
  public Visibility Empty { get; set; } = Visibility.Collapsed;
  public Visibility NotEmpty { get; set; } = Visibility.Visible;
  public bool UsingTrim { get; set; } = false;

  public StringEmptyVilibilityExtension() { }
  public StringEmptyVilibilityExtension(string name) { Name = name; }

  protected override Visibility GetDefaultValue()
  {
    return Visibility.Visible;
  }

  protected override Visibility GetConvertValue(string value)
  {
    return (UsingTrim ? value.Trim() : value) != "" ? NotEmpty : Empty;
  }
}

public sealed class IntEqualVilibilityExtension : ReadonlyConverterBindingExtensionBase<int, Visibility>
{
  public Visibility Equal { get; set; } = Visibility.Visible;
  public Visibility NotEqual { get; set; } = Visibility.Collapsed;
  public int Value { get; set; } = 0;

  public IntEqualVilibilityExtension() { }
  public IntEqualVilibilityExtension(string name) { Name = name; }

  protected override Visibility GetDefaultValue()
  {
    return Visibility.Visible;
  }

  protected override Visibility GetConvertValue(int value)
  {
    return value == Value ? Equal : NotEqual;
  }
}
