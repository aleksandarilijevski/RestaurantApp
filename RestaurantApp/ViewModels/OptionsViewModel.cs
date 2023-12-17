using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Utilities.Helpers;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
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
        private string _message;

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

        public OptionsViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        public DelegateCommand ShowUserManagementCommand
        {
            get
            {
                _showUserManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "UserManagement"));
                return _showUserManagementCommand;
            }
        }

        public DelegateCommand ShowTableOverviewCommand
        {
            get
            {
                _showTableOverviewCommand = new DelegateCommand(() => Navigate("MainRegion", "TableOrder"));
                return _showTableOverviewCommand;
            }
        }

        public DelegateCommand ShowArticleManagementCommand
        {
            get
            {
                _showArticleManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "ArticleManagement"));
                return _showArticleManagementCommand;
            }
        }

        public DelegateCommand ShowReportManagementCommand
        {
            get
            {
                _showReportManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "ReportManagement"));
                return _showReportManagementCommand;
            }
        }

        public DelegateCommand ShowOnlineOrderingCommand
        {
            get
            {
                _showOnlineOrderingCommand = new DelegateCommand(() => Navigate("MainRegion", "OnlineOrdering"));
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
                MessageBox.Show("No one is logged to be logged out!", "Main menu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (LoggedUserHelper.LoggedUser is not null)
            {
                MessageBox.Show("You're logged out!", "Main menu", MessageBoxButton.OK, MessageBoxImage.Information);
                LoggedUserHelper.LoggedUser = null;
                Message = "No one is logged!";
            }
        }

        private void ShowLoggedUser()
        {
            LoggedUser = LoggedUserHelper.LoggedUser;

            if (LoggedUser is null)
            {
                Message = "No one is logged!";
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
            _dialogService.ShowDialog("companyInformationsDialog");
        }
    }
}
