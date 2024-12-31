using System;
using System.Threading;

namespace GODolphin.Reactive
{
    public struct SingleAssignmentDisposable
    {
        IDisposable current;

        public bool IsDisposed => Volatile.Read(ref current) == DisposedSentinel.Instance;

        public IDisposable Disposable
        {
            get
            {
                var field = Volatile.Read(ref current);
                if (field == DisposedSentinel.Instance)
                {
                    return GODolphin.Reactive.Disposable.Empty;
                }

                return field;
            }
            set
            {
                // 如果 location1 的值等于 comparand，则将 location1 设置为 value。 返回旧值
                var field = Interlocked.CompareExchange(ref current, value, null);
                if (field == null)
                {
                    return;
                }

                if (field == DisposedSentinel.Instance)
                {
                    value?.Dispose();
                    return;
                }
                ThrowAlreadyAssignment();
            }
        }

        static void ThrowAlreadyAssignment()
        {
            throw new InvalidOperationException("Disposable is already assigned.");
        }

        public void Dispose()
        {
            var field = Interlocked.Exchange(ref current, DisposedSentinel.Instance);
            if (field != DisposedSentinel.Instance)
            {
                field?.Dispose();
            }
        }
    }

    sealed class DisposedSentinel : IDisposable
    {
        public static readonly DisposedSentinel Instance = new();

        DisposedSentinel() { }

        public void Dispose() { }
    }
}
