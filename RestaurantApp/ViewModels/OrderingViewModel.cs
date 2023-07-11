using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Diagnostics.Contracts;

namespace RestaurantApp.ViewModels
{
    public class OrderingViewModel : BindableBase,INavigationAware
    {
        private IDatabaseService _databaseService;
        private int _id;

        public int ID
        {
            get { return _id; }
        }

        public OrderingViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _id = int.Parse(navigationContext.Parameters["id"].ToString());
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
