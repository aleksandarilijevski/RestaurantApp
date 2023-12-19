using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace RestaurantApp.ViewModels
{
    public class OnlineOrdersViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private DelegateCommand _loadOnlineOrdersCommand;
        private DelegateCommand _addOnlineOrderCommand;
        private DelegateCommand _deleteOnlineOrdersCommand;
        private ObservableCollection<OnlineOrder> _onlineOrders;

        public OnlineOrdersViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public ObservableCollection<OnlineOrder> OnlineOrders
        {
            get
            {
                return _onlineOrders;
            }

            set
            {
                _onlineOrders = value;
                RaisePropertyChanged();
            }
        }

        public OnlineOrder SelectedOnlineOrder { get; set; }

        public DelegateCommand LoadOnlineOrdersCommand
        {
            get
            {
                _loadOnlineOrdersCommand = new DelegateCommand(LoadOnlineOrders);
                return _loadOnlineOrdersCommand;
            }
        }

        public DelegateCommand AddOnlineOrderCommand
        {
            get
            {
                _addOnlineOrderCommand = new DelegateCommand(AddOnlineOrder);
                return _addOnlineOrderCommand;
            }
        }

        public DelegateCommand DeleteOnlineOrderCommand
        {
            get
            {
                _deleteOnlineOrdersCommand = new DelegateCommand(DeleteOnlineOrder);
                return _deleteOnlineOrdersCommand;
            }
        }

        private async void LoadOnlineOrders()
        {
            using EFContext efContext = new EFContext();
            OnlineOrders = await _databaseService.GetAllOnlineOrders(efContext);
        }

        public async void AddOnlineOrder()
        {
            using EFContext efContext = new EFContext();

            int onlineOrderId = await _databaseService.GetMaxOnlineOrderID(efContext) + 1;

            OnlineOrder onlineOrder = new OnlineOrder();
            onlineOrder.ID = onlineOrderId;

            await _databaseService.AddOnlineOrder(onlineOrder, efContext);

            OnlineOrders.Add(onlineOrder);
        }

        private async void DeleteOnlineOrder()
        {
            using EFContext efContext = new EFContext();

            if (SelectedOnlineOrder is null)
            {
                MessageBox.Show("Online order is not selected!", "Online ordering", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await _databaseService.DeleteOnlineOrder(SelectedOnlineOrder, efContext);
            OnlineOrders.Remove(SelectedOnlineOrder);
        }
    }
}
