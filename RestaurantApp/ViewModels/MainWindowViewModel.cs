using EntityFramework.Enums;
using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;

namespace RestaurantApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DelegateCommand _navigateToArticleManagementCommand;
        private DelegateCommand _navigateToUserManagementCommand;
        private DelegateCommand _navigateToMenuCommand;
        private DelegateCommand _navigateToOnlineOrderingCommand;
        private DelegateCommand _loadConfigurationCommand;
        private DelegateCommand _showTableOverviewCommand;
        private DelegateCommand _checkIfAnyUserExistsCommand;

        public MainWindowViewModel(IRegionManager regionManager, IDatabaseService databaseService, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _databaseService = databaseService;
            _dialogService = dialogService;
        }

        public DelegateCommand NavigateToArticleManagementCommand
        {
            get
            {
                _navigateToArticleManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "ArticleManagement"));
                return _navigateToArticleManagementCommand;
            }
        }

        public DelegateCommand NavigateToUserManagementCommand
        {
            get
            {
                _navigateToUserManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "UserManagement"));
                return _navigateToUserManagementCommand;
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

        public DelegateCommand CheckIfAnyUserExistsCommand
        {
            get
            {
                _checkIfAnyUserExistsCommand = new DelegateCommand(CheckIfAnyUserExists);
                return _checkIfAnyUserExistsCommand;
            }
        }

        private void Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(regionName, viewName);
        }

        private async void LoadConfiguration()
        {
            using EFContext efContext = new EFContext();

            Configuration configuration = await _databaseService.GetConfiguration();

            if (configuration == null)
            {
                Configuration newConfiguration = new Configuration { CurrentDate = DateTime.Now, BillCounter = 0 };
                await _databaseService.CreateConfiguration(newConfiguration, efContext);
            }

            configuration = await _databaseService.GetConfiguration();

            if (configuration.CurrentDate.Date != DateTime.Now.Date)
            {
                configuration.CurrentDate = DateTime.Now;
                configuration.BillCounter = 0;
                await _databaseService.EditConfiguration(configuration, efContext);
            }
        }

        private async void CheckIfAnyUserExists()
        {
            bool exist = false;

            do
            {
                exist = await _databaseService.CheckIfAnyUserExists();
                
                if (!exist)
                {
                    _dialogService.ShowDialog("addUserDialog");
                }

            } while (!exist);
        }
    }
}
