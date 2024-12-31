using System;
using System.Threading;
using GODolphin.Reactive;

namespace GODolphin.Event;

public interface ISimpleEvent : IDisposable { }

public class SimpleEvent<T> : Observable<T>, ISubject<T>, ISimpleEvent
{
    /// <summary>
    /// * 未完成标志位
    /// </summary>
    private const byte NotCompleted = 0;

    /// <summary>
    /// * 成功完成标志位
    /// </summary>
    private const byte CompletedSuccess = 1;

    /// <summary>
    /// * 失败完成标志位
    /// </summary>
    private const byte CompletedFailure = 2;

    /// <summary>
    /// * 已终结标志位
    /// </summary>
    private const byte Disposed = 3;

    private byte completeState;

    private Exception error;

    internal ObserverNode<T> root;

    private T value;

    /// <summary>
    /// * 是否有订阅的观察者
    /// </summary>
    public bool HasObservers => root != null;

    /// <summary>
    /// * 是否完成了
    /// </summary>
    public bool IsCompleted =>
        completeState == CompletedSuccess || completeState == CompletedFailure;

    /// <summary>
    /// * 是否终结了
    /// </summary>
    public bool IsDisposed => completeState == Disposed;

    /// <summary>
    /// * 是否完成了或终结了
    /// </summary>
    public bool IsCompletedOrDisposed => IsCompleted || IsDisposed;

    public SimpleEvent()
        : this(default) { }

    public SimpleEvent(T value)
    {
        this.value = value;
    }

    public void Trigger(T val)
    {
        OnNext(val);
    }

    public void Trigger()
    {
        OnNext(value);
    }

    protected override IDisposable SubscribeCore(Observer<T> observer)
    {
        Result? completedResult;
        lock (this)
        {
            ThrowIfDisposed();
            if (IsCompleted)
            {
                completedResult = (error == null) ? Result.Success : Result.Failure(error);
            }
            else
            {
                completedResult = null;
            }
        }

        if (completedResult != null)
        {
            if (completedResult.Value.IsSuccess)
            {
                observer.OnNext(value);
            }
            observer.OnCompleted(completedResult.Value);
            return Disposable.Empty;
        }

        // ! Event 在订阅的时候不应该执行
        // observer.OnNext(value);

        lock (this)
        {
            ThrowIfDisposed();
            if (IsCompleted)
            {
                completedResult = (error == null) ? Result.Success : Result.Failure(error);
                if (completedResult != null)
                {
                    observer.OnCompleted(completedResult.Value);
                }
                return Disposable.Empty;
            }

            var subscription = new ObserverNode<T>(this, observer);
            return subscription;
        }
    }

    public void OnNext(T value)
    {
        ThrowIfDisposed();
        if (IsCompleted)
            return;

        var node = root;
        var last = node?.Previous;
        while (node != null)
        {
            node.Observer.OnNext(value);
            if (node == last)
                return;
            node = node.Next;
        }
    }

    public void OnError(Exception error)
    {
        ThrowIfDisposed();
        if (IsCompleted)
            return;

        var node = Volatile.Read(ref root);
        var last = node?.Previous;
        while (node != null)
        {
            node.Observer.OnError(error);
            if (node == last)
                return;
            node = node.Next;
        }
    }

    public void OnCompleted(Result result)
    {
        ThrowIfDisposed();
        if (IsCompleted)
            return;

        ObserverNode<T> node = null;
        lock (this)
        {
            if (completeState == NotCompleted)
            {
                completeState = result.IsSuccess ? CompletedSuccess : CompletedFailure;
                error = result.Exception;
                node = Volatile.Read(ref root);
                Volatile.Write(ref root, null);
            }
            else
            {
                ThrowIfDisposed();
                return;
            }
        }

        var last = node?.Previous;
        while (node != null)
        {
            node.Observer.OnCompleted(result);
            if (node == last)
                return;
            node = node.Next;
        }
    }

    public void Dispose(bool callOnCompleted)
    {
        ObserverNode<T> node = null;
        lock (this)
        {
            if (completeState == Disposed)
                return;

            if (callOnCompleted && !IsCompleted)
            {
                node = Volatile.Read(ref root);
            }
            Volatile.Write(ref root, null);
            completeState = Disposed;
        }

        while (node != null)
        {
            node.Observer.OnCompleted();
            node = node.Next;
        }
    }

    void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException("");
    }

    public void Dispose()
    {
        Dispose(true);
    }
}

public sealed class SimpleEvent : SimpleEvent<Unit> { }

sealed class ObserverNode<T> : IDisposable
{
    public readonly Observer<T> Observer;

    private SimpleEvent<T> parent;

    public ObserverNode<T> Previous { get; set; }

    public ObserverNode<T> Next { get; set; }

    public ObserverNode(SimpleEvent<T> parent, Observer<T> observer)
    {
        this.parent = parent;
        this.Observer = observer;

        if (parent.root == null)
        {
            Volatile.Write(ref parent.root, this);
        }
        else
        {
            var lastNode = parent.root.Previous ?? parent.root;

            lastNode.Next = this;
            this.Previous = lastNode;
            parent.root.Previous = this;
        }
    }

    public void Dispose()
    {
        var p = Interlocked.Exchange(ref parent, null);
        if (p == null)
            return;

        // keep this.Next for dispose on iterating
        // Remove node(self) from list(ReactiveProperty)
        lock (p)
        {
            if (p.IsCompletedOrDisposed)
                return;

            if (this == p.root)
            {
                if (this.Previous == null || this.Next == null)
                {
                    // case of single list
                    p.root = null;
                }
                else
                {
                    // otherwise, root is next node.
                    var root = this.Next;

                    // single list.
                    if (root.Next == null)
                    {
                        root.Previous = null;
                    }
                    else
                    {
                        root.Previous = this.Previous; // as last.
                    }

                    p.root = root;
                }
            }
            else
            {
                // node is not root, previous must exists
                this.Previous!.Next = this.Next;
                if (this.Next != null)
                {
                    this.Next.Previous = this.Previous;
                }
                else
                {
                    // next does not exists, previous is last node so modify root
                    p.root!.Previous = this.Previous;
                }
            }
        }
    }
}
