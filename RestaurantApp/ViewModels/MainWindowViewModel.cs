using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private DelegateCommand _navigateToArticleManagementCommand;
        private DelegateCommand _navigateToWaiterManagementCommand;
        private DelegateCommand _navigateToMenuCommand;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
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

        private async Task Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(regionName, viewName);
        }
    }
}
