﻿using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using RestauranApp.Factories.Extensions;
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
            IContainerProvider containerProvider = Container.Resolve<IContainerProvider>();
            containerRegistry.AddAbstractFactory<IDatabaseService, DatabaseService>(containerProvider);
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
            ViewModelLocationProvider.Register<TableOrder, TableOrderViewModel>();
            ViewModelLocationProvider.Register<Options, OptionsViewModel>();
            ViewModelLocationProvider.Register<ArticleManagement, ArticleManagementViewModel>();
            ViewModelLocationProvider.Register<EditArticle, EditArticleViewModel>();
            ViewModelLocationProvider.Register<AddArticle, AddArticleViewModel>();
            ViewModelLocationProvider.Register<UserManagement, UserManagementViewModel>();
            ViewModelLocationProvider.Register<EditUser, EditUserViewModel>();
            ViewModelLocationProvider.Register<AddUser, AddUserViewModel>();
            ViewModelLocationProvider.Register<Ordering, OrderingViewModel>();
            ViewModelLocationProvider.Register<Payment, PaymentViewModel>();
            ViewModelLocationProvider.Register<AddArticleByDataEntry, AddArticleByDataEntryViewModel>();
            ViewModelLocationProvider.Register<ArticleDetailsUserControl, ArticleDetailsViewModel>();
            ViewModelLocationProvider.Register<PaymentDialog, PaymentDialogViewModel>();
            ViewModelLocationProvider.Register<ReportManagement, ReportManagementViewModel>();
            ViewModelLocationProvider.Register<ReportDetails, ReportDetailsViewModel>();
            ViewModelLocationProvider.Register<OnlineOrdering, OnlineOrderingViewModel>();
            ViewModelLocationProvider.Register<DataEntryDetails, DataEntryDetailsViewModel>();
            ViewModelLocationProvider.Register<UserLoginDialog, UserLoginDialogViewModel>();
            ViewModelLocationProvider.Register<TableInvoiceHistory, TableInvoiceHistoryViewModel>();
            ViewModelLocationProvider.Register<CompanyInformations, CompanyInformationsViewModel>();
            ViewModelLocationProvider.Register<OnlineOrders, OnlineOrdersViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<MainModule>();
        }
    }
}
