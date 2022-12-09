namespace IceCoffee.Common.Wpf
{
    /// <summary>
    /// 清理ViewModel资源接口
    /// </summary>
    public interface IClearViewModel
    {
        /// <summary>
        /// 清理ViewModel资源, 在窗口关闭时调用
        /// </summary>
        void Clear();
    }
}