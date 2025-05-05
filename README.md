# StringBasedMonitor

StringBasedMonitor is a lightweight .NET library that mirrors the API of `System.Threading.Monitor` but uses string values as lock keys instead of object instances. It enables developers to coordinate thread synchronization using semantic string identifiers, simplifying scenarios where locks naturally map to named resources.

## Features

* **String-based locking**: Acquire and release locks based on string keys, making code more readable and descriptive.
* **Familiar API**: Methods match `Monitor`’s signatures (`Enter`, `Exit`, `Wait`, `Pulse`, `PulseAll`).
* **Flexible timeouts**: Support for infinite or timed waits using `TimeSpan` and integer milliseconds.
* **No external dependencies**: Pure .NET Standard library, compatible with .NET Framework and .NET Core/5+.

## Installation

You can install StringBasedMonitor via NuGet:

```powershell
Install-Package StringBasedMonitor
```

Or via the .NET CLI:

```bash
dotnet add package StringBasedMonitor
```

## Usage

First, include the namespace:

```csharp
using StringBasedMonitor;
```

### Basic Locking

```csharp
// Acquire a lock on the resource named "Order123"
StringMonitor.Enter("Order123");
try
{
    // Protected section for Order123
}
finally
{
    // Release the lock
    StringMonitor.Exit("Order123");
}
```

### Waiting and Signaling

```csharp
// Thread A:
StringMonitor.Enter("QueueProcessor");
try
{
    while (!HasWork)
    {
        // Wait until Pulse or timeout
        StringMonitor.Wait("QueueProcessor");
    }
    ProcessWork();
}
finally
{
    StringMonitor.Exit("QueueProcessor");
}

// Thread B signals work available:
StringMonitor.Enter("QueueProcessor");
try
{
    HasWork = true;
    StringMonitor.Pulse("QueueProcessor");
}
finally
{
    StringMonitor.Exit("QueueProcessor");
}
```

### Timed Wait

```csharp
StringMonitor.Enter("Updater");
try
{
    if (!StringMonitor.Wait("Updater", TimeSpan.FromSeconds(5)))
    {
        // Timeout occurred
    }
}
finally
{
    StringMonitor.Exit("Updater");
}
```
## Best Practices

* Use consistent key naming conventions (e.g., resource IDs or descriptive names).
* Always call `Exit` in a `finally` block to avoid deadlocks.
* Avoid very broad or frequently changing keys to minimize lock contention.
