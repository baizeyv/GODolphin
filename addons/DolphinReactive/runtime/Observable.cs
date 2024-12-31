using System;
using System.Threading;

namespace GODolphin.Reactive
{
    public abstract class Observable<T>
    {
        public IDisposable Subscribe(Observer<T> observer)
        {
            try
            {
                var subscription = SubscribeCore(observer);
                observer.SourceSubscription.Disposable = subscription;
                return observer; // return observer to make subscription chain
            }
            catch
            {
                observer.Dispose();
                throw;
            }
        }

        protected abstract IDisposable SubscribeCore(Observer<T> observer);
    }

    public abstract class Observer<T> : IDisposable
    {
        internal SingleAssignmentDisposable SourceSubscription;

        int calledOnCompleted;
        int disposed;

        /// <summary>
        /// * 是否已经终结了
        /// </summary>
        public bool IsDisposed => disposed != 0;

        private bool IsCalledOnCompleted => calledOnCompleted != 0;

        /// <summary>
        /// * 在完成时自动终结
        /// </summary>
        protected virtual bool AutoDisposeOnCompleted => true;

        public void OnCompleted(Result result)
        {
            if (Interlocked.Exchange(ref calledOnCompleted, 1) != 0)
                return;
            if (IsDisposed)
                return;
            var disposeOnFinally = AutoDisposeOnCompleted;
            try
            {
                OnCompletedCore(result);
            }
            catch (Exception e)
            {
                disposeOnFinally = true;
                ObservableSystem.GetUnhandledExceptionHandler().Invoke(e);
            }
            finally
            {
                if (disposeOnFinally)
                {
                    Dispose();
                }
            }
        }

        protected abstract void OnCompletedCore(Result result);

        public void OnNext(T value)
        {
            if (IsDisposed || IsCalledOnCompleted)
                return;
            try
            {
                OnNextCore(value);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        protected abstract void OnNextCore(T value);

        public void OnError(Exception error)
        {
            if (IsDisposed || IsCalledOnCompleted)
                return;
            try
            {
                OnErrorCore(error);
            }
            catch (Exception e)
            {
                ObservableSystem.GetUnhandledExceptionHandler().Invoke(e);
            }
        }

        protected abstract void OnErrorCore(Exception error);

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;
            DisposeCore();
            SourceSubscription.Dispose();
        }

        protected virtual void DisposeCore() { }
    }
}
