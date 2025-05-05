namespace StringBasedMonitor;

internal class MonitorObjectAndCounter
{
    public readonly object MonitorObject = new();
    public int Counter = 1;
}