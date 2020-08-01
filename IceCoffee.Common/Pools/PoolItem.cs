namespace IceCoffee.Common.Pools
{
    /// <summary>资源池包装项，自动归还资源到池中</summary>
    /// <typeparam name="T"></typeparam>
    public class PoolItem<T> : DisposeBase where T : class
    {
        #region 属性

        /// <summary>数值</summary>
        public T Value { get; }

        /// <summary>池</summary>
        public IPool<T> Pool { get; }

        #endregion 属性

        #region 构造

        /// <summary>包装项</summary>
        /// <param name="pool"></param>
        /// <param name="value"></param>
        public PoolItem(IPool<T> pool, T value)
        {
            Pool = pool;
            Value = value;
        }

        /// <summary>销毁</summary>
        /// <param name="disposing"></param>
        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);

            Pool.Put(Value);
        }

        #endregion 构造
    }
}