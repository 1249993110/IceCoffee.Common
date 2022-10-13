using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Wpf
{
    /// <summary>
    /// 主动清理ViewModel接口, 在窗口关闭事件中调用
    /// </summary>
    public interface IClearCaller
    {
        /// <summary>
        /// 调用ViewModel的Clear接口
        /// </summary>
        void InvokeClearMethod();
    }
}
