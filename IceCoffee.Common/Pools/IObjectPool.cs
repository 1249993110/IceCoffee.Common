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
    public interface IObjectPool<T> where T : class
    {
        /// <summary>
        /// Gets an object from the pool if one is available, otherwise creates one
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        /// <param name="obj">The object to add to the pool</param>
        void Return(T obj);
    }
}
