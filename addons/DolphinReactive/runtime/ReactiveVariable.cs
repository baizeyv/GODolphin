using System;
using System.Collections.Generic;
using System.Threading;
using Godot;

namespace GODolphin.Reactive
{
    public abstract class ReadOnlyReactiveVariable<T> : Observable<T>, IDisposable
    {
        public abstract T CurrentValue { get; }

        protected virtual void OnValueChanged(T value) { }

        protected virtual void OnReceiveError(Exception exception) { }

        public ReadOnlyReactiveVariable<T> ToReadOnlyReactiveProperty() => this;

        public abstract void Dispose();
    }

    public class ReactiveVariable<T> : ReadOnlyReactiveVariable<T>, ISubject<T>
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

        private IEqualityComparer<T> equalityComparer;

        private T currentValue;

        internal ObserverNode<T> root;

        public IEqualityComparer<T> EqualityComparer => equalityComparer;

        public override T CurrentValue => currentValue;

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

        public virtual T Value
        {
            get => this.currentValue;
            set
            {
                OnValueChanging(ref value);
                if (EqualityComparer != null)
                {
                    if (EqualityComparer.Equals(this.currentValue, value))
                    {
                        return;
                    }
                }

                this.currentValue = value;
                OnValueChanged(value);

                OnNextCore(value);
            }
        }

        private bool _subscribeWithInit;

        public ReactiveVariable()
            : this(default!) { }

        public ReactiveVariable(T value)
            : this(value, EqualityComparer<T>.Default) { }

        public ReactiveVariable(T value, bool subscribeWithInit)
            : this(value, EqualityComparer<T>.Default, true, subscribeWithInit) { }

        public ReactiveVariable(T value, IEqualityComparer<T> equalityComparer)
            : this(value, equalityComparer, true, true) { }

        public ReactiveVariable(
            T value,
            IEqualityComparer<T> equalityComparer,
            bool subscribeWithInit
        )
            : this(value, equalityComparer, true, subscribeWithInit) { }

        public ReactiveVariable(
            T value,
            IEqualityComparer<T> equalityComparer,
            bool callOnValueChangeInBaseConstructor,
            bool subscribeWithInit
        )
        {
            _subscribeWithInit = subscribeWithInit;
            this.equalityComparer = equalityComparer;
            if (callOnValueChangeInBaseConstructor)
            {
                OnValueChanging(ref value);
            }
            currentValue = value;
            if (callOnValueChangeInBaseConstructor)
            {
                OnValueChanged(value);
            }
        }

        protected virtual void OnValueChanging(ref T value) { }

        protected ref T GetValueRef() => ref currentValue;

        public virtual void ForceNotify()
        {
            OnNext(Value);
        }

        protected virtual void OnNextCore(T value)
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
                    if (_subscribeWithInit)
                        observer.OnNext(currentValue);
                }
                observer.OnCompleted(completedResult.Value);
                return Disposable.Empty;
            }

            if (_subscribeWithInit)
                observer.OnNext(currentValue);

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

        public override void Dispose()
        {
            Dispose(true);
        }

        public void OnNext(T value)
        {
            OnValueChanging(ref value);
            this.currentValue = value;
            OnValueChanged(value);

            OnNextCore(value);
        }

        public void OnError(Exception error)
        {
            ThrowIfDisposed();
            if (IsCompleted)
                return;

            OnReceiveError(error);

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

            if (result.IsFailure)
            {
                OnReceiveError(result.Exception);
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

            DisposeCore();
        }

        protected virtual void DisposeCore() { }

        void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("");
        }
    }

    sealed class ObserverNode<T> : IDisposable
    {
        public readonly Observer<T> Observer;

        private ReactiveVariable<T> parent;

        public ObserverNode<T> Previous { get; set; }

        public ObserverNode<T> Next { get; set; }

        public ObserverNode(ReactiveVariable<T> parent, Observer<T> observer)
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
}
