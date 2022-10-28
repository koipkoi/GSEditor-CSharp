namespace GSEditor.Core;

/// <summary>
/// 간이 의존성 주입기
/// </summary>
public sealed class Injector
{
  private static readonly Dictionary<Type, object> _instances = new();

  public static T Register<T>(T instance)
  {
    _instances[instance!.GetType()] = instance;
    return instance;
  }

  public static T Get<T>()
  {
    return (T)_instances[typeof(T)]!;
  }

  private Injector() { }
}
