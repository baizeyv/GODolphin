using System;
using GODolphin.Reactive;

namespace GODolphin.Event;

public static class SimpleEventExtensions
{
    public static IDisposable Subscribe(
        this StringEvent source,
        string stringEvent,
        Action<Unit> onNext
    )
    {
        return source.Subscribe(
            stringEvent,
            new AnonymousObserver<Unit>(
                onNext,
                ObservableSystem.GetUnhandledExceptionHandler(),
                Stubs.HandleResult
            )
        );
    }

    public static IDisposable Subscribe<T>(
        this StringEvent source,
        string stringEvent,
        Action<T> onNext
    )
    {
        return source.Subscribe(
            stringEvent,
            new AnonymousObserver<T>(
                onNext,
                ObservableSystem.GetUnhandledExceptionHandler(),
                Stubs.HandleResult
            )
        );
    }

    public static IDisposable Subscribe(this IntEvent source, int intEvent, Action<Unit> onNext)
    {
        return source.Subscribe(
            intEvent,
            new AnonymousObserver<Unit>(
                onNext,
                ObservableSystem.GetUnhandledExceptionHandler(),
                Stubs.HandleResult
            )
        );
    }

    public static IDisposable Subscribe<T>(this IntEvent source, int intEvent, Action<T> onNext)
    {
        return source.Subscribe(
            intEvent,
            new AnonymousObserver<T>(
                onNext,
                ObservableSystem.GetUnhandledExceptionHandler(),
                Stubs.HandleResult
            )
        );
    }
}
