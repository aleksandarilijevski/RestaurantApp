using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Utilities.Helpers;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        private string _message;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private DelegateCommand _showTableOverviewCommand;

        private DelegateCommand _showArticleManagementCommand;

        private DelegateCommand _showUserManagementCommand;

        private DelegateCommand _showReportManagementCommand;

        private DelegateCommand _showOnlineOrderingCommand;

        private DelegateCommand _showCompanyInformationsDialogCommand;

        private DelegateCommand _showLoggedUserCommand;

        private DelegateCommand _logoutUserCommand;

        public OptionsViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        public User LoggedUser { get; set; }

        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ShowUserManagementCommand
        {
            get
            {
                _showUserManagementCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.UserManagementViewName));
                return _showUserManagementCommand;
            }
        }

        public DelegateCommand ShowTableOverviewCommand
        {
            get
            {
                _showTableOverviewCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.TableOrderViewName));
                return _showTableOverviewCommand;
            }
        }

        public DelegateCommand ShowArticleManagementCommand
        {
            get
            {
                _showArticleManagementCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.ArticleManagementViewName));
                return _showArticleManagementCommand;
            }
        }

        public DelegateCommand ShowReportManagementCommand
        {
            get
            {
                _showReportManagementCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.ReportManagementViewName));
                return _showReportManagementCommand;
            }
        }

        public DelegateCommand ShowOnlineOrderingCommand
        {
            get
            {
                _showOnlineOrderingCommand = new DelegateCommand(() => Navigate(ViewConstants.MainRegionViewName, ViewConstants.OnlineOrderingViewName));
                return _showOnlineOrderingCommand;
            }
        }

        public DelegateCommand ShowCompanyInformationsDialogCommand
        {
            get
            {
                _showCompanyInformationsDialogCommand = new DelegateCommand(ShowCompanyInformationsDialog);
                return _showCompanyInformationsDialogCommand;
            }
        }

        public DelegateCommand ShowLoggedUserCommand
        {
            get
            {
                _showLoggedUserCommand = new DelegateCommand(ShowLoggedUser);
                return _showLoggedUserCommand;
            }
        }

        public DelegateCommand LogoutUserCommand
        {
            get
            {
                _logoutUserCommand = new DelegateCommand(LogoutUser);
                return _logoutUserCommand;
            }
        }

        private void LogoutUser()
        {
            if (LoggedUserHelper.LoggedUser is null)
            {
                MessageBox.Show(MessageBoxConstants.YouAreLoggedOut, MessageBoxConstants.MainMenuTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (LoggedUserHelper.LoggedUser is not null)
            {
                MessageBox.Show(MessageBoxConstants.YouAreLoggedOut, MessageBoxConstants.MainMenuTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                LoggedUserHelper.LoggedUser = null;
                Message = MessageBoxConstants.NoOneIsLogged;
            }
        }

        private void ShowLoggedUser()
        {
            LoggedUser = LoggedUserHelper.LoggedUser;

            if (LoggedUser is null)
            {
                Message = MessageBoxConstants.NoOneIsLogged;
            }

            if (LoggedUser is not null)
            {
                Message = $"User : {LoggedUser.FirstAndLastName} is logged!         |           Role : {LoggedUser.UserRole}";
            }
        }

        private void Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(regionName, viewName);
        }

        private void ShowCompanyInformationsDialog()
        {
            _dialogService.ShowDialog(ViewConstants.CompanyInformationsDialogViewName);
        }
    }
}
