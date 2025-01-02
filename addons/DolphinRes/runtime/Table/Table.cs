using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GODolphin.Pool;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace GODolphin.Res;
public abstract class Table<T> : IEnumerable<T>, IDisposable
{
    public void Add(T item)
    {
        OnAdd(item);
    }

    public void Remove(T item)
    {
        OnRemove(item);
    }

    public void Clear()
    {
        OnClear();
    }

    protected abstract void OnAdd(T item);

    protected abstract void OnRemove(T item);

    protected abstract void OnClear();

    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        OnDispose();
    }

    protected abstract void OnDispose();
}

public class TableIndex<TKey, TValue> : IDisposable
{
    private System.Collections.Generic.Dictionary<TKey, List<TValue>> _index = DictionaryPool<TKey, List<TValue>>.Obtain();

    private Func<TValue, TKey> _getKeyByValue = null;

    public TableIndex(Func<TValue, TKey> keyGetter)
    {
        _getKeyByValue = keyGetter;
    }

    public IDictionary<TKey, List<TValue>> Dictionary => _index;

    public void Add(TValue value)
    {
        var key = _getKeyByValue(value);
        if (_index.ContainsKey(key))
        {
            _index[key].Add(value);
        }
        else
        {
            var list = ListPool<TValue>.Obtain();
            list.Add(value);
            _index.Add(key, list);
        }
    }

    public void Remove(TValue value)
    {
        var key = _getKeyByValue(value);
        _index[key].Remove(value);
    }

    public IEnumerable<TValue> Get(TKey key)
    {
        if (_index.TryGetValue(key, out var retList))
        {
            return retList;
        }
        return Enumerable.Empty<TValue>();
    }

    public void Clear()
    {
        foreach (var value in _index.Values)
        {
            value.Clear();
        }
        _index.Clear();
    }

    public void Dispose()
    {
        foreach (var value in _index.Values)
        {
            value.Free();
        }
        _index.Free();
        _index = null;
    }
}
