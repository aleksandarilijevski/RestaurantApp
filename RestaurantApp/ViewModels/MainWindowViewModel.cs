﻿using EntityFramework.Models;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Input;

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

        private DelegateCommand _checkIfConfigFileExistsCommand;

        private int InactivityTimer = 900000;

        private Timer mouseInactivityTimer;

        public MainWindowViewModel(IRegionManager regionManager, IDatabaseService databaseService, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
            _dialogService = dialogService;

            _databaseService.GetConfiguration();

            InitializeMouseInactivityTimer();
            InitializeMouseEvents();
        }

        public DelegateCommand NavigateToArticleManagementCommand
        {
            get
            {
                _navigateToArticleManagementCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.ArticleManagementViewName));
                return _navigateToArticleManagementCommand;
            }
        }

        public DelegateCommand CheckIfConfigFileExistsCommand
        {
            get
            {
                _checkIfConfigFileExistsCommand = new DelegateCommand(CheckIfConfigFileExists);
                return _checkIfConfigFileExistsCommand;
            }
        }

        public DelegateCommand NavigateToUserManagementCommand
        {
            get
            {
                _navigateToUserManagementCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.UserManagementViewName));
                return _navigateToUserManagementCommand;
            }
        }

        public DelegateCommand NavigateToMenuCommand
        {
            get
            {
                _navigateToMenuCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.OptionsViewName));
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
                _showTableOverviewCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.TableOrderViewName));
                return _showTableOverviewCommand;
            }
        }

        public DelegateCommand NavigateToOnlineOrderingCommand
        {
            get
            {
                _navigateToOnlineOrderingCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.OnlineOrdersViewName));
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

        private void InitializeMouseInactivityTimer()
        {
            mouseInactivityTimer = new Timer(InactivityTimer);
            mouseInactivityTimer.Elapsed += MouseInactivityTimerElapsed;
            mouseInactivityTimer.Start();
        }

        private void InitializeMouseEvents()
        {
            Mouse.AddMouseMoveHandler(Application.Current.MainWindow, MouseMoveHandler);
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            mouseInactivityTimer.Stop();
            mouseInactivityTimer.Start();
        }

        private void MouseInactivityTimerElapsed(object sender, ElapsedEventArgs e)
        {
            LoggedUserHelper.LoggedUser = null;
        }

        private void CheckIfConfigFileExists()
        {
            if (!File.Exists("config.ini"))
            {
                do
                {
                    _dialogService.ShowDialog(ViewConstants.CompanyInformationsDialogViewName);
                } while (!File.Exists("config.ini"));
            }

            string data = string.Empty;

            using (StreamReader streamReader = new StreamReader("config.ini"))
            {
                data = streamReader.ReadToEnd();
            }

            JObject parsedData = JObject.Parse(data);

            if (parsedData.Count == 0)
            {
                MessageBox.Show(MessageBoxConstants.ConfigFileIsNotInRightFormat, MessageBoxConstants.CompanyInformationsTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                File.Delete("config.ini");

                do
                {
                    _dialogService.ShowDialog(ViewConstants.CompanyInformationsDialogViewName);
                } while (!File.Exists("config.ini"));

                return;
            }

            DrawningHelper.CompanyName = parsedData["Company name"].ToString();
            DrawningHelper.CompanyAddress = parsedData["Company address"].ToString();
            DrawningHelper.BillOutputPath = parsedData["Bill output path"].ToString();
            DrawningHelper.PDV = int.Parse(parsedData["PDV"].ToString());
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
            ObservableCollection<User> users = new ObservableCollection<User>();
            EFContext efContext = new EFContext();

            do
            {
                users = await _databaseService.GetAllUsers(efContext);

                if (users.Count == 0)
                {
                    _dialogService.ShowDialog(ViewConstants.AddUserDialogViewName);
                }

            } while (users.Count == 0);
        }
    }
}
