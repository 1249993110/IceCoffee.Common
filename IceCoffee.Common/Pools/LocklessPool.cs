using System;
using System.Diagnostics;
using System.Threading;

namespace IceCoffee.Common.Pools
{
    /// <summary>轻量级对象池。数组无锁实现，高性能</summary>
    /// <remarks>
    /// 内部 1+N 的存储结果，保留最热的一个对象在外层，便于快速存取。
    /// 数组具有极快的查找速度，结构体确保没有GC操作。
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class LocklessPool<T> : IObjectPool<T>, IDisposable where T : class
    {
        #region 属性
        /// <summary>
        /// 对象池大小。默认CPU*2
        /// </summary>
        public int Count => _items.Length;

        private ObjectWrapper[] _items;
        private T _firstItem;

        /// <summary>
        /// PERF: the struct wrapper avoids array-covariance-checks from the runtime when assigning to elements of the array
        /// </summary>
        [DebuggerDisplay("{Element}")]
        private struct ObjectWrapper
        {
            public T Element;
        }

        #endregion 属性

        #region 构造

        /// <summary>
        /// 实例化对象池
        /// </summary>
        /// <param name="count">默认大小CPU*2</param>
        public LocklessPool(int count = 0)
        {
            if (count <= 0)
            {
                count = Environment.ProcessorCount * 2;
            }

            _items = new ObjectWrapper[count - 1];
        }
        #endregion 构造

        #region 方法

        /// <summary>
        /// 从池中取走一个对象
        /// </summary>
        /// <returns></returns>
        public virtual T Get()
        {
            // 最热的一个对象在外层，便于快速存取
            var item = _firstItem;
            if (item == null || Interlocked.CompareExchange(ref _firstItem, null, item) != item)
            {
                var items = _items;
                int len = items.Length;
                for (var i = 0; i < len; ++i)
                {
                    item = items[i].Element;
                    if (item != null && Interlocked.CompareExchange(ref items[i].Element, null, item) == item)
                    {
                        return item;
                    }
                }

                item = Create();
            }

            return item;
        }

        /// <summary>
        /// 往池中放入一个对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual void Return(T obj)
        {
            // 最热的一个对象在外层，便于快速存取
            if (_firstItem != null || Interlocked.CompareExchange(ref _firstItem, obj, null) != null)
            {
                var items = _items;
                int len = items.Length;
                for (var i = 0; i < len && Interlocked.CompareExchange(ref items[i].Element, obj, null) != null; ++i)
                {
                }
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public virtual void Clear()
        {
            DisposeItem(_firstItem);
            _firstItem = null;

            var items = _items;
            int len = items.Length;
            for (var i = 0; i < len; ++i)
            {
                DisposeItem(items[i].Element);
                items[i].Element = null;
            }

            _items = null;
        }

        private void DisposeItem(T item)
        {
            if (item is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        #endregion

        #region 重载
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual T Create()
        {
            var type = typeof(T);
            return Activator.CreateInstance(type, true) as T;
        }
        #endregion

        #region IDisposable implementation
        /// <summary>
        /// Disposed flag
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected virtual void Dispose(bool disposingManagedResources)
        {
            if (_isDisposed == false)
            {
                if (disposingManagedResources)
                {
                    // Dispose managed resources here...
                    Clear();
                }

                // Dispose unmanaged resources here...

                // Set large fields to null here...

                // Mark as disposed.
                _isDisposed = true;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        ~LocklessPool()
        {
            Dispose(false);
        }
        #endregion
    }
}