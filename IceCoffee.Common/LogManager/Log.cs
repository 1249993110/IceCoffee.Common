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
            OverrideConfiguration(AppDomain.CurrentDomain.BaseDirectory + "NLogDefault.config");

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
        private static void OnLogRecorded(Exception exception, string message, LogLevel logLevel)
        {
            LogRecorded?.Invoke(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + message, exception, logLevel);
        }
        #region Trace

        /// <summary>
        /// 追踪信息
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message)
        {
            _logger.Trace(message);
            OnLogRecorded(null, message, LogLevel.Trace);
        }

        /// <summary>
        /// 追踪信息
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(Exception exception, string message)
        {
            _logger.Trace(exception, message);
            OnLogRecorded(exception, message, LogLevel.Trace);
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
            OnLogRecorded(null, message, LogLevel.Debug);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(Exception exception, string message)
        {
            _logger.Debug(exception, message);
            OnLogRecorded(exception, message, LogLevel.Debug);
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
            OnLogRecorded(null, message, LogLevel.Info);
        }

        /// <summary>
        /// 状态信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
            OnLogRecorded(null, string.Format(message, args), LogLevel.Info);
        }

        /// <summary>
        /// 状态信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(Exception exception, string message)
        {
            _logger.Info(exception, message);
            OnLogRecorded(exception, message, LogLevel.Info);
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
            OnLogRecorded(null, message, LogLevel.Warn);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(Exception exception, string message)
        {
            _logger.Warn(exception, message);
            OnLogRecorded(exception, message, LogLevel.Warn);
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
            OnLogRecorded(null, message, LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
            OnLogRecorded(null, string.Format(message, args), LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(Exception exception)
        {
            _logger.Error(exception);
            OnLogRecorded(exception.InnerException, exception.Message, LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(Exception exception, string message)
        {
            _logger.Error(exception, message);
            OnLogRecorded(exception, message, LogLevel.Error);
        }

        /// <summary>
        /// 普通错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(Exception exception, string message, params object[] args)
        {
            _logger.Error(message, args);
            OnLogRecorded(exception, string.Format(message, args), LogLevel.Error);
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
            OnLogRecorded(null, message, LogLevel.Fatal);
        }

        /// <summary>
        /// 将导致程序退出的严重错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(Exception exception)
        {
            _logger.Fatal(exception);
            OnLogRecorded(exception.InnerException, exception.Message, LogLevel.Fatal);
        }

        /// <summary>
        /// 将导致程序退出的严重错误
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(Exception exception, string message)
        {
            _logger.Fatal(exception, message);
            OnLogRecorded(exception, message, LogLevel.Fatal);
        }

        #endregion Fatal
    }
}