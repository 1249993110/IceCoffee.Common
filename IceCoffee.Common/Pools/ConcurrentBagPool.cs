using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace IceCoffee.Common.Pools
{
    /// <summary>
    /// 具有基础并发类型的线程安全对象池，<see cref="ConcurrentBag{T}"/>实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConcurrentBagPool<T> : ObjectPool<T>, IObjectPool<T> where T : class
    {
        #region 字段

        private readonly ConcurrentBag<T> _bag = new ConcurrentBag<T>();
        private readonly Func<T>? _objectGenerator;
        private readonly int _maximumRetained;

        #endregion 字段

        #region 属性

        /// <summary>
        /// 池中对象数量
        /// </summary>
        public int Count => _bag.Count;

        /// <summary>
        /// 最大保留数量，默认无限制
        /// </summary>
        public int MaximumRetained => _maximumRetained;
        #endregion 属性

        #region 方法

        /// <summary>
        /// 实例化 <see cref="ConcurrentBagPool{T}"/>
        /// </summary>
        public ConcurrentBagPool()
        {
        }

        /// <summary>
        /// 实例化 <see cref="ConcurrentBagPool{T}"/>
        /// </summary>
        /// <param name="maximumRetained"></param>
        public ConcurrentBagPool(int maximumRetained)
        {
            _maximumRetained = maximumRetained;
        }

        /// <summary>
        /// 实例化 <see cref="ConcurrentBagPool{T}"/>
        /// </summary>
        /// <param name="objectGenerator"></param>
        public ConcurrentBagPool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator;
        }

        /// <summary>
        /// 实例化 <see cref="ConcurrentBagPool{T}"/>
        /// </summary>
        /// <param name="objectGenerator"></param>
        /// <param name="maximumRetained"></param>
        public ConcurrentBagPool(Func<T> objectGenerator, int maximumRetained)
        {
            _objectGenerator = objectGenerator;
            _maximumRetained = maximumRetained;
        }

        /// <summary>
        /// 创建一个对象实例，默认调用 objectGenerator
        /// </summary>
        /// <returns>返回对象</returns>
        protected virtual T Create()
        {
            if(_objectGenerator != null)
            {
                return _objectGenerator.Invoke();
            }

            if(Activator.CreateInstance(typeof(T), true) is T t)
            {
                return t;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// 从池中取走一个对象
        /// </summary>
        /// <returns></returns>
        public override T Get()
        {
            if(_bag.TryTake(out T? item))
            {
                return item;
            }

            return Create();
        }

        /// <summary>
        /// 往池中放入一个对象
        /// </summary>
        /// <param name="item"></param>
        public override void Return(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if(_maximumRetained > 0 && Count >= _maximumRetained)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            else
            {
                _bag.Add(item);
            }
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
        /// 清空
        /// </summary>
        /// <returns></returns>
        public virtual void Clear()
        {
            while (this._bag.IsEmpty == false)
            {
                if (this._bag.TryTake(out T? item))
                {
                    if (item is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

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
        ~ConcurrentBagPool()
        {
            Dispose(false);
        }
        #endregion

        #endregion
    }
}