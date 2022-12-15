using System;
using System.Collections.Generic;

namespace GSEditor.Common.Utilities;

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
        try
        {
          action();
        }
        catch { }
        _lock.Remove(lockKey);
      }
    }
  }
}
