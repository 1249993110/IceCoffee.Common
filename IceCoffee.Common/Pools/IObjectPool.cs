using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Pools
{
    /// <summary>
    /// Object pool interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectPool<T> : IDisposable where T : class
    {
        /// <summary>
        /// 如果一个对象可用，则从池中获取一个对象，否则创建一个
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// 将对象返回到池中
        /// </summary>
        /// <param name="obj">The object to add to the pool</param>
        void Return(T obj);
    }
}
