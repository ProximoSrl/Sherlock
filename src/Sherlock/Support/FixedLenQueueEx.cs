using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Sherlock.Support
{
    public class FixedLenQueue<T> : IEnumerable<T>
    {
        private readonly int _maxLen;
        private readonly ConcurrentQueue<T> _queue;

        public FixedLenQueue(int maxLen)
        {
            _maxLen = maxLen;
            _queue = new ConcurrentQueue<T>();
        }

        public void Add(T item)
        {
            while (_queue.Count > _maxLen)
            {
                _queue.TryDequeue(out T discarded);
            }

            _queue.Enqueue(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class FixedLenQueueEx<T> : IEnumerable<T>
    {
        private readonly int _maxLen;
        private ConcurrentQueue<T> _queue;
        private long _sequence = 0;

        public FixedLenQueueEx(int maxLen)
        {
            _maxLen = maxLen;
            _queue = new ConcurrentQueue<T>();
        }

        public T Add(Func<UInt32, T> add)
        {
            T item = add((UInt32)Interlocked.Increment(ref _sequence));

            while (_queue.Count >= _maxLen)
            {
                _queue.TryDequeue(out T discarded);
            }

            _queue.Enqueue(item);
            return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _queue = new ConcurrentQueue<T>();
        }
    }
}
