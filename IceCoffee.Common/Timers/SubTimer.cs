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
                    isEnabled = false;
                }
                else
                {
                    _interval = value;
                }
            } 
        }

        private int _interval = 1;
        private Action _action;

        /// <summary>
        /// 是否启用，在启用时禁用将重置计数
        /// </summary>
        public bool IsEnabled 
        { 
            get => isEnabled; 
            set 
            { 
                if (isEnabled && value == false)
                {
                    countInSeconds = 0;
                }

                isEnabled = value;
            }
        }

        internal volatile bool isEnabled;

        /// <summary>
        /// 秒级的计数
        /// </summary>
        internal int countInSeconds;

        /// <summary>
        /// 执行方法
        /// </summary>
        public Action Action => _action;

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

    }
}
