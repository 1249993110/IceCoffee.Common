using System;
using System.Linq;
using System.Reflection;

namespace IceCoffee.Common
{
    /// <summary>
    /// 饿汉模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton1<T> where T : class, new()
    {
        private static readonly T _instance = new T();

        /// <summary>
        /// 获得实例
        /// </summary>
        public static T Instance { get { return _instance; } }
    }

    /// <summary>
    /// 懒汉模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton2<T> where T : class, new()
    {
        private static T? _instance;
        private static readonly object _singleton_Lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null) // 双if +lock
                {
                    lock (_singleton_Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// 使用反射，懒汉模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton3<T> where T : class
    {
        private static readonly Lazy<T> _instance
          = new Lazy<T>(() =>
          {
              var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

              if (ctors.Count() != 1)//Type {0} must have exactly one constructor.
              {
                  throw new InvalidOperationException(string.Format("类型{0}必须只有一个构造函数。", typeof(T)));
              }
              var ctor = ctors.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate);
              if (ctor == null)//The constructor for {0} must be private and take no parameters.
              {
                  throw new InvalidOperationException(string.Format("{0}的构造函数必须是私有的，并且不接受任何参数。", typeof(T)));
              }
              return (T)ctor.Invoke(null);
          }, true);

        public static T Instance
        {
            get { return _instance.Value; }
        }
    }
}