namespace IceCoffee.Common.Timers
{
    /// <summary>
    /// 全局计时器
    /// </summary>
    public static class GlobalTimer
    {
        private static readonly Timer _timer;
        private static readonly List<SubTimer> _subTimers;

        /// <summary>
        /// 构造
        /// </summary>
        static GlobalTimer()
        {
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, 1000);
            _subTimers = new List<SubTimer>();
        }

        private static void TimerCallback(object? state)
        {
            SubTimer[] subTimers;
            lock (_timer)
            {
                subTimers = _subTimers.Where(i => i.IsEnabled).ToArray();
            }

            Parallel.ForEach(subTimers, (subTimer) =>
            {
                Interlocked.Increment(ref subTimer.countInSeconds);

                if (subTimer.countInSeconds >= subTimer.Interval)
                {
                    subTimer.countInSeconds = 0;
                    subTimer.Action.Invoke();
                }
            });
        }

        /// <summary>
        /// 注册子计时器
        /// </summary>
        /// <param name="subTimer"></param>
        public static void RegisterSubTimer(SubTimer subTimer)
        {
            if (subTimer == null)
            {
                throw new ArgumentNullException(nameof(subTimer));
            }

            lock (_timer)
            {
                if (_subTimers.Contains(subTimer))
                {
                    throw new Exception("SubTimer registered.");
                }

                _subTimers.Add(subTimer);

                if(_subTimers.Count == 1)
                {
                    _timer.Change(0, 1000);
                }
            }
        }

        /// <summary>
        /// 取消注册子计时器, 并重置其计数
        /// </summary>
        /// <param name="subTimer"></param>
        public static void UnregisterSubTimer(SubTimer subTimer)
        {
            if (subTimer == null)
            {
                throw new ArgumentNullException(nameof(subTimer));
            }

            subTimer.IsEnabled = false;

            lock (_timer)
            {
                _subTimers.Remove(subTimer);

                if (_subTimers.Count == 0)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }
    }
}