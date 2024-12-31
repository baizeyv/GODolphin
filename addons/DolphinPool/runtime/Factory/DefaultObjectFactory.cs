namespace GODolphin.Pool;

public class DefaultObjectFactory<T> : IObjectFactory<T>
    where T : new()
{
    public T Create()
    {
        return new T();
    }
}
