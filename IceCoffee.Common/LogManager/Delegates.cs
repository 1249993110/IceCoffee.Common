using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.LogManager
{
    /// <summary>
    /// 日志记录事件处理器
    /// </summary>
    public delegate void LogRecordedEventHandler(string message, Exception exception, LogLevel logLevel);

}
