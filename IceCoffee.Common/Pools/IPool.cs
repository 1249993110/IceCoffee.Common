namespace IceCoffee.Common.Pools
{
    /// <summary>对象池接口</summary>
    /// <typeparam name="T"></typeparam>
    public interface IPool<T> where T : class
    {
        /// <summary>从池中取走一个对象</summary>
        /// <returns></returns>
        T Take();

        /// <summary>往池中放入一个对象</summary>
        /// <param name="value">是否归还成功</param>
        bool Put(T value);

        /// <summary>清空</summary>
        int Clear();
    }
}