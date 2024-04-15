using Prism.Ioc;
using RestaurantApp.Factories.Interfaces;
using System;

namespace RestauranApp.Factories.Extensions
{
    public static class AbstractFactoryExtensions
    {
        public static void AddAbstractFactory<TInterface, TImplementation>(this IContainerRegistry registryContainer, IContainerProvider containerProvider)
        where TInterface : class
        where TImplementation : class, TInterface
        {
            registryContainer.Register<TInterface, TImplementation>();
            registryContainer.RegisterSingleton<Func<TInterface>>(() => () => containerProvider.Resolve<TInterface>());
            registryContainer.RegisterSingleton<IAbstractFactory<TInterface>, AbstractFactoryExtensions<TInterface>>();
        }
    }

    public class AbstractFactoryExtensions<T> : IAbstractFactory<T>
    {
        private readonly Func<T> _factory;

        public AbstractFactoryExtensions(Func<T> factory)
        {
            _factory = factory;
        }

        public T Create()
        {
            return _factory();
        }
    }
}
