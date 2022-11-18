namespace GSEditor.Utilities;

public static class LockAction
{
  private static readonly HashSet<object> _lock = new();

  public static void RunSafe(this object lockKey, Action action)
  {
    lock (lockKey)
    {
      if (!_lock.Contains(lockKey))
      {
        _lock.Add(lockKey);
        action();
        _lock.Remove(lockKey);
      }
    }
  }
}
