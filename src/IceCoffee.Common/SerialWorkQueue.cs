using System.Collections.Concurrent;

namespace IceCoffee.Common
{
    /// <summary>
    /// 线程安全的串行工作队列
    /// </summary>
    public class SerialWorkQueue<T> : IDisposable
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly Task _task;
        private readonly int _delay;
        private volatile bool _isRunning;

        /// <summary>
        /// 做工作, 仅当待处理工作数量大于 0 时触发
        /// </summary>
        private readonly Action<T> _callback;

        /// <summary>
        /// 构造 WorkQueue 实例
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="delay"></param>
        public SerialWorkQueue(Action<T> callback, int delay = 20)
        {
            _callback = callback;
            _queue = new ConcurrentQueue<T>();
            _delay = delay;
            _isRunning = true;
            _task = Task.Factory.StartNew(Callback, TaskCreationOptions.LongRunning);
        }

        private void Callback()
        {
            while (_isRunning)
            {
                if (_queue.TryDequeue(out var result))
                {
                    try
                    {
                        _callback.Invoke(result);
                    }
                    finally
                    {
                    }
                }
                else
                {
                    Thread.Sleep(_delay);
                }
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="ConcurrentQueue{T}"></see>
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        #region IDisposable implementation

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isRunning = false;
                _task.Wait();
                _task.Dispose();
            }
        }

        #endregion
    }
}