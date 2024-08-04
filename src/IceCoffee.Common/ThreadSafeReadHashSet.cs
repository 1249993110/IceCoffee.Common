using System.Collections;

namespace IceCoffee.Common
{
    /// <summary>
    /// 线程安全的哈希集合, 保护由多个线程读取、一个线程写入的资源
    /// </summary>
    public class ThreadSafeReadHashSet<T> : IEnumerable<T>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly HashSet<T> _hashSet = new HashSet<T>();

        /// <summary>
        /// 获取哈希集合中的元素个数
        /// </summary>
        public int Count
        {
            get
            {
                return _hashSet.Count;
            }
        }

        /// <summary>
        /// 判断哈希集合中是否包含指定的元素
        /// </summary>
        /// <param name="item">要判断的元素</param>
        /// <returns>如果哈希集合中包含指定的元素, 则为 true; 否则为 false</returns>
        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _hashSet.Contains(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 添加元素到哈希集合中
        /// </summary>
        /// <param name="item">要添加的元素</param>
        /// <returns>如果成功添加元素，则为 true；否则为 false。</returns>
        public bool Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加一个集合到哈希集合中
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var item in items)
                {
                    _hashSet.Add(item);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 删除哈希集合中的元素
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 删除哈希集合中的多个元素
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(IEnumerable<T> items)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var item in items)
                {
                    _hashSet.Remove(item);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _hashSet.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 将哈希集合的元素复制到指定的数组中, 从指定的索引位置开始
        /// </summary>
        /// <param name="array">目标数组</param>
        /// <param name="index">目标数组的起始索引</param>
        /// <param name="count">要复制的元素个数</param>
        public void CopyTo(T[] array, int index, int count)
        {
            _lock.EnterReadLock();
            try
            {
                _hashSet.CopyTo(array, index, count);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 将哈希集合的元素复制到一个新数组中
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            _lock.EnterReadLock();
            try
            {
                T[] array = new T[_hashSet.Count];
                _hashSet.CopyTo(array, 0, _hashSet.Count);
                return array;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 实现 IEnumerable 接口的 GetEnumerator 方法
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ToArray().AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// 实现 IEnumerable 接口的 GetEnumerator 方法
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ~ThreadSafeReadHashSet()
        {
            if (_lock != null)
            {
                _lock.Dispose();
            }
        }
    }
}
