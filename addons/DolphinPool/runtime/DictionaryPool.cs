using System.Collections.Generic;

namespace GODolphin.Pool;

public static class DictionaryPool<TKey, TValue>
{
    private static Stack<Dictionary<TKey, TValue>> _dictionaryStack = new();

    public static Dictionary<TKey, TValue> Obtain()
    {
        if (_dictionaryStack.Count == 0)
        {
            return new Dictionary<TKey, TValue>(8);
        }
        return _dictionaryStack.Pop();
    }

    public static void Free(Dictionary<TKey, TValue> dictionary)
    {
        dictionary.Clear();
        _dictionaryStack.Push(dictionary);
    }
}

public static class DictionaryPoolExtensions
{
    public static void Free<TKey, TValue>(this Dictionary<TKey, TValue> self)
    {
        DictionaryPool<TKey, TValue>.Free(self);
    }
}
