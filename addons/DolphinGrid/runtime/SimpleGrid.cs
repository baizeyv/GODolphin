using System;
using Godot;

namespace GODolphin.Grid;

public class SimpleGrid<T>
{
    private T[,] _grid;

    private int _colCount;

    private int _rowCount;

    public SimpleGrid(int colCount, int rowCount)
    {
        _colCount = colCount;
        _rowCount = rowCount;
        _grid = new T[_colCount, _rowCount];
    }

    public void Fill(T value)
    {
        for (var x = 0; x < _colCount; x++)
        {
            for (var y = 0; y < _rowCount; y++)
            {
                _grid[x, y] = value;
            }
        }
    }

    public void Fill(Func<int, int, T> onFill)
    {
        if (onFill == null)
            throw new ArgumentNullException("OnFill can not be null.");
        for (var x = 0; x < _colCount; x++)
        {
            for (var y = 0; y < _rowCount; y++)
            {
                _grid[x, y] = onFill(x, y);
            }
        }
    }

    public void Resize(int colCount, int rowCount, Func<int, int, T> onAdd)
    {
        var newGrid = new T[_colCount, _rowCount];
        for (var x = 0; x < _colCount; x++)
        {
            for (var y = 0; y < _rowCount; y++)
            {
                newGrid[x, y] = _grid[x, y];
            }

            for (var y = _rowCount; y < rowCount; y++)
            {
                newGrid[x, y] = onAdd(x, y);
            }
        }

        for (var x = _colCount; x < colCount; x++)
        {
            for (var y = 0; y < rowCount; y++)
            {
                newGrid[x, y] = onAdd(x, y);
            }
        }

        Fill(default(T));
        _colCount = colCount;
        _rowCount = rowCount;
        _grid = newGrid;
    }

    public void ForEach(Action<int, int, T> each)
    {
        for (var x = 0; x < _colCount; x++)
        {
            for (var y = 0; y < _rowCount; y++)
            {
                each(x, y, _grid[x, y]);
            }
        }
    }

    public void ForEach(Action<T> each)
    {
        for (var x = 0; x < _colCount; x++)
        {
            for (var y = 0; y < _rowCount; y++)
            {
                each(_grid[x, y]);
            }
        }
    }

    public void Clear(Action<T> cleanupItem = null)
    {
        for (var x = 0; x < _colCount; x++)
        {
            for (var y = 0; y < _rowCount; y++)
            {
                cleanupItem?.Invoke(_grid[x, y]);
                _grid[x, y] = default;
            }
        }

        _grid = null;
    }

    public T this[int xIndex, int yIndex]
    {
        get
        {
            if (xIndex >= 0 && xIndex < _colCount && yIndex >= 0 && yIndex < _rowCount)
            {
                return _grid[xIndex, yIndex];
            }
            else
            {
                GD.Print($"out of bounds [{xIndex}:{yIndex}] in grid[{_colCount}:{_rowCount}]");
                return default;
            }
        }
        set
        {
            if (xIndex >= 0 && xIndex < _colCount && yIndex >= 0 && yIndex < _rowCount)
            {
                _grid[xIndex, yIndex] = value;
            }
            else
            {
                GD.Print($"out of bounds [{xIndex}:{yIndex}] in grid[{_colCount}:{_rowCount}]");
            }
        }
    }
}
