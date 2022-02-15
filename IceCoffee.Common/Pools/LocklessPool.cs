using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Pools
{
    /// <summary>轻量级对象池。数组无锁实现，高性能</summary>
    /// <remarks>
    /// 内部 1+N 的存储结果，保留最热的一个对象在外层，便于快速存取。
    /// 数组具有极快的查找速度，结构体确保没有GC操作。
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class LocklessPool<T> : IObjectPool<T> where T : class
    {
        private readonly ObjectPool<T> _pool;

        public LocklessPool()
        {
            // 创建一个可销毁的对象池
            _pool = new DefaultObjectPoolProvider().Create(new LocklessPooledObjectPolicy<T>());
        }

        public LocklessPool(int maximumRetained)
        {
            // 创建一个可销毁的对象池
            _pool = new DefaultObjectPoolProvider() { MaximumRetained = maximumRetained }.Create(new LocklessPooledObjectPolicy<T>());
        }

        public LocklessPool(Func<T> objectGenerator)
        {
            // 创建一个可销毁的对象池
            _pool = new DefaultObjectPoolProvider().Create(new LocklessPooledObjectPolicy<T>(objectGenerator));
        }

        public LocklessPool(Func<T> objectGenerator, int maximumRetained)
        {
            // 创建一个可销毁的对象池
            _pool = new DefaultObjectPoolProvider() { MaximumRetained = maximumRetained }.Create(new LocklessPooledObjectPolicy<T>(objectGenerator));
        }

        public virtual T Get()
        {
            return _pool.Get();
        }

        public virtual void Return(T obj)
        {
            _pool.Return(obj);
        }

        public void Dispose()
        {
            if(_pool is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
