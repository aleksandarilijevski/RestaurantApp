using Prism.Ioc;
using RestaurantApp.Factories.Interfaces;
using System;

namespace RestaurantApp.Factories
{
    public static class AbstractFactoryExtenstion
    {
        public static void AddAbstractFactory<TInterface, TImplementation>(this IContainerRegistry registryContainer, IContainerProvider containerProvider)
        where TInterface : class
        where TImplementation : class, TInterface
        {
            registryContainer.Register<TInterface, TImplementation>();
            registryContainer.RegisterSingleton<Func<TInterface>>(() => () => containerProvider.Resolve<TInterface>());
            registryContainer.RegisterSingleton<IAbstractFactory<TInterface>, AbstractFactory<TInterface>>();
        }
    }

    public class AbstractFactory<T> : IAbstractFactory<T>
    {
        private readonly Func<T> _factory;

        public AbstractFactory(Func<T> factory)
        {
            _factory = factory;
        }

        public T Create()
        {
            return _factory();
        }
    }
}
