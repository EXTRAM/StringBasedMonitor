using System;

namespace StringBasedMonitor;

public class StringBasedLock : IDisposable
{
    private readonly string _value;

    public StringBasedLock(string value)
    {
        _value = value;
        StringBasedMonitor.Enter(value);
    }

    public void Dispose()
    {
        StringBasedMonitor.Exit(_value);
    }
}