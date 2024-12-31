using System;

namespace GODolphin.Pool;

public class SimpleObjectPool<T> : Pool<T>
{
    private readonly Action<T> _resetMethod;

    public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0)
    {
        SetFactoryMethod(factoryMethod);
        _resetMethod = resetMethod;

        for (var i = 0; i < initCount; i++)
        {
            _freeObjects.Enqueue(_factory.Create());
        }
    }

    protected override void CustomFree(T obj)
    {
        _resetMethod?.Invoke(obj);
    }
}
