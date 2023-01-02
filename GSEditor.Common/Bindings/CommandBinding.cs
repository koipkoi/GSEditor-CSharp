using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace GSEditor.Common.Bindings;

public sealed class CommandBindingExtension : MarkupExtension
{
  private static readonly BindingFlags _privateBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase;
  private static readonly BindingFlags _publicBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;

  public string Name { get; set; } = "";

  public CommandBindingExtension(string name) { Name = name; }

  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget targetProvider)
      throw new InvalidOperationException();

    var rootObject = targetProvider.GetType()
      .GetProperties(_privateBindingFlags)
      .Where(p => p.Name.Contains("RootObject"))
      .First()
      .GetValue(targetProvider);
    if (rootObject is not FrameworkElement frameworkElement)
      throw new InvalidOperationException();

    return new DelegateCommand(() =>
    {
      var dataContext = frameworkElement.DataContext;
      if (dataContext != null)
        dataContext.GetType().GetMethod(Name, _publicBindingFlags)?.Invoke(dataContext, null);
    });
  }

  private sealed class DelegateCommand : ICommand
  {
    private readonly Action _action;
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

    public DelegateCommand(Action action)
    {
      _action = action;
    }

    public bool CanExecute(object? _)
    {
      return true;
    }

    public void Execute(object? _)
    {
      _action.Invoke();
    }
  }
}
