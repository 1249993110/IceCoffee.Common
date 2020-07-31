using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.LogManager
{
    /// <summary>
    /// 日志记录事件
    /// </summary>
    public delegate void LogRecordedEventHandler(string message, Exception exception, LogLevel logLevel);

    public static class Log
    {
        private static readonly NLog.Logger _logger;

        #region 构造方法
        static Log()
        {
            //初始化配置日志
            OverrideConfiguration(AppDomain.CurrentDomain.BaseDirectory + "DefaultNLog.config");
        
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }
        #endregion

        /// <summary>
        /// 日志被记录
        /// </summary>
        public static event LogRecordedEventHandler LogRecorded;

        /// <summary>
        /// 覆盖默认配置文件
        /// </summary>
        public static void OverrideConfiguration(LoggingConfiguration loggingConfiguration)
        {
            NLog.LogManager.Configuration = loggingConfiguration;
        }

        /// <summary>
        /// 覆盖默认配置文件路径
        /// </summary>
        public static void OverrideConfiguration(string path = "NLog.config")
        {
            NLog.LogManager.Configuration = new XmlLoggingConfiguration(path);
        }

        #region Trace
        /// <summary>
        /// 追踪信息
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message)
        {
            _logger.Trace(message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, null, LogLevel.Trace);
        }

        /// <summary>
        /// 追踪信息
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message, Exception exception)
        {
            _logger.Trace(exception, message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, exception, LogLevel.Trace);
        }
        #endregion

        #region Debug
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            _logger.Debug(message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, null, LogLevel.Debug);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message, Exception exception)
        {
            _logger.Debug(exception, message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, exception, LogLevel.Debug);
        }
        #endregion

        #region Info
        /// <summary>
        /// 状态信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            _logger.Info(message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, null, LogLevel.Info);
        }
        /// <summary>
        /// 状态信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message, Exception exception)
        {
            _logger.Info(exception, message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, exception, LogLevel.Info);
        }
        #endregion

        #region Warn
        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message)
        {
            _logger.Warn(message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, null, LogLevel.Warn);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message, Exception exception)
        {
            _logger.Warn(exception, message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, exception, LogLevel.Warn);
        }
        #endregion

        #region Error
        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            _logger.Error(message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, null, LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(Exception exception)
        {
            _logger.Error(exception);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + exception.Message, null, LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, exception, LogLevel.Error);
        }
        #endregion

        #region Fatal
        /// <summary>
        /// 将导致程序退出的严重错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(string message)
        {
            _logger.Fatal(message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, null, LogLevel.Fatal);
        }

        /// <summary>
        /// 将导致程序退出的严重错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(string message, Exception exception)
        {
            _logger.Fatal(exception, message);
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, exception, LogLevel.Fatal);
        }
        #endregion
    }
}
