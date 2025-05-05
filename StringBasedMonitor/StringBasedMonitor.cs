using System;
using System.Threading;

namespace StringBasedMonitor;

public static class StringBasedMonitor
{
    private static readonly MonitorStorage MonitorStorage = new();
    private static readonly object StringBasedMonitorLockObject = new();

    public static void Enter(string value, ref bool lockTaken)
    {
        if (lockTaken) throw new ArgumentException(ExceptionMessageConstants.LockTakenShouldBeFalse);

        object lockObject;
        lock (StringBasedMonitorLockObject)
        {
            lockObject = MonitorStorage.AddLock(value);
        }

        Monitor.Enter(lockObject, ref lockTaken);
    }

    public static void Enter(string value)
    {
        var lockTaken = false;
        Enter(value, ref lockTaken);
    }

    public static void Exit(string value)
    {
        object? lockObject;
        lock (StringBasedMonitorLockObject)
        {
            lockObject = MonitorStorage.RemoveLock(value);
        }

        if (lockObject == null) throw new ArgumentNullException(ExceptionMessageConstants.MonitorIsNotExist);

        Monitor.Exit(lockObject);
    }

    public static bool TryEnter(string value, int millisecondsTimeout = Timeout.Infinite)
    {
        object lockObject;
        lock (StringBasedMonitorLockObject)
        {
            lockObject = MonitorStorage.AddLock(value);
        }

        if (Monitor.TryEnter(lockObject, millisecondsTimeout))
        {
            return true;
        }

        lock (StringBasedMonitorLockObject)
        {
            MonitorStorage.RemoveLock(value);
            return false;
        }
    }

    public static bool TryEnter(string value, TimeSpan timeout)
    {
        return TryEnter(value, (int)timeout.TotalMilliseconds);
    }

    public static void TryEnter(string value, ref bool lockTaken)
    {
        if (lockTaken) throw new ArgumentException(ExceptionMessageConstants.LockTakenShouldBeFalse);
        lockTaken = TryEnter(value);
    }

    public static void TryEnter(string value, int millisecondsTimeout, ref bool lockTaken)
    {
        if (lockTaken) throw new ArgumentException(ExceptionMessageConstants.LockTakenShouldBeFalse);
        lockTaken = TryEnter(value, millisecondsTimeout);
    }

    public static void TryEnter(string value, TimeSpan timeout, ref bool lockTaken)
    {
        if (lockTaken) throw new ArgumentException(ExceptionMessageConstants.LockTakenShouldBeFalse);
        lockTaken = TryEnter(value, timeout);
    }

    public static bool IsEntered(string value)
    {
        lock (StringBasedMonitorLockObject)
        {
            var lockObject = MonitorStorage.GetLock(value);
            if (lockObject == null) return false;

            return Monitor.IsEntered(lockObject);
        }
    }


    public static bool Wait(string value, TimeSpan timeout, bool exitContext = false)
    {
        return Wait(value, (int)timeout.TotalMilliseconds, exitContext);
    }

    public static bool Wait(string value, int millisecondsTimeout = Timeout.Infinite, bool exitContext = false)
    {
        object? lockObject;
        lock (StringBasedMonitorLockObject)
        {
            lockObject = MonitorStorage.GetLock(value);
            if (lockObject == null) throw new SynchronizationLockException(ExceptionMessageConstants.MonitorIsNotExist);
        }

        return Monitor.Wait(lockObject, millisecondsTimeout, exitContext);
    }

    public static void Pulse(string value)
    {
        lock (StringBasedMonitorLockObject)
        {
            var lockObject = MonitorStorage.GetLock(value);
            if (lockObject == null) throw new SynchronizationLockException(ExceptionMessageConstants.MonitorIsNotExist);
            Monitor.Pulse(lockObject);
        }
    }

    public static void PulseAll(string value)
    {
        lock (StringBasedMonitorLockObject)
        {
            var lockObject = MonitorStorage.GetLock(value);
            if (lockObject == null) throw new SynchronizationLockException(ExceptionMessageConstants.MonitorIsNotExist);
            Monitor.PulseAll(lockObject);
        }
    }
}