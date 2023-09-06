using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System;

namespace RestaurantApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private IDatabaseService _databaseService;
        private DelegateCommand _navigateToArticleManagementCommand;
        private DelegateCommand _navigateToWaiterManagementCommand;
        private DelegateCommand _navigateToMenuCommand;
        private DelegateCommand _navigateToOnlineOrderingCommand;
        private DelegateCommand _loadConfigurationCommand;
        private DelegateCommand _showTableOverviewCommand;

        public MainWindowViewModel(IRegionManager regionManager, IDatabaseService databaseService)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;
        }

        public DelegateCommand NavigateToArticleManagementCommand
        {
            get
            {
                _navigateToArticleManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "ArticleManagement"));
                return _navigateToArticleManagementCommand;
            }
        }

        public DelegateCommand NavigateToWaiterManagementCommand
        {
            get
            {
                _navigateToWaiterManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "WaiterManagement"));
                return _navigateToWaiterManagementCommand;
            }
        }

        public DelegateCommand NavigateToMenuCommand
        {
            get
            {
                _navigateToMenuCommand = new DelegateCommand(() => Navigate("MainRegion", "Options"));
                return _navigateToMenuCommand;
            }
        }

        public DelegateCommand LoadConfigurationCommand
        {
            get
            {
                _loadConfigurationCommand = new DelegateCommand(LoadConfiguration);
                return _loadConfigurationCommand;
            }
        }

        public DelegateCommand NavigateToTableOrderCommand
        {
            get
            {
                _showTableOverviewCommand = new DelegateCommand(() => Navigate("MainRegion", "TableOrder"));
                return _showTableOverviewCommand;
            }
        }

        public DelegateCommand NavigateToOnlineOrderingCommand
        {
            get
            {
                _navigateToOnlineOrderingCommand = new DelegateCommand(() => Navigate("MainRegion", "OnlineOrdering"));
                return _navigateToOnlineOrderingCommand;
            }
        }

        private void Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(regionName, viewName);
        }

        private async void LoadConfiguration()
        {
            Configuration configuration = await _databaseService.GetConfiguration();

            if (configuration == null)
            {
                Configuration newConfiguration = new Configuration { CurrentDate = DateTime.Now, BillCounter = 0 };
                await _databaseService.CreateConfiguration(newConfiguration);
            }

            configuration = await _databaseService.GetConfiguration();

            if (configuration.CurrentDate.Date != DateTime.Now.Date)
            {
                configuration.CurrentDate = DateTime.Now;
                configuration.BillCounter = 0;
                await _databaseService.EditConfiguration(configuration);
            }
        }
    }
}
