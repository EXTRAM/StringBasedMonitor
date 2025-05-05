using System;

namespace StringBasedMonitor;

public class StringLock : IDisposable
{
    private readonly string _value;

    public StringLock(string value)
    {
        _value = value;
        StringMonitor.Enter(value);
    }

    public void Dispose()
    {
        StringMonitor.Exit(_value);
    }
}