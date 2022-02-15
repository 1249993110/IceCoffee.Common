using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace IceCoffee.Common.Pools
{
    internal class LocklessPooledObjectPolicy<T> : PooledObjectPolicy<T> where T : class
    {
        private readonly Func<T>? _objectGenerator;

        public LocklessPooledObjectPolicy()
        {
        }

        public LocklessPooledObjectPolicy(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator;
        }

        /// <inheritdoc />
        public override T Create()
        {
            if (_objectGenerator != null)
            {
                return _objectGenerator.Invoke();
            }

            if (Activator.CreateInstance(typeof(T), true) is T t)
            {
                return t;
            }

            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public override bool Return(T obj)
        {
            return true;
        }
    }

}
