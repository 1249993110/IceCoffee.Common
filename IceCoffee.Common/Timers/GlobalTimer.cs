using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IceCoffee.Common.Timers
{
    /// <summary>
    /// 全局计时器
    /// </summary>
    public static class GlobalTimer
    {
        private static Timer _timer;

        private static List<SubTimer> _subTimers;

        /// <summary>
        /// 构造
        /// </summary>
        static GlobalTimer()
        {
            _timer = new Timer(1000D);
            _timer.Elapsed += _timer_Elapsed;
            _subTimers = new List<SubTimer>();
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var subTimer in _subTimers)
            {
                if (subTimer.isEnabled)
                {
                    ++subTimer.countInSeconds;

                    if (subTimer.countInSeconds >= subTimer.Interval)
                    {
                        subTimer.countInSeconds = 0;
                        Task.Run(subTimer.Action);
                    }
                }
            }
        }

        /// <summary>
        /// 开始全部定时器
        /// </summary>
        public static void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// 停止全部定时器，并重置计数
        /// </summary>
        public static void Stop()
        {
            _timer.Stop();

            foreach (var subTimer in _subTimers)
            {
                subTimer.countInSeconds = 0;
            }
        }

        /// <summary>
        /// 注册子计时器
        /// </summary>
        /// <param name="subTimer"></param>
        public static void RegisterSubTimer(SubTimer subTimer)
        {
            if(subTimer == null)
            {
                throw new ArgumentNullException(nameof(subTimer));
            }

            if(_subTimers.Contains(subTimer))
            {
                throw new Exception("subTimer已注册");
            }

            _subTimers.Add(subTimer);
        }

        /// <summary>
        /// 取消注册子计时器，并重置其计数
        /// </summary>
        /// <param name="subTimer"></param>
        public static void UnregisterSubTimer(SubTimer subTimer)
        {
            if (subTimer == null)
            {
                throw new ArgumentNullException(nameof(subTimer));
            }

            subTimer.IsEnabled = false;
            _subTimers.Remove(subTimer);
        }
    }
}
