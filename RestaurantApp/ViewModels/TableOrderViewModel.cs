using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;

namespace RestaurantApp.ViewModels
{
    public class TableOrderViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private DelegateCommand<string> _orderingCommand;
        private int _id;

        public TableOrderViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public int ID
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public DelegateCommand<string> OrderingCommand
        {
            get
            {
                _orderingCommand = new DelegateCommand<string>(ShowOrderingUsercontrol);
                return _orderingCommand;
            }
        }

        private async void ShowOrderingUsercontrol(string id)
        {
            ID = int.Parse(id);

            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "id", ID }
            };

            _regionManager.RequestNavigate("MainRegion", "Ordering", navigationParameters);
        }
    }
}
