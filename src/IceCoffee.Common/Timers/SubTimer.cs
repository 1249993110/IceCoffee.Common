namespace IceCoffee.Common.Timers
{
    /// <summary>
    /// 子计时器
    /// </summary>
    public class SubTimer
    {
        /// <summary>
        /// 秒级的计数
        /// </summary>
        internal volatile int countInSeconds;

        private readonly Action _action;
        private int _interval = 1;
        private volatile bool _isEnabled;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="action"></param>
        public SubTimer(Action action)
        {
            this._action = action;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="action"></param>
        /// <param name="interval"></param>
        public SubTimer(Action action, int interval)
        {
            this._action = action;
            this.Interval = interval;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        public Action Action => _action;

        /// <summary>
        /// 执行间隔 (单位：秒, 默认：1 秒)
        /// </summary>
        public int Interval
        {
            get => _interval;
            set
            {
                if (value < 1)
                {
                    _isEnabled = false;
                }
                else
                {
                    _interval = value;
                }
            }
        }
        /// <summary>
        /// 是否启用, 设置为禁用时将重置计数
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == false)
                {
                    countInSeconds = 0;
                }

                _isEnabled = value;
            }
        }
    }
}