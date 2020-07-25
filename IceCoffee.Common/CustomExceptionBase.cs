using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common
{
    /// <summary>
    /// 自定义异常类型
    /// </summary>
    public enum CustomExceptionType
    {
        /// <summary>
        /// 非检查异常，致命异常并且对于被捕获者不知如何处理的异常
        /// </summary>
        Unchecked,

        /// <summary>
        /// 检查异常，必须处理的异常
        /// </summary>
        Checked
    }

    /// <summary>
    /// 自定义异常基类
    /// </summary>
    public class CustomExceptionBase : Exception
    {
        /// <summary>
        /// 异常类型
        /// </summary>
        public CustomExceptionType CustomExceptionType { get; set; } = CustomExceptionType.Unchecked;

        public CustomExceptionBase()
        {

        }

        public CustomExceptionBase(string message) : base(message)
        {

        }

        public CustomExceptionBase(string message, Exception innerException) : base(message, innerException)
        {

        }

    }
}
