using System;
using System.Collections.Generic;

namespace GODolphin.Pool;

public static class ListPool<T>
{
    private static Stack<List<T>> _listStack = new();

    public static List<T> Obtain()
    {
        if (_listStack.Count == 0)
            return new List<T>(8);
        return _listStack.Pop();
    }

    public static void Free(List<T> list)
    {
        if (_listStack.Contains(list))
        {
            throw new InvalidOperationException(
                "重复回收 List, The List is released even though it is in the pool"
            );
        }
        list.Clear();
        _listStack.Push(list);
    }
}

public static class ListPoolExtensions
{
    public static void Free<T>(this List<T> self)
    {
        ListPool<T>.Free(self);
    }
}
