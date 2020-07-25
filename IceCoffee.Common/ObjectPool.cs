using System;
using System.Collections.Concurrent;

namespace IceCoffee.Common
{
    public abstract class ObjectPool<T> : IDisposable
    {
        #region 字段
        //并发安全集合
        private readonly ConcurrentBag<T> _bag = new ConcurrentBag<T>();

        //集合锁
        private readonly object _bagLock = new object();

        //是否已释放资源
        private bool _isDisposed;
        #endregion

        #region 属性
        /// <summary>
        /// 池中对象数量
        /// </summary>
        public int Count
        {
            get
            {
                return _bag.Count;
            }
        }

        /// <summary>
        /// 资源是否被释放
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                lock (_bagLock)
                {
                    return _isDisposed;
                }
            }
        }
        #endregion

        #region 方法
        public ObjectPool()
        {

        }

        /// <summary>
        /// 创建一个对象实例
        /// </summary>
        /// <returns>返回对象</returns>
        protected abstract T Create();

        /// <summary>
        /// 往池中添加一个对象
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            _bag.Add(item);
        }

        /// <summary>
        /// 从池中取走一个对象
        /// </summary>
        /// <returns></returns>
        public virtual T Take()
        {
            T item;
            return _bag.TryTake(out item) ? item : Create();
        }

        /// <summary>
        /// 使用Create方法创建指定数量对象初始化对象池
        /// </summary>
        /// <param name="count"></param>
        public virtual void Initialize(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                _bag.Add(Create());
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">正在释放资源</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (_bagLock)
            {
                if (_isDisposed)
                {
                    return;
                }                    

                if (disposing)
                {
                    // free managed objects here
                    for (int i = 0; i < this.Count; i++)
                    {
                        IDisposable item = this.Take() as IDisposable;
                        if (item != null)
                        {
                            item.Dispose();
                        }
                    }
                }

                _isDisposed = true;
            }
        }
        #endregion
    }
}
