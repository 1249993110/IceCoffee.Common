using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.LogManager
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>
        /// 追踪信息
        /// </summary>
        Trace,

        /// <summary>
        /// 普通信息
        /// </summary>
        Debug,

        /// <summary>
        /// 调试信息
        /// </summary>
        Info,

        /// <summary>
        /// 警告信息
        /// </summary>
        Warn,

        /// <summary>
        /// 错误信息
        /// </summary>
        Error,

        /// <summary>
        /// 严重错误
        /// </summary>
        Fatal

    }
}
