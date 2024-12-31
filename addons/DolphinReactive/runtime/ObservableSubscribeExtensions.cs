using System;

namespace GODolphin.Reactive;

public static class ObservableSubscribeExtensions
{
    public static IDisposable Subscribe<T>(this Observable<T> source, Action<T> onNext)
    {
        return source.Subscribe(
            new AnonymousObserver<T>(
                onNext,
                ObservableSystem.GetUnhandledExceptionHandler(),
                Stubs.HandleResult
            )
        );
    }
}

internal sealed class AnonymousObserver<T> : Observer<T>
{
    private Action<T> onNext;
    private Action<Exception> onError;
    private Action<Result> onCompleted;

    public AnonymousObserver(
        Action<T> onNext,
        Action<Exception> onError,
        Action<Result> onCompleted
    )
    {
        this.onNext = onNext;
        this.onError = onError;
        this.onCompleted = onCompleted;
    }

    protected override void OnCompletedCore(Result result)
    {
        onCompleted?.Invoke(result);
    }

    protected override void OnNextCore(T value)
    {
        onNext?.Invoke(value);
    }

    protected override void OnErrorCore(Exception error)
    {
        onError?.Invoke(error);
    }
}
