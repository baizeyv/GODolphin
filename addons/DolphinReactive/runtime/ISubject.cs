using System;

namespace GODolphin.Reactive;

/// <summary>
/// * 实现这个接口的类需要继承自 Observable<T>
/// * 主题接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISubject<T>
{
    // IDisposable Subscribe(Observer<T> observer);

    void OnNext(T value);

    void OnError(Exception error);

    void OnCompleted(Result complete);
}

internal sealed class SubjectToObserver<T> : Observer<T>
{
    private ISubject<T> _subject;

    internal SubjectToObserver(ISubject<T> subject)
    {
        _subject = subject;
    }

    protected override void OnCompletedCore(Result result)
    {
        _subject.OnCompleted(result);
    }

    protected override void OnNextCore(T value)
    {
        _subject.OnNext(value);
    }

    protected override void OnErrorCore(Exception error)
    {
        _subject.OnError(error);
    }
}

public static class SubjectExtensions
{
    public static Observer<T> AsObserver<T>(this ISubject<T> subject)
    {
        return new SubjectToObserver<T>(subject);
    }

    public static void OnCompleted<T>(this ISubject<T> subject)
    {
        subject.OnCompleted(default);
    }

    public static void OnCompleted<T>(this ISubject<T> subject, Exception exception)
    {
        subject.OnCompleted(Result.Failure(exception));
    }
}
