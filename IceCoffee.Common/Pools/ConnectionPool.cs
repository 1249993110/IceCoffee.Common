using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace IceCoffee.Common.Pools
{
    /// <summary>资源池。支持空闲释放，主要用于数据库连接池和网络连接池</summary>
    /// <typeparam name="T"></typeparam>
    public class ConnectionPool<T> : DisposeBase, IPool<T> where T : class
    {
        #region 属性
        /// <summary>名称</summary>
        public string Name { get; set; }

        private int _freeCount;
        /// <summary>空闲个数</summary>
        public int FreeCount { get { return _freeCount; } }

        private int _busyCount;
        /// <summary>繁忙个数</summary>
        public int BusyCount { get { return _busyCount; } }

        private int _max = 100;
        /// <summary>最大个数。默认100</summary>
        public int Max { get { return _max; } set { _max = value; } }

        private int _min = 1;
        /// <summary>最小个数。默认1</summary>
        public int Min { get { return _min; } set { _min = value; } }

        private int _idleTime = 10;
        /// <summary>空闲清理时间。最小个数之上的资源超过空闲时间时被清理，默认10s</summary>
        public int IdleTime { get { return _idleTime; } set { _idleTime = value; } }

        private int _allIdleTime = 0;
        /// <summary>完全空闲清理时间。最小个数之下的资源超过空闲时间时被清理，默认0s永不清理</summary>
        public int AllIdleTime { get { return _allIdleTime; } set { _allIdleTime = value; } }

        /// <summary>基础空闲集合。只保存最小个数，最热部分</summary>
        private readonly ConcurrentStack<Item> _free = new ConcurrentStack<Item>();

        /// <summary>扩展空闲集合。保存最小个数以外部分</summary>
        private readonly ConcurrentQueue<Item> _free2 = new ConcurrentQueue<Item>();

        /// <summary>借出去的放在这</summary>
        private readonly ConcurrentDictionary<T, Item> _busy = new ConcurrentDictionary<T, Item>();

        private readonly object _syncRoot = new object();
        #endregion

        #region 构造
        /// <summary>实例化一个资源池</summary>
        public ConnectionPool()
        {
            var str = this.GetType().Name;
            if (str.Contains("`"))
            {
                //str = str.Substring(null, "`");
                str = Substring(str, null, "`");
            }
            if (str != "Pool")
                Name = str;
            else
                Name = string.Format("Pool<{0}>", new object[] { typeof(T).Name });
        }

        /// <summary>销毁</summary>
        /// <param name="disposing"></param>
        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);

            _timer.TryDispose();

            //WriteLog($"Dispose {typeof(T).FullName} FreeCount={FreeCount:n0} BusyCount={BusyCount:n0} Total={Total:n0}");

            Clear();
        }

        private volatile bool _inited;
        private void Init()
        {
            if (_inited) return;

            lock (_syncRoot)
            {
                if (_inited) return;
                _inited = true;

                //WriteLog($"Init {typeof(T).FullName} Min={Min} Max={Max} IdleTime={IdleTime}s AllIdleTime={AllIdleTime}s");
            }
        }
        #endregion

        #region 内嵌
        class Item
        {
            /// <summary>数值</summary>
            public T Value { get; set; }

            /// <summary>过期时间</summary>
            public DateTime LastTime { get; set; }
        }
        #endregion

        #region 主方法
        /// <summary>借出</summary>
        /// <returns></returns>
        public virtual T Take()
        {
            //var sw = Log == null || Log == Logger.Null ? null : Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();
            Interlocked.Increment(ref _Total);

            Item pi = null;
            do
            {
                // 从空闲集合借一个
                if (_free.TryPop(out pi) || _free2.TryDequeue(out pi))
                {
                    Interlocked.Decrement(ref _freeCount);
                }
                else
                {
                    // 超出最大值后，抛出异常
                    var count = _busyCount;
                    if (count >= _max)
                    {
                        //var msg = $"申请失败，已有 {count:n0} 达到或超过最大值 {Max:n0}";
                        var msg = string.Format("申请失败，已有 {0} 达到或超过最大值 {1}", new object[] { count, _max });

                        //WriteLog("Acquire Max " + msg);

                        throw new Exception(Name + " " + msg);
                    }

                    // 借不到，增加
                    pi = new Item
                    {
                        Value = Create(),
                    };

                    if (count == 0) Init();
                    //WriteLog("Acquire Create Free={0} Busy={1}", FreeCount, count + 1);
                }

                // 借出时如果不可用，再次借取
            } while (!OnGet(pi.Value));

            // 最后时间
            //pi.LastTime = TimerX.Now;
            pi.LastTime = DateTime.Now;

            // 加入繁忙集合
            _busy.TryAdd(pi.Value, pi);

            Interlocked.Increment(ref _busyCount);
            Interlocked.Increment(ref _Success);
            if (sw != null)
            {
                sw.Stop();
                var ms = sw.Elapsed.TotalMilliseconds;

                if (Cost < 0.001)
                    Cost = ms;
                else
                    Cost = (Cost * 3 + ms) / 4;
            }

            return pi.Value;
        }

        /// <summary>借出时是否可用</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool OnGet(T value)
        {
            return true;
        }

        /// <summary>申请资源包装项，Dispose时自动归还到池中</summary>
        /// <returns></returns>
        public PoolItem<T> GetItem()
        {
            return new PoolItem<T>(this, Take());
        }

        /// <summary>归还</summary>
        /// <param name="value"></param>
        public virtual bool Put(T value)
        {
            if (value == null) return false;

            // 从繁忙队列找到并移除缓存项
            if (!_busy.TryRemove(value, out var pi))
            {
                //WriteLog("Put Error");

                return false;
            }

            Interlocked.Decrement(ref _busyCount);

            // 是否可用
            if (!OnPut(value)) return false;

            var db = value as DisposeBase;
            if (db != null && db.Disposed) return false;

            var min = _min;

            // 如果空闲数不足最小值，则返回到基础空闲集合
            if (_freeCount < min || _free.Count < min)
                _free.Push(pi);
            else
                _free2.Enqueue(pi);

            // 最后时间
            //pi.LastTime = TimerX.Now;
            pi.LastTime = DateTime.Now;

            Interlocked.Increment(ref _freeCount);

            // 启动定期清理的定时器
            StartTimer();

            return true;
        }

        /// <summary>归还时是否可用</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool OnPut(T value)
        {
            return true;
        }

        /// <summary>清空已有对象</summary>
        public virtual int Clear()
        {
            var count = _freeCount + _busyCount;

            //_busy.Clear();
            //_BusyCount = 0;

            //_free.Clear();
            //while (_free2.TryDequeue(out var rs)) ;
            //_FreeCount = 0;

            while (_free.TryPop(out var pi)) OnDispose(pi.Value);
            while (_free2.TryDequeue(out var pi)) OnDispose(pi.Value);
            _freeCount = 0;

            foreach (var item in _busy)
            {
                OnDispose(item.Key);
            }
            _busy.Clear();
            _busyCount = 0;

            return count;
        }

        /// <summary>销毁</summary>
        /// <param name="value"></param>
        protected virtual void OnDispose(T value)
        {
            value.TryDispose();
        }
        #endregion

        #region 重载
        /// <summary>创建实例</summary>
        /// <returns></returns>
        //protected virtual T OnCreate() => typeof(T).CreateInstance() as T;
        protected virtual T Create()
        {
            var type = typeof(T);
            return Activator.CreateInstance(type, true) as T;
        }
        #endregion

        #region 定期清理
        //private TimerX _timer;
        private Timer _timer;

        private void StartTimer()
        {
            if (_timer != null) return;
            lock (this)
            {
                if (_timer != null) return;

                //_timer = new TimerX(Work, null, 5000, 5000) { Async = true };
                _timer = new Timer(Work, null, 5000, 5000);
            }
        }

        private void Work(object state)
        {
            //// 总数小于等于最小个数时不处理
            //if (FreeCount + BusyCount <= Min) return;

            // 遍历并干掉过期项
            var count = 0;

            // 清理过期不还。避免有借没还
            if (!_busy.IsEmpty)
            {
                //var exp = TimerX.Now.AddSeconds(-AllIdleTime);
                var exp = DateTime.Now.AddSeconds(-_allIdleTime);
                foreach (var item in _busy)
                {
                    if (item.Value.LastTime < exp)
                    {
                        if (_busy.TryRemove(item.Key, out var v))
                        {
                            // 业务层可能故意有借没还
                            //v.TryDispose();

                            Interlocked.Decrement(ref _busyCount);
                        }
                    }
                }
            }

            // 总数小于等于最小个数时不处理
            if (_idleTime > 0 && !_free2.IsEmpty && _freeCount + _busyCount > _min)
            {
                //var exp = TimerX.Now.AddSeconds(-IdleTime);
                var exp = DateTime.Now.AddSeconds(-_idleTime);
                // 移除扩展空闲集合里面的超时项
                while (_free2.TryPeek(out var pi) && pi.LastTime < exp)
                {
                    // 取出来销毁
                    if (_free2.TryDequeue(out pi))
                    {
                        pi.Value.TryDispose();

                        count++;
                        Interlocked.Decrement(ref _freeCount);
                    }
                }
            }

            if (_allIdleTime > 0 && !_free.IsEmpty)
            {
                //var exp = TimerX.Now.AddSeconds(-AllIdleTime);
                var exp = DateTime.Now.AddSeconds(-_allIdleTime);
                // 移除基础空闲集合里面的超时项
                while (_free.TryPeek(out var pi) && pi.LastTime < exp)
                {
                    // 取出来销毁
                    if (_free.TryPop(out pi))
                    {
                        pi.Value.TryDispose();

                        count++;
                        Interlocked.Decrement(ref _freeCount);
                    }
                }
            }

            if (count > 0)
            {
                var p = Total == 0 ? 0 : (double)Success / Total;

                //WriteLog("Release Free={0} Busy={1} 清除过期资源 {2:n0} 项。总请求 {3:n0} 次，命中 {4:p2}，平均 {5:n2}us", FreeCount, BusyCount, count, Total, p, Cost * 1000);
            }
        }
        #endregion

        #region 统计
        private int _Total;
        /// <summary>总请求数</summary>
        public int Total
        {
            get
            {
                return _Total;
            }
        }

        private int _Success;
        /// <summary>成功数</summary>
        public int Success
        {
            get
            {
                return _Success;
            }
        }

        /// <summary>平均耗时。单位ms</summary>
        private double Cost;
        #endregion

        #region 日志
        /// <summary>日志</summary>
        //public ILog Log { get; set; } = Logger.Null;

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        //public void WriteLog(string format, params object[] args)
        //{
        //    if (Log == null) return;

        //    Log.Info(Name + "." + format, args);
        //}
        #endregion

        private static string Substring(string str, string after, string before = null, int startIndex = 0, int[] positions = null)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (string.IsNullOrEmpty(after) && string.IsNullOrEmpty(before)) return str;

            /*
             * 1，只有start，从该字符串之后部分
             * 2，只有end，从开头到该字符串之前
             * 3，同时start和end，取中间部分
             */

            var p = -1;
            if (!string.IsNullOrEmpty(after))
            {
                p = str.IndexOf(after, startIndex);
                if (p < 0) return null;
                p += after.Length;

                // 记录位置
                if (positions != null && positions.Length > 0) positions[0] = p;
            }

            if (string.IsNullOrEmpty(before)) return str.Substring(p);

            var f = str.IndexOf(before, p >= 0 ? p : startIndex);
            if (f < 0) return null;

            // 记录位置
            if (positions != null && positions.Length > 1) positions[1] = f;

            if (p >= 0)
                return str.Substring(p, f - p);
            else
                return str.Substring(0, f);
        }
    }
}
