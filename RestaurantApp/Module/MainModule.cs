using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using RestaurantApp.Views;

namespace RestaurantApp.Module
{
    class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager region = containerProvider.Resolve<IRegionManager>();
            region.RegisterViewWithRegion("MainRegion", typeof(Options));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TableOrder>();
            containerRegistry.RegisterForNavigation<Options>();
            containerRegistry.RegisterForNavigation<MainWindow>();
            containerRegistry.RegisterForNavigation<ArticalManagement>();
        }
    }
}
