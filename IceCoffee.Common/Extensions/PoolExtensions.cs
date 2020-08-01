using IceCoffee.Common.Pools;
using System;
using System.IO;
using System.Text;

namespace IceCoffee.Common.Extensions
{
    /// <summary>
    /// 对象池扩展
    /// </summary>
    public static class PoolExtensions
    {
        #region StringBuilder

        /// <summary>字符串构建器池</summary>
        public static IPool<StringBuilder> StringBuilder { get; set; } = new StringBuilderPool();

        /// <summary>归还一个字符串构建器到对象池</summary>
        /// <param name="sb"></param>
        /// <param name="requireResult">是否需要返回结果</param>
        /// <returns></returns>
        public static string Put(this StringBuilder sb, bool requireResult = false)
        {
            if (sb == null) return null;

            var str = requireResult ? sb.ToString() : null;

            PoolExtensions.StringBuilder.Put(sb);

            return str;
        }

        /// <summary>字符串构建器池</summary>
        public class StringBuilderPool : LocklessPool<StringBuilder>
        {
            /// <summary>初始容量。默认100个</summary>
            public int InitialCapacity { get; set; } = 100;

            /// <summary>最大容量。超过该大小时不进入池内，默认4k</summary>
            public int MaximumCapacity { get; set; } = 4 * 1024;

            /// <summary>创建</summary>
            /// <returns></returns>
            protected override StringBuilder OnCreate() => new StringBuilder(InitialCapacity);

            /// <summary>归还</summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public override bool Put(StringBuilder value)
            {
                if (value.Capacity > MaximumCapacity) return false;

                value.Clear();

                return true;
            }
        }

        #endregion StringBuilder

        #region MemoryStream

        /// <summary>内存流池</summary>
        public static IPool<MemoryStream> MemoryStream { get; set; } = new MemoryStreamPool();

        /// <summary>归还一个内存流到对象池</summary>
        /// <param name="ms"></param>
        /// <param name="requireResult">是否需要返回结果</param>
        /// <returns></returns>
        public static Byte[] Put(this MemoryStream ms, bool requireResult = false)
        {
            if (ms == null) return null;

            var buf = requireResult ? ms.ToArray() : null;

            PoolExtensions.MemoryStream.Put(ms);

            return buf;
        }

        /// <summary>内存流池</summary>
        public class MemoryStreamPool : LocklessPool<MemoryStream>
        {
            /// <summary>初始容量。默认1024个</summary>
            public int InitialCapacity { get; set; } = 1024;

            /// <summary>最大容量。超过该大小时不进入池内，默认64k</summary>
            public int MaximumCapacity { get; set; } = 64 * 1024;

            /// <summary>创建</summary>
            /// <returns></returns>
            protected override MemoryStream OnCreate() => new MemoryStream(InitialCapacity);

            /// <summary>归还</summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public override bool Put(MemoryStream value)
            {
                if (value.Capacity > MaximumCapacity) return false;

                value.Position = 0;
                value.SetLength(0);

                return true;
            }
        }

        #endregion MemoryStream
    }
}