using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Pools
{
    /// <summary>
    /// 内存流池
    /// </summary>
    public class MemoryStreamPool : LocklessPool<MemoryStream>
    {
        /// <summary>
        /// 初始容量。默认1024个
        /// </summary>
        public int InitialCapacity { get; set; } = 1024;

        /// <summary>
        /// 最大容量。超过该大小时不进入池内，默认64k
        /// </summary>
        public int MaximumCapacity { get; set; } = 64 * 1024;

       /// <summary>
       /// <inheritdoc/>
       /// </summary>
       /// <returns></returns>
        protected override MemoryStream Create()
        {
            return new MemoryStream(InitialCapacity);
        }

        /// <summary>归还</summary>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        public override void Return(MemoryStream memoryStream)
        {
            if (memoryStream.Capacity > MaximumCapacity)
            {
                return;
            }

            memoryStream.Position = 0;
            memoryStream.SetLength(0);
            base.Return(memoryStream);
        }
    }
}
