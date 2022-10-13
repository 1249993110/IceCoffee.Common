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

        /// <summary>
        /// 做工作, 仅当待处理工作数量大于 0 时触发
        /// </summary>
        public event Action<List<T>>? DoWork;

        /// <summary>
        /// 构造 WorkQueue 实例
        /// </summary>
        /// <param name="processInterval">处理间隔（单位：毫秒）</param>
        public RegularWorkQueue(int processInterval)
        {
            _queue = new ConcurrentQueue<T>();

            if (processInterval <= 0)
            {
                throw new ArgumentException("processInterval 必须大于 0", nameof(processInterval));
            }

            _timer = new Timer(TimerCallback, null, 0, processInterval);
        }

        /// <summary>
        /// Adds an object to the end of the System.Collections.Concurrent.ConcurrentQueue
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

                do
                {
                    if (_queue.TryDequeue(out T? result))
                    {
                        works.Add(result);
                    }
                } while (_queue.IsEmpty == false);

                DoWork?.Invoke(works);
            }
        }
    }
}