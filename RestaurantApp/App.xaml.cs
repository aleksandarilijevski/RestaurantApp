using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using RestaurantApp.Module;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;
using RestaurantApp.ViewModels;
using RestaurantApp.Views;
using System.Windows;

namespace RestaurantApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            MainWindow window = Container.Resolve<MainWindow>();
            return window;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDatabaseService, DatabaseService>();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
            ViewModelLocationProvider.Register<TableOrder, TableOrderViewModel>();
            ViewModelLocationProvider.Register<Options, OptionsViewModel>();
            ViewModelLocationProvider.Register<ArticleManagement, ArticleManagementViewModel>();
            ViewModelLocationProvider.Register<EditArticle, EditArticleViewModel>();
            ViewModelLocationProvider.Register<AddArticle, AddArticleViewModel>();
            ViewModelLocationProvider.Register<WaiterManagement, WaiterManagementViewModel>();
            ViewModelLocationProvider.Register<EditWaiter, EditWaiterViewModel>();
            ViewModelLocationProvider.Register<AddWaiter, AddWaiterViewModel>();
            ViewModelLocationProvider.Register<Ordering, OrderingViewModel>();
            ViewModelLocationProvider.Register<Payment, PaymentViewModel>();
            ViewModelLocationProvider.Register<AddArticleByDataEntry, AddArticleByDataEntryViewModel>();
            ViewModelLocationProvider.Register<ArticleDetails, ArticleDetailsViewModel>();
            ViewModelLocationProvider.Register<EditArticleDetail, EditArticleDetailViewModel>();
            ViewModelLocationProvider.Register<PaymentDialog, PaymentDialogViewModel>();
            ViewModelLocationProvider.Register<ReportManagement, ReportManagementViewModel>();
            ViewModelLocationProvider.Register<ReportDetails, ReportDetailsViewModel>();
            ViewModelLocationProvider.Register<OnlineOrdering, OnlineOrderingViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<MainModule>();
        }
    }
}
