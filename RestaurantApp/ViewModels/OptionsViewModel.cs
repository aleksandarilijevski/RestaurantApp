using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private DelegateCommand _showTableOverviewCommand;
        private DelegateCommand _showArticleManagementCommand;

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

        public DelegateCommand ShowArticleManagementCommand
        {
            get
            {
                _showArticleManagementCommand = new DelegateCommand(() => Navigate("MainRegion", "ArticleManagement"));
                return _showArticleManagementCommand;
            }
        }

        private async Task Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(regionName, viewName);
        }
    }
}
