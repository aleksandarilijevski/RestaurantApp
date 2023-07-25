using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using RestaurantApp.ViewModels;
using RestaurantApp.Views;

namespace RestaurantApp.Module
{
    class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager region = containerProvider.Resolve<IRegionManager>();
            region.RegisterViewWithRegion("MainRegion", typeof(Options));
            region.RegisterViewWithRegion("OrderingRegion","Ordering");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TableOrder>();
            containerRegistry.RegisterForNavigation<Options>();
            containerRegistry.RegisterForNavigation<MainWindow>();
            containerRegistry.RegisterForNavigation<ArticleManagement>();
            containerRegistry.RegisterForNavigation<WaiterManagement>();
            containerRegistry.RegisterForNavigation<Ordering>();
            containerRegistry.RegisterForNavigation<Payment>();
            containerRegistry.RegisterForNavigation<AddArticleByDataEntry>();
            containerRegistry.RegisterForNavigation<ArticleDetails>();
            containerRegistry.RegisterDialog<EditArticle, EditArticleViewModel>("editArticleDialog");
            containerRegistry.RegisterDialog<AddArticle, AddArticleViewModel>("addArticleDialog");
            containerRegistry.RegisterDialog<EditWaiter, EditWaiterViewModel>("editWaiterDialog");
            containerRegistry.RegisterDialog<AddWaiter, AddWaiterViewModel>("addWaiterDialog");
            containerRegistry.RegisterDialog<EditArticleDetail, EditArticleDetailViewModel>("editArticleDetailDialog");
        }
    }
}
