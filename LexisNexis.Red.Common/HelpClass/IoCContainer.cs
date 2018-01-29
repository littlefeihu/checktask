using System;
using Microsoft.Practices.Unity;
using System.Collections.Generic;

namespace LexisNexis.Red.Common.HelpClass
{
    /// <summary>
    /// Singleton container for IOC
    /// </summary>
    public sealed class IoCContainer
    {
        private static readonly IoCContainer container;

        private static readonly UnityContainer unityContainer;

        private IoCContainer()
        {

        }

        static IoCContainer()
        {
            container = new IoCContainer();
            unityContainer = new UnityContainer();

        }

        /// <summary>
        ///  Get the Instance
        /// </summary>
        public static IoCContainer Instance
        {
            get { return container; }
        }

        /// <summary>
        /// RegisterInterface
        /// </summary>
        /// <typeparam name="TInterface">interface</typeparam>
        /// <typeparam name="TImplement">implement class</typeparam>
        public void RegisterInterface<TInterface, TImplement>() where TImplement : TInterface
        {
            unityContainer.RegisterType<TInterface, TImplement>(new ContainerControlledLifetimeManager());
        }
        public void RegisterInterface<TInterface, TImplement>(string name) where TImplement : TInterface
        {
            unityContainer.RegisterType<TInterface, TImplement>(name, new ContainerControlledLifetimeManager());
        }
        /// <summary>
        /// Register instance
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="instance"></param>
        public void RegisterInstance<TInterface>(TInterface instance)
        {
            unityContainer.RegisterInstance<TInterface>(instance, new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// ResolveInterface
        /// </summary>
        /// <typeparam name="TInterface">interface</typeparam>
        /// <returns>implement instance</returns>
        public TInterface ResolveInterface<TInterface>()
        {
            return unityContainer.Resolve<TInterface>();
        }
        public TInterface ResolveInterface<TInterface>(string name)
        {
            return unityContainer.Resolve<TInterface>(name);
        }

        /// <summary>
        /// Resolve type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return unityContainer.Resolve(type);
        }

        public T Resolve<T>()
        {
            return unityContainer.Resolve<T>();
        }
        public T Resolve<T>(string name)
        {
            return unityContainer.Resolve<T>(name);
        }
        public IEnumerable<T> ResolveAll<T>()
        {
            return unityContainer.ResolveAll<T>();
        }

    }
}
