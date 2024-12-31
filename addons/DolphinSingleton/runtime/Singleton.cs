using System;

namespace GODolphin.Singleton;

public abstract class Singleton<T> : ISingleton, IDisposable
    where T : Singleton<T>
{
    protected static T _instance;

    private static bool _disposed;

    static object _lock = new();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    if (_disposed)
                        return null;
                    _instance = SingletonCreator.CreateSingleton<T>();
                }
            }

            return _instance;
        }
    }

    public virtual void OnSingletonInitialize() { }

    public virtual void Dispose()
    {
        lock (_lock)
        {
            _disposed = true;
        }
        _instance = null;
    }
}
