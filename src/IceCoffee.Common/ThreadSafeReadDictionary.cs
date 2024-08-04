using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IceCoffee.Common
{
    /// <summary>
    /// 线程安全的字典, 保护由多个线程读取、一个线程写入的资源
    /// </summary>
    public class ThreadSafeReadDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
    {
        /// <summary>
        /// 添加或更新状态
        /// </summary>
        public enum AddOrUpdateStatus
        {
            Added,
            Updated,
            Unchanged
        };

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        /// <summary>
        /// 获取字典中的元素个数
        /// </summary>
        public int Count
        {
            get
            {
                return _dict.Count;
            }
        }

        /// <summary>
        /// 读取字典中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Read(TKey key)
        {
            _lock.EnterReadLock();
            try
            {
                return _dict[key];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 尝试读取字典中的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRead(TKey key, out TValue? value)
        {
            _lock.EnterReadLock();
            try
            {
                return _dict.TryGetValue(key, out value);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 添加元素到字典中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            _lock.EnterWriteLock();
            try
            {
                _dict.Add(key, value);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加元素到字典中, 可选择超时
        /// </summary>
        /// <param name="key">要添加的元素的键</param>
        /// <param name="value">要添加的元素的值</param>
        /// <param name="timeout">超时时间（以毫秒为单位）</param>
        /// <returns>如果成功添加元素，则为 true；否则为 false。</returns>
        public bool AddWithTimeout(TKey key, TValue value, int timeout)
        {
            if (_lock.TryEnterWriteLock(timeout))
            {
                try
                {
                    _dict.Add(key, value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 添加一个集合到字典中
        /// </summary>
        /// <param name="enumerable"></param>
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var kv in enumerable)
                {
                    _dict.Add(kv.Key, kv.Value);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加或更新字典中的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public AddOrUpdateStatus AddOrUpdate(TKey key, TValue value)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                if (_dict.TryGetValue(key, out var result))
                {
                    if (EqualityComparer<TValue>.Default.Equals(value, result))
                    {
                        return AddOrUpdateStatus.Unchanged;
                    }
                    else
                    {
                        _lock.EnterWriteLock();
                        try
                        {
                            _dict[key] = value;
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                        return AddOrUpdateStatus.Updated;
                    }
                }
                else
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        _dict.Add(key, value);
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                    return AddOrUpdateStatus.Added;
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// 删除字典中的元素
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            _lock.EnterWriteLock();
            try
            {
                _dict.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 删除字典中的多个元素
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveRange(IEnumerable<TKey> keys)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var key in keys)
                {
                    _dict.Remove(key);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取字典中的所有键
        /// </summary>
        /// <returns></returns>
        public TKey[] GetKeys()
        {
            _lock.EnterReadLock();
            try
            {
                TKey[] keys = new TKey[_dict.Count];
                _dict.Keys.CopyTo(keys, 0);
                return keys;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 获取字典中的所有值
        /// </summary>
        /// <returns></returns>
        public TValue[] GetValues()
        {
            _lock.EnterReadLock();
            try
            {
                TValue[] array = new TValue[_dict.Count];
                _dict.Values.CopyTo(array, 0);
                return array;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 判断字典中是否包含指定的键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            _lock.EnterReadLock();
            try
            {
                return _dict.ContainsKey(key);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 清空字典
        /// </summary>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _dict.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 将字典转换为键值对数组
        /// </summary>
        /// <returns>键值对数组</returns>
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            _lock.EnterReadLock();
            try
            {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[_dict.Count];
                ((IDictionary<TKey, TValue>)_dict).CopyTo(array, 0);
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
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
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

        ~ThreadSafeReadDictionary()
        {
            if (_lock != null)
            {
                _lock.Dispose();
            }
        }
    }
}
