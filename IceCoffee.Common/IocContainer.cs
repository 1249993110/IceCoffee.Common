//using Autofac;
//using Autofac.Core;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace JT809_Superior.Sockets
//{
//    /// <summary>
//    /// 控制反转容器，使用Autofac
//    /// </summary>
//    public static class IocContainer
//    {
//        private static readonly ContainerBuilder _builder;

//        private static readonly IContainer _container;

//        static IocContainer()
//        {
//            _builder = new ContainerBuilder();

//            //var assemblys = AppDomain.CurrentDomain.GetAssemblies();

//            // 在程序集中自动查找类型
//            //_builder.RegisterAssemblyTypes(Assembly.LoadFrom("ESIL.Data.DaYaWan.dll")).AsImplementedInterfaces();
//            //_builder.RegisterAssemblyTypes(assemblys.FirstOrDefault(x => x.FullName.StartsWith("JT809_Superior.Sockets")));

//            //_builder.Register(c => new JT809Serializer(new JT809_2011_Config()));//.SingleInstance();
//            //_builder.RegisterType<JT809Serializer>()
//            //    .WithParameter(new TypedParameter(typeof(IJT809Config), new JT809_2011_Config()));
//            //_builder.RegisterType<LinkManagementHandler>().As<ILinkManagementHandler>().SingleInstance();

//            _container = _builder.Build();
//        }

//        /// <summary>
//        /// 从上下文中检索服务
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public static T Resolve<T>()
//        {
//            return _container.Resolve<T>();
//        }

//        /// <summary>
//        /// 从上下文中检索服务
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="parameters"></param>
//        /// <returns></returns>
//        public static T Resolve<T>(params Parameter[] parameters)
//        {
//            return _container.Resolve<T>(parameters);
//        }
//    }
//}