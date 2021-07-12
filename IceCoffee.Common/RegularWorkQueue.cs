using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IceCoffee.Common
{
    /// <summary>
    /// 定期工作队列
    /// </summary>
    public class RegularWorkQueue<T> : ConcurrentQueue<T>, IDisposable
    {
        private readonly Timer _timer;

        /// <summary>
        /// 做工作
        /// </summary>
        public event Action<IList<T>> DoWork;

        /// <summary>
        /// 构造 WorkQueue 实例
        /// </summary>
        /// <param name="processInterval">处理间隔</param>
        public RegularWorkQueue(double processInterval)
        {
            _timer = new Timer();
            _timer.Interval = processInterval;
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.IsEmpty == false)
            {
                List<T> works = new List<T>();

                while (this.IsEmpty == false)
                {
                    if (this.TryDequeue(out T result))
                    {
                        works.Add(result);
                    }
                }

                DoWork?.Invoke(works);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
