using System.Collections.Concurrent;

namespace IceCoffee.Common
{
    /// <summary>
    /// 线程安全的串行工作队列
    /// </summary>
    public class SerialWorkQueue<T> : IDisposable
    {
        private readonly ConcurrentQueue<T> _queue;
        private Task? _task;
        private readonly int _delay;
        private volatile bool _isRunning;

        /// <summary>
        /// 做工作, 仅当待处理工作数量大于 0 时触发
        /// </summary>
        public event Func<T, Task>? DoWork;

        /// <summary>
        /// 构造 WorkQueue 实例
        /// </summary>
        /// <param name="delay"></param>
        public SerialWorkQueue(int delay = 20)
        {
            _queue = new ConcurrentQueue<T>();
            _delay = delay;
            _isRunning = true;
            _task = Task.Factory.StartNew(Callback, TaskCreationOptions.LongRunning);
        }

        private async void Callback()
        {
            while (_isRunning)
            {
                if (DoWork != null && _queue.TryDequeue(out var result))
                {
                    try
                    {
                        await DoWork.Invoke(result).ConfigureAwait(false);
                    }
                    catch
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

        public void Dispose()
        {
            _isRunning = false;
            if (_task != null)
            {
                _task.Wait();
                ((IDisposable)_task).Dispose();
                _task = null;
            }
        }
    }
}