using System.Windows;

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
    // 디자인 타임 오류 예외처리
    if (Application.Current is not App)
      return Activator.CreateInstance<T>();

    return (T)_instances[typeof(T)]!;
  }

  private Injector() { }
}
