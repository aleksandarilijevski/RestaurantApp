using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace RestaurantApp.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private DelegateCommand _showTableOverviewCommand;
        private DelegateCommand _showArticleManagementCommand;
        private DelegateCommand _showWaiterManagementCommand;
        private DelegateCommand _showReportManagementCommand;
        private DelegateCommand _showOnlineOrderingCommand;

        public OptionsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public DelegateCommand ShowWaiterManagementCommand
        {
            get
            {
                _showWaiterManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "WaiterManagement"));
                return _showWaiterManagementCommand;
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

        private void Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(regionName, viewName);
        }
    }
}
