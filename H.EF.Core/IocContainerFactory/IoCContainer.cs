using System;
using Autofac;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.DependencyModel;
using H.EF.Core.Helpers;

namespace H.EF.Core
{
    /// <summary>
    /// 容器工厂
    /// </summary>
    public static class IoCContainer
    {
        /// <summary>
        /// 容器对象
        /// </summary>
        private static IContainer _container;
        private static ContainerBuilder _builder = new ContainerBuilder();
        private static string[] _otherAssembly;
        private static List<Type> _types = new List<Type>();
        private static Dictionary<Type, Type> _dicTypesSingleInstance = new Dictionary<Type, Type>();
        private static Dictionary<Type, Type> _dicTypesInstancePerDependency = new Dictionary<Type, Type>();

        /// <summary>
        /// ContainerBuilder
        /// </summary>
        public static ContainerBuilder Container
        {
            get
            {
                return _builder;
            }
        }

        /// <summary>
        /// Ioc容器初始化
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void Initialize(IServiceCollection services)
        {
            //所有程序集 和程序集下类型
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");//排除所有的系统程序集、Nuget下载包
            var listAllType = new List<Type>();
            foreach (var lib in libs)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    listAllType.AddRange(assembly.GetTypes().Where(type => type != null));
                }
                catch { }
            }
            //找到所有外部IDependencyRegistrar实现，调用注册
            var registrarType = typeof(IDependencyRegistrar);
            var arrRegistrarType = listAllType.Where(t => registrarType.IsAssignableFrom(t) && t != registrarType).ToArray();
            var listRegistrarInstances = new List<IDependencyRegistrar>();
            foreach (var drType in arrRegistrarType)
            {
                listRegistrarInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            }
            //排序
            listRegistrarInstances = listRegistrarInstances.OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in listRegistrarInstances)
            {
                dependencyRegistrar.Register(_builder, listAllType);
            }

            //注册ITransientDependency实现类
            var dependencyType = typeof(ITransientDependency);
            var arrDependencyType = listAllType.Where(t => dependencyType.IsAssignableFrom(t) && t != dependencyType).ToArray();
            _builder.RegisterTypes(arrDependencyType)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired().EnableInterfaceInterceptors();

            foreach (Type type in arrDependencyType)
            {
                if (type.IsClass && !type.IsAbstract && !type.BaseType.IsInterface && type.BaseType != typeof(object))
                {
                    _builder.RegisterType(type).As(type.BaseType)
                        .InstancePerLifetimeScope()
                        .PropertiesAutowired();
                }
            }


            //注册ISingletonDependency实现类
            var singletonDependencyType = typeof(ISingletonDependency);
            var arrSingletonDependencyType = listAllType.Where(t => singletonDependencyType.IsAssignableFrom(t) && t != singletonDependencyType).ToArray();
            _builder.RegisterTypes(arrSingletonDependencyType)
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired();

            foreach (Type type in arrSingletonDependencyType)
            {
                if (type.IsClass && !type.IsAbstract && !type.BaseType.IsInterface && type.BaseType != typeof(object))
                {
                    _builder.RegisterType(type).As(type.BaseType)
                        .SingleInstance()
                        .PropertiesAutowired();
                }
            }
        }

        /// <summary>
        /// 注册程序集
        /// </summary>
        /// <param name="assemblies">程序集名称的集合</param>
        public static void Register(params string[] assemblies)
        {
            _otherAssembly = assemblies;
        }

        /// <summary>
        /// 注册一个实体(单例)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void Register<T>(T instance) where T : class
        {
            _builder.RegisterInstance(instance).SingleInstance();
        }

        /// <summary>
        /// 注册一个单例实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>() where T : class
        {
            _builder.RegisterType<T>().SingleInstance(); 
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="types"></param>
        public static void Register(params Type[] types)
        {
            _types.AddRange(types.ToList());
        }

        /// <summary>
        /// 构建IOC容器，需在各种Register后调用。
        /// </summary>
        public static IServiceProvider Build(IServiceCollection services)
        {
            if (_otherAssembly != null)
            {
                foreach (var item in _otherAssembly)
                {
                    _builder.RegisterAssemblyTypes(Assembly.Load(item));
                }
            }

            if (_types != null)
            {
                foreach (var type in _types)
                {
                    _builder.RegisterType(type);
                }
            }

            if (_dicTypesSingleInstance != null)
            {
                foreach (var dicType in _dicTypesSingleInstance)
                {
                    _builder.RegisterType(dicType.Value).SingleInstance().As(dicType.Key);
                }
            }

            if (_dicTypesInstancePerDependency != null)
            {
                foreach (var dicType in _dicTypesInstancePerDependency)
                {
                    _builder.RegisterType(dicType.Value).InstancePerDependency().As(dicType.Key);
                }
            }
            RegisterService();

            _builder.Populate(services);
            _container = _builder.Build();
            return new AutofacServiceProvider(_container);
        }
        /// <summary>
        /// 注册拦截方法
        /// </summary>
        public static void RegisterService()
        {
            AssemblyHelpers _assemblyHelper = new AssemblyHelpers();
            var assemblys = _assemblyHelper.GetAssemblies();
            _builder.RegisterType<TransactionInterceptor>();
            foreach (var assembly in assemblys)
            {
                var contains = assembly.GetCustomAttributes().Where(type => type.Equals(typeof(TransactionHandlerAttribute)));
                if (contains.Count() > 0)
                {
                    _builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                    .EnableInterfaceInterceptors().InterceptedBy(typeof(TransactionInterceptor));

                }
            }
        }
        /// <summary>
        /// 注册程序集。
        /// </summary>
        /// <param name="implementationAssemblyName"></param>
        /// <param name="interfaceAssemblyName"></param>
        public static void Register(string implementationAssemblyName, string interfaceAssemblyName)
        {
            var implementationAssembly = Assembly.Load(implementationAssemblyName);
            var interfaceAssembly = Assembly.Load(interfaceAssemblyName);
            var implementationTypes =
                implementationAssembly.DefinedTypes.Where(t =>
                    t.IsClass && !t.IsAbstract && !t.IsGenericType && !t.IsNested);
            foreach (var type in implementationTypes)
            {
                var interfaceTypeName = interfaceAssemblyName + ".I" + type.Name;
                var interfaceType = interfaceAssembly.GetType(interfaceTypeName);
                if (interfaceType.IsAssignableFrom(type))
                {
                    _dicTypesSingleInstance.Add(interfaceType, type);
                }
            }
        }
        /// <summary>
        /// 注册(单例)
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterSingleInstance<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _dicTypesSingleInstance.Add(typeof(TInterface), typeof(TImplementation));
        }

        /// <summary>
        /// 注册(依赖实例)
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterInstancePerDependency<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _dicTypesInstancePerDependency.Add(typeof(TInterface), typeof(TImplementation));
        }

        /// <summary>
        /// Resolve an instance of the default requested type from the container
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

    }
}