namespace GODolphin.Singleton;

public static class SingletonProperty<T>
    where T : class, ISingleton
{
    private static T _instance;

    private static readonly object _lock = new();

    private static bool _disposed;

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

    public static void Dispose()
    {
        lock (_lock)
        {
            _disposed = true;
        }

        _instance = null;
    }
}
