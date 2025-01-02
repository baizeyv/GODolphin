using System;
using System.Collections.Generic;

namespace GODolphin.Pool
{
    public abstract class Pool<T> : IPool<T>
    {
        /// <summary>
        /// * The maximum number of objects that will be pooled.
        /// </summary>
        public readonly int max;

        /// <summary>
        /// * The highest number of free objects. Can be reset any time.
        /// </summary>
        public int peak;

        protected readonly Queue<T> _freeObjects;

        protected IObjectFactory<T> _factory;

        public int CurInUseCount
        {
            get;
            private set;
        }

        public int FreeCount => _freeObjects.Count;

        public void SetObjectFactory(IObjectFactory<T> factory)
        {
            _factory = factory;
        }

        public void SetFactoryMethod(Func<T> factoryMethod)
        {
            _factory = new CustomObjectFactory<T>(factoryMethod);
        }

        public Pool()
            : this(16, 20) { }

        public Pool(int initialCapacity)
            : this(initialCapacity, 20) { }

        public Pool(int initialCapacity, int max)
        {
            _freeObjects = new Queue<T>(initialCapacity);
            this.max = max;
        }

        public T Obtain()
        {
            var obj = _freeObjects.Count == 0 ? _factory.Create() : _freeObjects.Dequeue();
            CurInUseCount++;
            return obj;
        }

        public void Free(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            CustomFree(obj);

            if (_freeObjects.Count < max)
            {
                _freeObjects.Enqueue(obj);
                peak = Math.Max(peak, _freeObjects.Count);
            }
            Reset(obj);
            CurInUseCount--;
        }

        protected abstract void CustomFree(T obj);

        protected void Reset(T obj)
        {
            if (obj is IPoolable poolableObject)
            {
                poolableObject.Reset();
            }
        }

        public void Clear()
        {
            _freeObjects.Clear();
        }

        public int GetFree()
        {
            return _freeObjects.Count;
        }
    }
}
