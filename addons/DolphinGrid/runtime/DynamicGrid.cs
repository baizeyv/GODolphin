using System;
using System.Collections.Generic;

namespace GODolphin.Grid;

public class DynamicGrid<T>
{
    private Dictionary<Tuple<int, int>, T> _grid = new();

    public void ForEach(Action<int, int, T> each)
    {
        foreach (var kvp in _grid)
        {
            each(kvp.Key.Item1, kvp.Key.Item2, kvp.Value);
        }
    }

    public void ForEach(Action<T> each)
    {
        foreach (var kvp in _grid)
        {
            each(kvp.Value);
        }
    }

    public void Clear(Action<T> cleanupItem = null)
    {
        if (cleanupItem != null)
        {
            var values = _grid.Values;
            foreach (var value in values)
            {
                cleanupItem(value);
            }
        }
        _grid.Clear();
    }

    public T this[int xIndex, int yIndex]
    {
        get
        {
            var key = new Tuple<int, int>(xIndex, yIndex);
            return _grid.TryGetValue(key, out var value) ? value : default;
        }
        set
        {
            var key = new Tuple<int, int>(xIndex, yIndex);
            _grid[key] = value;
        }
    }
}
