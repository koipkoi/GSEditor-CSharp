using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Markup;

namespace GSEditor.Common.Bindings;

public sealed class EventBindingExtension : MarkupExtension
{
  private static readonly MethodInfo _handlerMethodInfo = typeof(EventBindingExtension)!.GetMethod("HandlerInternal", new Type[] { typeof(object), typeof(object), typeof(string), typeof(string) })!;

  public string Name { get; set; } = "";
  public string Argument { get; set; } = "";

  public EventBindingExtension() { }
  public EventBindingExtension(string name) { Name = name; }

  public override object? ProvideValue(IServiceProvider serviceProvider)
  {
    if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget targetProvider)
      throw new InvalidOperationException();
    if (targetProvider.TargetObject is not FrameworkElement)
      throw new InvalidOperationException();

    var memberInfo = targetProvider.TargetProperty as MemberInfo;
    if (memberInfo == null)
      throw new InvalidOperationException();

    return CreateHandler(memberInfo, Name);
  }

  private object? CreateHandler(MemberInfo memberInfo, string name)
  {
    Type eventHandlerType = GetEventHandlerType(memberInfo);
    if (eventHandlerType == null)
      return null;

    var handlerInfo = eventHandlerType.GetMethod("Invoke");
    var method = new DynamicMethod("", handlerInfo?.ReturnType, new Type[] { handlerInfo!.GetParameters()[0].ParameterType, handlerInfo!.GetParameters()[1].ParameterType, });
    var ilGenerator = method.GetILGenerator();
    ilGenerator.Emit(OpCodes.Ldarg, 0);
    ilGenerator.Emit(OpCodes.Ldarg, 1);
    ilGenerator.Emit(OpCodes.Ldstr, name);
    if (Argument == null)
    {
      ilGenerator.Emit(OpCodes.Ldnull);
    }
    else
    {
      ilGenerator.Emit(OpCodes.Ldstr, Argument);
    }
    ilGenerator.Emit(OpCodes.Call, _handlerMethodInfo);
    ilGenerator.Emit(OpCodes.Ret);
    return method.CreateDelegate(eventHandlerType);
  }

  private static Type GetEventHandlerType(MemberInfo memberInfo)
  {
    Type? eventHandlerType = null;
    if (memberInfo is EventInfo)
    {
      var info = memberInfo as EventInfo;
      var eventInfo = info;
      eventHandlerType = eventInfo?.EventHandlerType;
    }
    else if (memberInfo is MethodInfo)
    {
      var info = memberInfo as MethodInfo;
      var methodInfo = info;
      ParameterInfo[] pars = methodInfo!.GetParameters();
      eventHandlerType = pars[1].ParameterType;
    }
    return eventHandlerType!;
  }

  public static void HandlerInternal(object sender, object args, string name, string? argument)
  {
    if (sender is FrameworkElement element)
    {
      var dataContext = element.DataContext;
      if (dataContext == null)
        return;

      var methods = dataContext.GetType()
        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .Where(m => m.Name == name);

      foreach (var method in methods)
      {
        var parameters = method.GetParameters();
        if (parameters.Length >= 2)
          continue;

        if (method.Name == name)
        {
          if (parameters.Length == 0)
          {
            _ = method.Invoke(dataContext, null);
            return;
          }

          if (parameters.Length == 1)
          {
            if (parameters[0].ParameterType.IsInstanceOfType(args))
            {
              _ = method.Invoke(dataContext, new object[] { args });
              return;
            }

            if (parameters[0].ParameterType.IsInstanceOfType(sender))
            {
              _ = method.Invoke(dataContext, new object[] { sender });
              return;
            }

            if (!string.IsNullOrEmpty(argument))
            {
              try
              {
                var data = Convert.ChangeType(argument, parameters[0].ParameterType);
                _ = method.Invoke(dataContext, new object[] { data });
                return;
              }
              catch (Exception)
              {
                continue;
              }
            }
          }
        }
      }
    }
  }
}
