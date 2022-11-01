using System.ComponentModel;
using System.Reflection;
using System.Windows;
using XAMLMarkupExtensions.Base;

namespace GSEditor.UI.Extensions;

public abstract class ReadonlyConverterBindingExtensionBase<V, T> : NestedMarkupExtension
{
  public string Name { get; set; } = "";

  public override object FormatOutput(TargetInfo endPoint, TargetInfo info)
  {
    if (endPoint.TargetObject is FrameworkElement frameworkElement)
    {
      PropertyInfo? targetProperty = frameworkElement.GetType().GetProperty(endPoint.TargetProperty.ToString()!);
      object? currentDataContext = null;
      PropertyInfo? property = null;
      INotifyPropertyChanged? currentNotifyPropertyChanged = null;
      IBindingList? currentBindingList = null;

      void applyValue()
      {
        if (currentDataContext != null)
        {
          var value = (V)property?.GetValue(currentDataContext)!;
          if (value != null && targetProperty != null)
          {
            var converted = GetConvertValue(value);
            targetProperty.SetValue(frameworkElement, converted);

            if (currentBindingList != null)
              currentBindingList.ListChanged -= bindingListChanged;
            if (value is IBindingList bindingList)
            {
              currentBindingList = bindingList;
              bindingList.ListChanged += bindingListChanged;
            }
          }
        }
      }

      void bindingListChanged(object? _, ListChangedEventArgs __) => propertyChanged(null, new PropertyChangedEventArgs(""));
      void propertyChanged(object? _, PropertyChangedEventArgs __) => applyValue();

      void dataContextChanged(object? _, DependencyPropertyChangedEventArgs __)
      {
        currentDataContext = frameworkElement.DataContext;
        if (currentDataContext != null)
        {
          property = currentDataContext.GetType().GetProperty(Name);
          if (property != null)
          {
            if (currentNotifyPropertyChanged != null)
              currentNotifyPropertyChanged.PropertyChanged -= propertyChanged;

            if (currentDataContext is INotifyPropertyChanged notifyPropertyChanged)
            {
              notifyPropertyChanged.PropertyChanged += propertyChanged;
              currentNotifyPropertyChanged = notifyPropertyChanged;
            }

            applyValue();
          }
        }
      }

      frameworkElement.Loaded += (_, __) =>
      {
        frameworkElement.DataContextChanged += dataContextChanged;
        dataContextChanged(null, new DependencyPropertyChangedEventArgs());
      };
    }
    return GetDefaultValue()!;
  }

  protected override bool UpdateOnEndpoint(TargetInfo endpoint)
  {
    return true;
  }

  protected abstract T GetDefaultValue();
  protected abstract T GetConvertValue(V value);
}
