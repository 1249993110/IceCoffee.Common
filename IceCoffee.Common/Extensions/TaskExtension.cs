using System.Threading.Tasks;

namespace IceCoffee.Common.Extensions
{
    /// <summary>
    /// Task扩展
    /// </summary>
    public static class TaskExtension
    {
        /// <summary>
        /// 同步等待获取异步方法的结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static TResult WaitAndGetResult<TResult>(this Task<TResult> task)
        {
            task.Wait();
            return task.GetAwaiter().GetResult();
        }
    }
}