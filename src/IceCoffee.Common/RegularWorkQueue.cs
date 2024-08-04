using System.Collections.Concurrent;

namespace IceCoffee.Common
{
    /// <summary>
    /// 线程安全的定期工作队列
    /// </summary>
    public class RegularWorkQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly Timer _timer;
        private readonly Action<List<T>> _callback;

        /// <summary>
        /// 构造 WorkQueue 实例
        /// </summary>
        /// <param name="callback">当待处理工作数量大于 0 时触发</param>
        /// <param name="processInterval">处理间隔（单位：毫秒）</param>
        public RegularWorkQueue(Action<List<T>> callback, int processInterval)
        {
            if (processInterval <= 0)
            {
                throw new ArgumentException("processInterval 必须大于 0", nameof(processInterval));
            }

            _queue = new ConcurrentQueue<T>();
            _callback = callback;
            _timer = new Timer(TimerCallback, null, 0, processInterval);
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="ConcurrentQueue{T}"></see>
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        private void TimerCallback(object? state)
        {
            if (_queue.IsEmpty == false)
            {
                var works = new List<T>();

                while (_queue.TryDequeue(out var result))
                {
                    works.Add(result);
                }

                _callback.Invoke(works);
            }
        }

        ///// <summary>
        ///// Copies the elements stored in the <see cref="ConcurrentQueue{T}"/> to a new array.
        ///// </summary>
        ///// <returns>A new array containing a snapshot of elements copied from the <see cref="ConcurrentQueue{T}"/></returns>
        //public T[] ToArray()
        //{
        //    return _queue.ToArray();
        //}

    }
}