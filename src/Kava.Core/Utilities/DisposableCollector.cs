using System;
using System.Collections.Generic;
using Kava.Core.Extensions;

namespace Kava.Core.Utilities;

public sealed class DisposableCollector : IDisposable
{
    private readonly object _lock = new();
    private readonly List<IDisposable> _items = [];

    public void Add(IDisposable item)
    {
        lock (_lock)
        {
            _items.Add(item);
        }
    }

    public void AddRange(IEnumerable<IDisposable> items)
    {
        lock (_lock)
        {
            _items.AddRange(items);
        }
    }

    public void AddRange(params IDisposable[] items)
    {
        lock (_lock)
        {
            _items.AddRange(items);
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _items.DisposeAll();
            _items.Clear();
        }
    }
}
