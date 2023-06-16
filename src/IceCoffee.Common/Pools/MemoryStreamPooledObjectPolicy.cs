using Microsoft.Extensions.ObjectPool;

namespace IceCoffee.Common.Pools
{
    /// <summary>
    /// 内存流池策略
    /// </summary>
    public class MemoryStreamPooledObjectPolicy : PooledObjectPolicy<MemoryStream>
    {
        public MemoryStreamPooledObjectPolicy()
        {
        }

        /// <summary>
        /// 初始容量。默认1024个
        /// </summary>
        public int InitialCapacity { get; set; } = 1024;

        /// <summary>
        /// 最大容量。超过该大小时不进入池内, 默认64k
        /// </summary>
        public int MaximumCapacity { get; set; } = 64 * 1024;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override MemoryStream Create()
        {
            return new MemoryStream(InitialCapacity);
        }

        /// <summary>归还</summary>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        public override bool Return(MemoryStream memoryStream)
        {
            if (memoryStream.Capacity > MaximumCapacity)
            {
                return false;
            }

            memoryStream.Position = 0;
            memoryStream.SetLength(0);

            return true;
        }
    }
}