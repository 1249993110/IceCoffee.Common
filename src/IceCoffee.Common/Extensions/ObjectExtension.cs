namespace IceCoffee.Common.Extensions
{
    /// <summary>
    /// 对象扩展方法
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 尝试释放对象
        /// </summary>
        /// <param name="obj"></param>
        public static void TryDispose(this object obj)
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
