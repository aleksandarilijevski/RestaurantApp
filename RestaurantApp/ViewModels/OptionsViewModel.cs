using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

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

        public OptionsViewModel(IRegionManager regionManager,IDialogService dialogService)
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
