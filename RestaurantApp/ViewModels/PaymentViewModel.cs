using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class PaymentViewModel : BindableBase,INavigationAware
    {
        private Table _table;
        private decimal _totalPrice;
        private DelegateCommand _getTotalPriceCommand;

        public PaymentViewModel()
        {

        }

        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                _totalPrice = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand GetTotalPriceCommand
        {
            get
            {
                _getTotalPriceCommand = new DelegateCommand(GetTotalPrice);
                return _getTotalPriceCommand;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _table = (Table)navigationContext.Parameters["table"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void GetTotalPrice()
        {
            _totalPrice = CalculateTotalPrice();
            RaisePropertyChanged(nameof(TotalPrice));
        }

        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (Article article in _table.Articles)
            {
                totalPrice += article.Price * article.Quantity;
            }

            return totalPrice;
        }
    }
}
