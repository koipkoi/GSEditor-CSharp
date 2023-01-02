using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace GSEditor.Common.Converters;

public sealed class BoolToVisibilityConverter : MarkupExtension, IValueConverter
{
  public Visibility True { get; set; } = Visibility.Visible;
  public Visibility False { get; set; } = Visibility.Collapsed;
  public Visibility DesignerMode { get; set; } = Visibility.Visible;

  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    return this;
  }

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
      return DesignerMode;
    if (value is bool b)
      return b ? True : False;
    return True;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return value;
  }
}
