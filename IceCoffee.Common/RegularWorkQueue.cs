using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common
{
    /// <summary>
    /// 定期工作队列
    /// </summary>
    public class RegularWorkQueue<T> : ConcurrentQueue<T>
    {
        private readonly Timer _timer;

        /// <summary>
        /// 做工作，仅当待处理工作数量大于 0 时触发
        /// </summary>
        public event Action<List<T>>? DoWork;

        /// <summary>
        /// 构造 WorkQueue 实例
        /// </summary>
        /// <param name="processInterval">处理间隔（单位：毫秒）</param>
        public RegularWorkQueue(int processInterval)
        {
            if(processInterval <= 0)
            {
                throw new ArgumentException("processInterval 必须大于 0", nameof(processInterval));
            }

            _timer = new Timer(TimerCallback, null, 0, processInterval);
        }

        private void TimerCallback(object? state)
        {
            if (this.IsEmpty == false)
            {
                var works = new List<T>();

                while (this.IsEmpty == false)
                {
                    if (this.TryDequeue(out T? result))
                    {
                        works.Add(result);
                    }
                }
                
                if(works.Count > 0)
                {
                    DoWork?.Invoke(works);
                }
            }
        }
    }
}
