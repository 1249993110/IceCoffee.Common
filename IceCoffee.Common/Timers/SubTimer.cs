using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Timers
{
    /// <summary>
    /// 子计时器
    /// </summary>
    public class SubTimer
    {
        /// <summary>
        /// 执行间隔 (单位：秒，默认：1 秒)
        /// </summary>
        public int Interval 
        { 
            get => _interval; 
            set 
            {
                if (value < 1)
                {
                    IsEnabled = false;
                }
                else
                {
                    _interval = value;
                }
            } 
        }

        private int _interval = 1;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 秒级的计数
        /// </summary>
        internal int countInSeconds;

        /// <summary>
        /// 执行方法
        /// </summary>
        internal Action action;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="action"></param>
        /// <param name="interval"></param>
        public SubTimer(Action action, int interval)
        {
            this.action = action;
            this._interval = interval;
        }

    }
}
