using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace IceCoffee.Common
{
    /// <summary>
    /// 具有默认构造方法的对象深拷贝
    /// </summary>
    public static class ObjectClone
    {
        #region 深拷贝

        /// <summary>
        /// 深拷贝对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T obj) where T : new()
        {
            object result;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                result = ser.ReadObject(ms);
                ms.Close();
            }
            return (T)result;
        }

        /// <summary>
        /// 深拷贝对象，不能有循环引用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepCopy(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            Type type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(
                     type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(DeepCopy(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, DeepCopy(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }

        #endregion 深拷贝

        /// <summary>
        /// 简单拷贝对象的公开属性
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public static TOut CopyProperties<TOut>(object inObject) where TOut : new()
        {
            return (TOut)CopyProperties(inObject, typeof(TOut));
        }

        /// <summary>
        /// 简单拷贝对象的公开属性
        /// </summary>
        /// <param name="outType"></param>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public static object CopyProperties(object inObject, Type outType)
        {
            object outObject = Activator.CreateInstance(outType);

            Type inObjectType = inObject.GetType();

            foreach (PropertyInfo propertyInfo in outType.GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    object propertyValue = inObjectType.GetProperty(propertyInfo.Name).GetValue(inObject);
                    if (propertyValue != null)
                    {
                        propertyInfo.SetValue(outObject, propertyValue);
                    }
                }
            }
            return outObject;
        }

        /// <summary>
        /// 简单拷贝对象的公开属性
        /// </summary>
        /// <param name="srcObject"></param>
        /// <param name="toObject"></param>
        public static void CopyProperties(object srcObject, object toObject)
        {
            Type srcObjectType = srcObject.GetType();

            foreach (PropertyInfo propertyInfo in toObject.GetType().GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    object propertyValue = srcObjectType.GetProperty(propertyInfo.Name).GetValue(srcObject);
                    if (propertyValue != null)
                    {
                        propertyInfo.SetValue(toObject, propertyValue);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 具有默认构造方法的对象浅拷贝
    /// </summary>
    public static class ObjectClone<TIn, TOut>
        where TIn : new()
        where TOut : new()
    {
        private static readonly Func<TIn, TOut> cache = GetFunc();

        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                PropertyInfo propertyInfo = typeof(TIn).GetProperty(item.Name);

                if (propertyInfo != null && propertyInfo.PropertyType == item.PropertyType && item.CanWrite)
                {
                    MemberExpression property = Expression.Property(parameterExpression, propertyInfo);
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        /// <summary>
        /// 浅拷贝对象
        /// </summary>
        /// <param name="tIn"></param>
        /// <returns></returns>
        public static TOut ShallowCopy(TIn tIn)
        {
            return cache(tIn);
        }
    }

    /// <summary>
    /// 具有默认构造方法的对象浅拷贝
    /// </summary>
    public static class ObjectClone<TObject> where TObject : new()
    {
        private static readonly Func<TObject, TObject> cache = GetFunc();

        private static Func<TObject, TObject> GetFunc()
        {
            Type objectType = typeof(TObject);
            ParameterExpression parameterExpression = Expression.Parameter(objectType, "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in objectType.GetProperties())
            {
                if (item.CanWrite == false)
                {
                    continue;
                }

                MemberExpression property = Expression.Property(parameterExpression, objectType.GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(objectType), memberBindingList.ToArray());
            Expression<Func<TObject, TObject>> lambda = Expression.Lambda<Func<TObject, TObject>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        /// <summary>
        /// 浅拷贝对象
        /// </summary>
        /// <param name="tObject"></param>
        /// <returns></returns>
        public static TObject ShallowCopy(TObject tObject)
        {
            return cache(tObject);
        }
    }

    //public static class ObjectClone1<TOut>
    //{
    //    #region 浅拷贝
    //    private static Dictionary<Type, Func<object, TOut>> _dic = new Dictionary<Type, Func<object, TOut>>();

    //    private static Func<object, TOut> GetFunc(Type tInType)
    //    {
    //        ParameterExpression parameterExpression = Expression.Parameter(tInType, "p");
    //        List<MemberBinding> memberBindingList = new List<MemberBinding>();

    //        foreach (var item in typeof(TOut).GetProperties())
    //        {
    //            if (item.CanWrite == false)
    //            {
    //                continue;
    //            }

    //            MemberExpression property = Expression.Property(parameterExpression, tInType.GetProperty(item.Name));
    //            MemberBinding memberBinding = Expression.Bind(item, property);
    //            memberBindingList.Add(memberBinding);
    //        }

    //        MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
    //        Expression<Func<object, TOut>> lambda = Expression.Lambda<Func<object, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

    //        return lambda.Compile();
    //    }

    //    /// <summary>
    //    /// 浅拷贝对象
    //    /// </summary>
    //    /// <param name="tIn"></param>
    //    /// <returns></returns>
    //    public static TOut ShallowCopy(object inObject)
    //    {
    //        Type tInType = inObject.GetType();

    //        Func<object, TOut> cache;

    //        if (_dic.TryGetValue(tInType,out cache) == false)
    //        {
    //            cache = GetFunc(tInType);
    //            _dic.Add(tInType, cache);
    //        }

    //        return cache(inObject);
    //    }
    //    #endregion
    //}
}