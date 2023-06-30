using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace RestaurantApp.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private DelegateCommand _showTableOverviewCommand;
        private DelegateCommand _showArticalManagementCommand;

        public OptionsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public DelegateCommand ShowTableOverviewCommand
        {
            get
            {
                _showTableOverviewCommand = new DelegateCommand(() => Navigate("MainRegion", "TableOrder"));
                return _showTableOverviewCommand;
            }
        }

        public DelegateCommand ShowArticalManagementCommand
        {
            get
            {
                _showArticalManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "ArticalManagement"));
                return _showArticalManagementCommand;
            }
        }

        private void Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(regionName, viewName);
        }
    }
}
