using System.Collections.Generic;

namespace StringBasedMonitor;

internal class MonitorStorage
{
    private readonly Dictionary<string, MonitorObjectAndCounter> _monitors = new();

    public object AddLock(string value)
    {
        if (_monitors.TryGetValue(value, out var existingLock))
        {
            existingLock.Counter++;
            return existingLock.MonitorObject;
        }

        var newLock = new MonitorObjectAndCounter();
        _monitors.Add(value, newLock);
        return newLock.MonitorObject;
    }

    public object? GetLock(string value)
    {
        return _monitors.TryGetValue(value, out var existingLock) ? existingLock.MonitorObject : null;
    }

    public object? RemoveLock(string value)
    {
        if (_monitors.TryGetValue(value, out var existingLock))
        {
            existingLock.Counter--;
            if (existingLock.Counter <= 0) _monitors.Remove(value);
            return existingLock.MonitorObject;
        }

        return null;
    }
}