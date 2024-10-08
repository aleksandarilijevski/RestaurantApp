﻿using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using RestaurantApp.ViewModels;
using RestaurantApp.Views;
using System;
using System.Windows;

namespace RestaurantApp.Module
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            CreateDB();
            SplashScreen();
            IRegionManager region = containerProvider.Resolve<IRegionManager>();
            region.RegisterViewWithRegion("MainRegion", "Options");
            region.RegisterViewWithRegion("OrderingRegion", "Ordering");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TableOrder>();
            containerRegistry.RegisterForNavigation<Options>();
            containerRegistry.RegisterForNavigation<MainWindow>();
            containerRegistry.RegisterForNavigation<ArticleManagement>();
            containerRegistry.RegisterForNavigation<UserManagement>();
            containerRegistry.RegisterForNavigation<Ordering>();
            containerRegistry.RegisterForNavigation<Payment>();
            containerRegistry.RegisterForNavigation<AddArticleByDataEntry>();
            containerRegistry.RegisterForNavigation<ArticleDetailsUserControl>();
            containerRegistry.RegisterForNavigation<ReportManagement>();
            containerRegistry.RegisterForNavigation<OnlineOrdering>();
            containerRegistry.RegisterForNavigation<DataEntryManagement>();
            containerRegistry.RegisterForNavigation<OnlineOrders>();
            containerRegistry.RegisterDialog<EditArticle, EditArticleViewModel>("editArticleDialog");
            containerRegistry.RegisterDialog<AddArticle, AddArticleViewModel>("addArticleDialog");
            containerRegistry.RegisterDialog<EditUser, EditUserViewModel>("editUserDialog");
            containerRegistry.RegisterDialog<AddUser, AddUserViewModel>("addUserDialog");
            containerRegistry.RegisterDialog<PaymentDialog, PaymentDialogViewModel>("paymentDialog");
            containerRegistry.RegisterDialog<ReportDetails, ReportDetailsViewModel>("reportDetailsDialog");
            containerRegistry.RegisterDialog<DataEntryDetails, DataEntryDetailsViewModel>("dataEntryDetailsDialog");
            containerRegistry.RegisterDialog<UserLoginDialog, UserLoginDialogViewModel>("userLoginDialog");
            containerRegistry.RegisterDialog<TableInvoiceHistory, TableInvoiceHistoryViewModel>("tableInvoiceHistoryDialog");
            containerRegistry.RegisterDialog<CompanyInformations, CompanyInformationsViewModel>("companyInformationsDialog");
        }

        private void SplashScreen()
        {
            SplashScreen splashScreen = new SplashScreen("/Resources/loadingScreen.png");
            splashScreen.Show(false);
            splashScreen.Close(TimeSpan.FromSeconds(1));
        }

        private void CreateDB()
        {
            using (EFContext efContext = new EFContext())
            {
                efContext.Database.Migrate();
            }
        }
    }
}
