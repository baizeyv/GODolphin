namespace GODolphin.Reactive;

public static class ObserverExtensions
{
    public static void OnCompleted<T>(this Observer<T> observer)
    {
        observer.OnCompleted(Result.Success);
    }
}
