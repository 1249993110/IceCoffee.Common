using NLog.Config;
using System;
using System.Runtime.CompilerServices;

namespace IceCoffee.Common.LogManager
{
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

        #endregion 构造方法

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnLogRecorded(string message, Exception exception, LogLevel logLevel)
        {
            LogRecorded?.Invoke(DateTime.Now.ToString() + " " + message, exception, logLevel);
        }

        #region Trace

        /// <summary>
        /// 追踪信息
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message)
        {
            _logger.Trace(message);
            OnLogRecorded(message, null, LogLevel.Trace);
        }

        /// <summary>
        /// 追踪信息
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message, Exception exception)
        {
            _logger.Trace(exception, message);
            OnLogRecorded(message, exception, LogLevel.Trace);
        }

        #endregion Trace

        #region Debug

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            _logger.Debug(message);
            OnLogRecorded(message, null, LogLevel.Debug);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message, Exception exception)
        {
            _logger.Debug(exception, message);
            OnLogRecorded(message, exception, LogLevel.Debug);
        }

        #endregion Debug

        #region Info

        /// <summary>
        /// 状态信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            _logger.Info(message);
            OnLogRecorded(message, null, LogLevel.Info);
        }

        /// <summary>
        /// 状态信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message, Exception exception)
        {
            _logger.Info(exception, message);
            OnLogRecorded(message, exception, LogLevel.Info);
        }

        #endregion Info

        #region Warn

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message)
        {
            _logger.Warn(message);
            OnLogRecorded(message, null, LogLevel.Warn);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message, Exception exception)
        {
            _logger.Warn(exception, message);
            OnLogRecorded(message, exception, LogLevel.Warn);
        }

        #endregion Warn

        #region Error

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            _logger.Error(message);
            OnLogRecorded(message, null, LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(Exception exception)
        {
            _logger.Error(exception);
            OnLogRecorded(exception.Message, exception.InnerException, LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
            OnLogRecorded(message, exception, LogLevel.Error);
        }

        #endregion Error

        #region Fatal

        /// <summary>
        /// 将导致程序退出的严重错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(string message)
        {
            _logger.Fatal(message);
            OnLogRecorded(message, null, LogLevel.Fatal);
        }

        /// <summary>
        /// 将导致程序退出的严重错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(Exception exception)
        {
            _logger.Fatal(exception);
            OnLogRecorded(exception.Message, exception.InnerException, LogLevel.Fatal);
        }

        /// <summary>
        /// 将导致程序退出的严重错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(string message, Exception exception)
        {
            _logger.Fatal(exception, message);
            OnLogRecorded(message, exception, LogLevel.Fatal);
        }

        #endregion Fatal
    }
}